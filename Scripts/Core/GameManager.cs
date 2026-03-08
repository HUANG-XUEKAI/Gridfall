using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Spawn Rule")]
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private int spawnPerTick = 4;
    
    [Header("HP")]
    [SerializeField] private int defaultHP = 5;

    [Header("Refs")]
    [SerializeField] private CanvasGroup mainMenu;
    [SerializeField] private CanvasGroup gameInterface;
    [SerializeField] private CanvasGroup gameOverPanel;
    
    [SerializeField] private HandManager handManager;
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private ScoreSystem scoreSystem;
    
    private int hp;
    private bool gameOver;
    
    private Coroutine spawnRoutine;

    void Awake()
    {
        InitializeUI();
        ResetGame();
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
        ResetGame();
        
        boardManager.BuildBoard();
        boardManager.ClearAllShapes();
        boardManager.ClearAllHighlights();
        
        handManager.ResetHand();
        
        scoreSystem.ResetScore();
        
        HideGUI(mainMenu);
        HideGUI(gameOverPanel);
        ShowGUI(gameInterface);
        
        StartSpawning();
    }
    
    public void ResetGame()
    {
        hp = defaultHP;
        gameOver = false;
        
        GameEvents.RaiseHPChanged(new HPChangedEvent
        {
            currentHP = hp,
            maxHP = defaultHP,
            delta = 0
        });
    }

    public void BackMainMenu()
    {
        StopSpawning();
        
        boardManager.ClearAllHighlights();
        boardManager.DestroyAllCells();
        handManager.ClearHand();
        
        ResetGame();
        
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

        while (!gameOver)
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

                TakeDamage(damage);

                if (gameOver)
                    yield break;
            }
        }
    }
    
    private void GameOver()
    {
        gameOver = true;
        ShowGUI(gameOverPanel);
        StopSpawning();

        GameEvents.RaiseGameOver(new GameOverEvent
        {
            finalHP = hp
        });

        Debug.Log("Game Over!");
    }

    #region 特殊效果
    public void AddHP(int amount)
    {
        if (amount <= 0) return;
        
        int oldHP = hp;
        hp = Mathf.Min(defaultHP, hp + amount);

        GameEvents.RaiseHPChanged(new HPChangedEvent
        {
            currentHP = hp,
            maxHP = defaultHP,
            delta = hp - oldHP
        });
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || gameOver) return;

        int oldHP = hp;
        hp -= amount;

        if (hp <= 0)
        {
            hp = 0;

            GameEvents.RaiseHPChanged(new HPChangedEvent
            {
                currentHP = hp,
                maxHP = defaultHP,
                delta = hp - oldHP
            });

            GameOver();
            return;
        }

        GameEvents.RaiseHPChanged(new HPChangedEvent
        {
            currentHP = hp,
            maxHP = defaultHP,
            delta = hp - oldHP
        });
    }
    #endregion

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