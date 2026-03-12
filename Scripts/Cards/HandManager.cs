using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandManager : MonoBehaviour
{
    [Header("Rule")]
    [SerializeField] private int handSize = 8;

    [Header("Refs")]
    [SerializeField] private Transform handRoot;
    [SerializeField] private CardView cardPrefab;
    [SerializeField] private CardDatabase cardDB;
    [SerializeField] private BoardManager boardMG;

    [Header("UI")]
    [SerializeField] private Button playButton; // 新增：出牌按钮
    [SerializeField] private TMPro.TextMeshProUGUI playBtnText;

    private readonly List<CardView> cards = new();
    private readonly List<CardView> selectedCards = new();

    private bool aiming = false;
    private BasicPattern activePattern; // 本次出牌按“第一张选中牌”结算
    
    private PlayContext currentPlayContext;
    private CardPattern currentPattern = CardPattern.None;

    private enum AimMode
    {
        None,
        Cell,       // Single：点一个格子
        Line,       // Pair：点任意格子决定行/列
        TripleLine, // Triple：点任意格子决定相邻三行/列
        Area4x4     // Rainbow：点任意格子决定4x4左上角（或以该格为中心）
    }

    private AimMode aimMode = AimMode.None;
    
    [Header("Line Mode UI")]
    [SerializeField] private Button lineModeButton;
    [SerializeField] private TMPro.TextMeshProUGUI lineModeText;

    private bool isRowMode = true; // 默认行
    
    private Vector2Int lastAimCell = new(-999, -999);
    private List<Vector2Int> previewCells = new();
    
    private void Awake()
    {
        cardDB.InitializeNormalCards();
        RefreshPlayButtonUI();
        RefreshLineModeUI();
    }
    
    private void OnEnable()
    {
        boardMG.OnCellPointerDown += HandleAimDown;
        boardMG.OnCellDragOver += HandleAimDrag;
        boardMG.OnCellPointerUp += HandleAimUp;
        
        boardMG.ClearAllHighlights();
    }
    
    private void OnDisable()
    {
        if (boardMG != null)
        {
            boardMG.OnCellPointerDown -= HandleAimDown;
            boardMG.OnCellDragOver -= HandleAimDrag;
            boardMG.OnCellPointerUp -= HandleAimUp;
            boardMG.ClearAllHighlights();
        }
        
        aiming = false;
        activePattern = null;
        aimMode = AimMode.None;
        RefreshPlayButtonUI();
        lastAimCell = new Vector2Int(-999, -999);
    }
    
    public void OnTogglePlayMode() => TogglePlayMode();
    
    private void TogglePlayMode()
    {
        if (!aiming)
        {
            if (selectedCards.Count == 0) return;

            currentPlayContext = BuildPlayContext();

            bool hasBase = currentPlayContext.HasBaseEffect;
            bool hasSpecial = currentPlayContext.HasSpecialEffect;

            if (!hasBase && !hasSpecial)
                return;

            currentPattern = currentPlayContext.basePattern;
            activePattern = currentPlayContext.activePattern;

            // 只有特殊牌、没有基础牌：直接执行特殊效果
            if (!hasBase && hasSpecial)
            {
                ExecuteSpecialEffects(currentPlayContext, new Vector2Int(-1, -1));
                ConsumeSelectedCards();
                RefillToHandSize();
                RefreshPlayButtonUI();
                return;
            }

            aiming = true;

            aimMode = currentPattern switch
            {
                CardPattern.Single => AimMode.Cell,
                CardPattern.Pair => AimMode.Line,
                CardPattern.Triple => AimMode.TripleLine,
                CardPattern.Rainbow => AimMode.Area4x4,
                CardPattern.Quad => AimMode.None,
                _ => AimMode.Cell
            };

            if (currentPattern == CardPattern.Quad)
            {
                var strategy = PlayStrategyFactory.GetStrategy(CardPattern.Quad);

                PlayExecutionResult execResult = strategy != null
                    ? strategy.Execute(boardMG, activePattern, -1, -1, isRowMode)
                    : PlayExecutionResult.Fail();

                if (execResult.success)
                {
                    GameEvents.RaiseBoardResolved(new GameEvents.BoardResolvedEvent
                    {
                        pattern = currentPattern,
                        clearedCellCount = execResult.clearedCellCount,
                        clearedLineCount = 0,
                        clearedBaseScore = execResult.clearedBaseScore
                    });

                    GameEvents.RaiseCardPlayed(new GameEvents.CardPlayedEvent
                    {
                        pattern = currentPlayContext != null ? currentPlayContext.basePattern : CardPattern.None,
                        activeShape = currentPlayContext != null ? currentPlayContext.activePattern : null,
                        normalCardCount = currentPlayContext != null ? currentPlayContext.normalCards.Count : 0,
                        specialCardCount = currentPlayContext != null ? currentPlayContext.specialCards.Count : 0
                    });

                    ExecuteSpecialEffects(currentPlayContext, new Vector2Int(-1, -1));
                    ConsumeSelectedCards();
                    RefillToHandSize();
                }

                aiming = false;
                activePattern = null;
                aimMode = AimMode.None;
                currentPlayContext = null;
                currentPattern = CardPattern.None;
                lastAimCell = new Vector2Int(-999, -999);
                boardMG.ClearAllHighlights();
            }
        }
        else
        {
            aiming = false;
            activePattern = null;
            aimMode = AimMode.None;
            boardMG.ClearAllHighlights();
            lastAimCell = new Vector2Int(-999, -999);
        }

        RefreshPlayButtonUI();
    }
    
    public void OnToggleLineMode() => ToggleLineMode();
    
    private void ToggleLineMode()
    {
        isRowMode = !isRowMode;
        RefreshLineModeUI();
    }
    
    public void ResetHand()
    {
        ClearHand();
        RefillToHandSize();
        RefreshPlayButtonUI();
        RefreshLineModeUI();
    }
    
    public void ClearHand()
    {
        // 退出瞄准/清选择
        aiming = false;
        activePattern = null;
        aimMode = AimMode.None;
        currentPattern = CardPattern.None;
        
        foreach (var c in cards)
        {
            if (c != null) Destroy(c.gameObject);
        }
        cards.Clear();
        selectedCards.Clear();
        
        currentPlayContext = null;
        lastAimCell = new Vector2Int(-999, -999);
        previewCells.Clear();
        if (boardMG != null) boardMG.ClearAllHighlights();
        
        // 刷新 UI（按钮状态/行列按钮等）
        RefreshPlayButtonUI();
        RefreshLineModeUI();
    }

    public void RefillToHandSize()
    {
        while (cards.Count < handSize)
        {
            BasicCard card = null;
            
            if (cardDB.AllowSpecialCards)
            {
                card = cardDB.GetSpecialCardRandom();
                if (card == null)
                    card = cardDB.GetNormalCardRandom();
            }
            else
            {
                card = cardDB.GetNormalCardRandom();
            }

            if (card == null)
                break;
            
            var cardView = Instantiate(cardPrefab, handRoot);
            cardView.Init(card);
            cardView.OnClicked += HandleCardClicked;
            cards.Add(cardView);
        }
    }
    
    private void ExecuteSpecialEffects(PlayContext ctx, Vector2Int targetCell)
    {
        if (ctx == null) return;

        var effectContext = new CardEffectContext
        {
            board = boardMG,
            //gameManager = gameMG,
            targetCell = targetCell
        };

        foreach (var cv in ctx.specialCards)
        {
            var effect = cv.Effect;
            if (effect != null)
            {
                effect.Execute(effectContext);

                GameEvents.RaiseEffectExecuted(new GameEvents.SpecialEffectEvent
                {
                    sourceCard = cv.Card,
                    effectName = effect.name
                });
            }
        }
    }
    
    private PlayContext BuildPlayContext()
    {
        var ctx = new PlayContext();

        foreach (var card in selectedCards)
        {
            ctx.selectedCards.Add(card);

            if (card.IsSpecial) ctx.specialCards.Add(card);
            else ctx.normalCards.Add(card);
        }

        var evalResult = HandEvaluator.Evaluate(ctx.normalCards);
        ctx.basePattern = evalResult.pattern;
        ctx.activePattern = evalResult.activePattern;

        return ctx;
    }

    private void HandleCardClicked(CardView cv)
    {
        if (aiming)
        {
            // 瞄准中不允许改选牌（防止你刚要打又换牌导致逻辑混乱）
            return;
        }

        // 多选：点一下加入/再点一下移除
        if (selectedCards.Contains(cv))
        {
            selectedCards.Remove(cv);
            cv.SetSelected(false);
        }
        else
        {
            selectedCards.Add(cv);
            cv.SetSelected(true);
        }
        
        RefreshPlayButtonUI();
    }
    
    private void UpdatePreviewIfNeeded(int x, int y)
    {
        var cell = new Vector2Int(x, y);
        if (cell == lastAimCell) return;
        lastAimCell = cell;

        previewCells = BuildPreviewCells(x, y);
        boardMG.HighlightCells(previewCells);
    }

    private List<Vector2Int> BuildPreviewCells(int x, int y)
    {
        var result = new List<Vector2Int>();

        switch (aimMode)
        {
            case AimMode.Cell:
                // 预览：单格（但只在图案匹配时高亮更合理）
                var block = boardMG.Get(x, y);
                if (block != null && block.pattern == activePattern)
                    result.Add(new Vector2Int(x, y));
                break;

            case AimMode.Line:
                result.AddRange(GetLineCells(x, y, isRowMode));
                break;

            case AimMode.TripleLine:
                result.AddRange(GetTripleLineCells(x, y, isRowMode)); // 用你修过的“边缘三连”逻辑
                break;

            case AimMode.Area4x4:
                result.AddRange(GetArea4x4Cells_RB(x, y)); // 右下角版本
                break;
        }

        return result;
    }
    
    private IEnumerable<Vector2Int> GetLineCells(int x, int y, bool row)
    {
        if (row)
            for (int xx = 0; xx < boardMG.Width; xx++) yield return new Vector2Int(xx, y);
        else
            for (int yy = 0; yy < boardMG.Height; yy++) yield return new Vector2Int(x, yy);
    }

    private IEnumerable<Vector2Int> GetTripleLineCells(int x, int y, bool row)
    {
        if (row)
        {
            int startY = GetTripleRangeStart(y, boardMG.Height);
            for (int yy = startY; yy < startY + 3; yy++)
            for (int xx = 0; xx < boardMG.Width; xx++)
                yield return new Vector2Int(xx, yy);
        }
        else
        {
            int startX = GetTripleRangeStart(x, boardMG.Width);
            for (int xx = startX; xx < startX + 3; xx++)
            for (int yy = 0; yy < boardMG.Height; yy++)
                yield return new Vector2Int(xx, yy);
        }
    }
    
    private int GetTripleRangeStart(int index, int maxExclusive)
    {
        if (index <= 0) return 0;
        if (index >= maxExclusive - 1) return maxExclusive - 3;
        return index - 1;
    }

    private IEnumerable<Vector2Int> GetArea4x4Cells_RB(int x, int y)
    {
        int startX = Mathf.Clamp(x - 3, 0, boardMG.Width - 4);
        int startY = Mathf.Clamp(y - 3, 0, boardMG.Height - 4);

        for (int yy = startY; yy < startY + 4; yy++)
        for (int xx = startX; xx < startX + 4; xx++)
            yield return new Vector2Int(xx, yy);
    }
    
    private void HandleAimDown(int x, int y)
    {
        if (!aiming) return;
        UpdatePreviewIfNeeded(x, y);
    }

    private void HandleAimDrag(int x, int y)
    {
        if (!aiming) return;
        UpdatePreviewIfNeeded(x, y);
    }

    private void HandleAimUp(int x, int y)
    {
        if (!aiming) return;

        // 松开在棋盘外：取消本次预览，不消耗牌，不退出瞄准
        if (x < 0 || y < 0)
        {
            lastAimCell = new Vector2Int(-999, -999); // 让下次拖回棋盘能立刻刷新预览
            return;
        }

        var strategy = PlayStrategyFactory.GetStrategy(currentPattern);

        PlayExecutionResult execResult = strategy != null
            ? strategy.Execute(boardMG, activePattern, x, y, isRowMode)
            : PlayExecutionResult.Fail();

        boardMG.ClearAllHighlights();

        if (execResult.success)
        {
            GameEvents.RaiseBoardResolved(new GameEvents.BoardResolvedEvent
            {
                pattern = currentPattern,
                clearedCellCount = execResult.clearedCellCount,
                clearedLineCount = 0,
                clearedBaseScore = execResult.clearedBaseScore
            });

            AfterSuccessfulPlay(x, y);
        }
        
        // 不成功：继续保持瞄准，让玩家再拖一次
    }
    
    private void AfterSuccessfulPlay(int x, int y)
    {
        if (currentPlayContext != null && currentPlayContext.HasSpecialEffect)
        {
            ExecuteSpecialEffects(currentPlayContext, new Vector2Int(x, y));
        }
        
        GameEvents.RaiseCardPlayed(new GameEvents.CardPlayedEvent
        {
            pattern = currentPlayContext != null ? currentPlayContext.basePattern : CardPattern.None,
            activeShape = currentPlayContext != null ? currentPlayContext.activePattern : null,
            normalCardCount = currentPlayContext != null ? currentPlayContext.normalCards.Count : 0,
            specialCardCount = currentPlayContext != null ? currentPlayContext.specialCards.Count : 0
        });

        ConsumeSelectedCards();
        RefillToHandSize();

        aiming = false;
        activePattern = null;
        aimMode = AimMode.None;
        currentPlayContext = null;

        RefreshPlayButtonUI();
    }
    
    private void ConsumeSelectedCards()
    {
        // 先复制一份，避免遍历时修改集合
        var toRemove = new List<CardView>(selectedCards);

        foreach (var cv in toRemove)
        {
            cv.OnClicked -= HandleCardClicked;
            cards.Remove(cv);
            Destroy(cv.gameObject);
        }

        selectedCards.Clear();
    }
    
    private void RefreshPlayButtonUI()
    {
        if (playBtnText == null) return;
        
        // 先不做根据牌型改变提示文字
        /*string patternText = currentPattern switch
        {
            CardPattern.None => "Play",
            CardPattern.Single => "Play (Single)",
            CardPattern.Pair => "Play (Pair)",
            CardPattern.Triple => "Play (Triple)",
            CardPattern.Quad => "Play (Quad)",
            CardPattern.Rainbow => "Play (4x4)",
            _ => "Play"
        };*/

        playBtnText.text = aiming ? "Cancel" : "Play";

        playButton.interactable = aiming || selectedCards.Count > 0;

        bool needLineOption = aiming && (currentPattern == CardPattern.Pair || currentPattern == CardPattern.Triple);

        if (lineModeButton != null)
        {
            lineModeButton.gameObject.SetActive(needLineOption);
        }
    }
    
    private void RefreshLineModeUI()
    {
        if (lineModeText != null)
            lineModeText.text = isRowMode ? "—" : "|";
    }
}