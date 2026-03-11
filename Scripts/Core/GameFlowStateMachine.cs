using System;
using UnityEngine;

public class GameFlowStateMachine : MonoBehaviour
{
    [SerializeField] private HandManager handManager;
    [SerializeField] private BoardManager boardManager;
    private MatchDataCenter MDC => MatchDataCenter.Instance;
    private AccountDataCenter ADC => AccountDataCenter.Instance;
    
    public static GameFlowStateMachine Instance { get; private set; }

    public GameFlowState CurrentState { get; private set; } = GameFlowState.None;
    public GameFlowState PreviousState { get; private set; } = GameFlowState.None;

    public static event Action<GameFlowState, GameFlowState> OnStateChanged;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        if (handManager == null)
            handManager = FindFirstObjectByType<HandManager>();

        if (boardManager == null)
            boardManager = FindFirstObjectByType<BoardManager>();

        if (handManager == null || boardManager == null)
        {
            Debug.LogError("[GameFlowStateMachine] Missing HandManager or BoardManager in scene.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
            return;
        }
    }

    private void Start()
    {
        RequestBackMainMenu();
    }

    private void OnEnable()
    {
        GameEvents.GameOver += OnGameOver;
    }

    private void OnDisable()
    {
        GameEvents.GameOver -= OnGameOver;
    }

    private void OnGameOver(GameOverEvent e)
    {
        ChangeState(GameFlowState.GameOver);
    }

    public void ChangeState(GameFlowState newState)
    {
        if (CurrentState == newState)
            return;

        var oldState = CurrentState;
        ExitState(oldState);

        PreviousState = oldState;
        CurrentState = newState;

        EnterState(newState);

        OnStateChanged?.Invoke(oldState, newState);
    }

    private void EnterState(GameFlowState state)
    {
        switch (state)
        {
            case GameFlowState.MainMenu:
            {
                EnterMainMenu();
                break;
            }
            case GameFlowState.Prepare:
            {
                EnterPrepare();
                break;
            }
            case GameFlowState.GamePlay:
            {
                EnterGamePlay();
                break;
            }
            case GameFlowState.GameOver:
            {
                EnterGameOver();
                break;
            }
        }
    }
    
    /*
    boardManager.BuildBoard();          //生成
    boardManager.ClearAllBlocks();      //清理所有方块
    boardManager.ClearAllHighlights();  //清理所有高亮状态
    boardManager.DestroyAllCells();     //销毁所有格子槽
    
    handManager.ResetHand();            //重置手牌
    handManager.ClearHand();            //清理手牌
    
    MDC.StartNewMatch();                //new一份局内数据
    */
    
    private void EnterMainMenu()
    {
        boardManager.DestroyAllCells();
    }

    private void EnterPrepare()
    {
        MDC.CreatNewMatchData();
        MDC.ClearCarriedItems();
        
        boardManager.BuildBoard();
        boardManager.ClearAllBlocks();
    }
    
    private void EnterGamePlay()
    {
        MDC.StartMatch();
        boardManager.BuildBoard();
        handManager.ResetHand();
        boardManager.StartSpawning();
        boardManager.ClearAllHighlights();
    }
    
    private void EnterGameOver()
    {
        boardManager.StopSpawning();
    }

    private void ExitState(GameFlowState state)
    {
        switch (state)
        {
            case GameFlowState.MainMenu:
            {
                ExitMainMenu();
                break;
            }
            case GameFlowState.Prepare:
            {
                ExitPrepare();
                break;
            }
            case GameFlowState.GamePlay:
            {
                ExitGamePlay();
                break;
            }
            case GameFlowState.GameOver:
            {
                ExitGameOver();
                break;
            }
        }
    }

    private void ExitMainMenu()
    {
        
    }

    private void ExitPrepare()
    {
        
    }

    private void ExitGamePlay()
    {
        MDC.ResumeMatch();
        boardManager.ClearAllHighlights();
        boardManager.StopSpawning();
    }
    
    private void ExitGameOver()
    {
        boardManager.ClearAllBlocks();
        handManager.ResetHand();
    }

    public bool IsInState(GameFlowState state) => CurrentState == state;
    
    public void RequestPrepareGame() => ChangeState(GameFlowState.Prepare);
    public void RequestStartGame()
    {
        bool energyEnough = AccountDataCenter.Instance.CostEnergy(1);
        if (!energyEnough)
        {
            Debug.Log("体力不足，无法开始新局。");
            return;
        }
        
        /*if (!TryConsumePreparedConsumables())
        {
            Debug.Log("道具库存不足，无法开始新局。");
            ADC.AddEnergy(1); // 把刚扣的体力退回
            return;
        }*/ 
        
        ChangeState(GameFlowState.GamePlay);
    }
    
    /*private bool TryConsumePreparedConsumables()
    {
        if (MDC.CurrentMatch == null) return false;

        var slots = MDC.CurrentMatch.equippedConsumables;
        if (slots == null || slots.Count == 0) return true;

        // 先统计每种道具要扣几次
        Dictionary<string, int> costMap = new();

        foreach (var slot in slots)
        {
            if (slot == null || string.IsNullOrEmpty(slot.itemId))
                continue;

            if (!costMap.ContainsKey(slot.itemId))
                costMap[slot.itemId] = 0;

            costMap[slot.itemId]++;
        }

        // 先检查库存够不够
        foreach (var pair in costMap)
        {
            if (ADC.GetConsumableCount(pair.Key) < pair.Value)
                return false;
        }

        // 再正式扣
        foreach (var pair in costMap)
        {
            bool success = ADC.CostConsumable(pair.Key, pair.Value);
            if (!success)
                return false;
        }

        return true;
    }*/
    
    public void RequestBackMainMenu() => ChangeState(GameFlowState.MainMenu);

    #region 暂停游戏

    public void PauseGame()
    {
        if (CurrentState != GameFlowState.GamePlay) return;
        if (MDC.CurrentMatch == null) return;
        if (MDC.CurrentMatch.isPausing) return;

        MDC.PauseMatch();
        boardManager.StopSpawning();
    }

    public void ResumeGame()
    {
        if (CurrentState != GameFlowState.GamePlay) return;
        if (MDC.CurrentMatch == null) return;
        if (!MDC.CurrentMatch.isPausing) return;

        MDC.ResumeMatch();
        boardManager.StartSpawning();
    }

    #endregion
}