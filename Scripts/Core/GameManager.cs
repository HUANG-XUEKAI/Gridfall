using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Spawn Rule")]
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private int spawnPerTick = 4;
    
    //[SerializeField] private int defaultHP = 5;

    [Header("Refs")]
    [SerializeField] private CanvasGroup mainMenu;
    [SerializeField] private CanvasGroup gameInterface;
    [SerializeField] private CanvasGroup gameOverPanel;
    
    [SerializeField] private HandManager handManager;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private ScoreSystem scoreSystem;
    
    private MatchDataCenter MDC => MatchDataCenter.Instance;
    private Coroutine spawnRoutine;

    void Awake()
    {
        InitializeUI();
        //ResetGame();
        StopSpawning();
    }

    void InitializeUI()
    {
        ShowGUI(mainMenu);
        HideGUI(gameInterface);
        HideGUI(gameOverPanel);
    }

    public void StartGame()
    {
        MDC.StartNewMatch();
        
        boardManager.BuildBoard();
        boardManager.ClearAllShapes();
        boardManager.ClearAllHighlights();
        
        handManager.ResetHand();
        
        HideGUI(mainMenu);
        HideGUI(gameOverPanel);
        ShowGUI(gameInterface);
        
        StartSpawning();
    }
    
    //逐步弱化，最后删掉。
    /*public void ResetGame() 
    {
        hp = defaultHP;
        gameOver = false;
        
        GameEvents.RaiseHPChanged(new HPChangedEvent
        {
            currentHP = hp,
            maxHP = defaultHP,
            delta = 0
        });
    }*/

    public void BackMainMenu()
    {
        StopSpawning();
        
        boardManager.ClearAllHighlights();
        boardManager.DestroyAllCells();
        handManager.ClearHand();
        
        //ResetGame();
        
        ShowGUI(mainMenu);
        HideGUI(gameInterface);
        HideGUI(gameOverPanel);
    }
    
    public void StartSpawning()
    {
        if (spawnRoutine != null) 
            return; // 已在跑
        spawnRoutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        var wait = new WaitForSeconds(spawnInterval);

        while (MDC.CurrentMatch.isGaming)
        {
            yield return wait;

            boardManager.SpawnRandom(spawnPerTick);

            int damage = boardManager.ResolveFullLinesAndClear();
            if (damage > 0)
            {
                GameEvents.RaiseBoardResolved(new BoardResolvedEvent
                {
                    pattern = CardPattern.None,
                    clearedCellCount = 0,
                    clearedLineCount = damage
                });

                MDC.TakeDamage(damage);

                if (!MDC.CurrentMatch.isGaming)
                    yield break;
            }
        }
    }
    
    public static void ShowGUI(CanvasGroup gui)
    {
        if (gui != null && !IsVisible(gui))
        {
            gui.alpha = 1;
            gui.blocksRaycasts = true;
            gui.interactable = true;
        }
    }
    
    public static void HideGUI(CanvasGroup gui)
    {
        if (gui != null && IsVisible(gui))
        {
            gui.alpha = 0;
            gui.blocksRaycasts = false;
            gui.interactable = false;
        }
    }
    
    public static bool IsVisible(CanvasGroup gui)
    {
        return (gui.alpha > 0.0001f);
    }
}