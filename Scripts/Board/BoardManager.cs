using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { get; private set; }
    
    [Header("Size")]
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;
    
    [Header("Spawn Rule")]
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private int spawnPerTick = 4;

    [Header("Refs")]
    [SerializeField] private Transform boardRoot; // GridLayoutGroup 挂这
    [SerializeField] private CellView cellPrefab;
    [SerializeField] private BoardInputCatcher inputCatcher;

    [Header("Config")]
    [SerializeField] private BlockDatabase blockDB;

    private BasicBlock[,] blocks;
    private CellView[,] cells;
    private Coroutine spawnRoutine;
    private MatchDataCenter MDC => MatchDataCenter.Instance;

    public int Width => width;
    public int Height => height;
    
    public event Action<int,int> OnCellPointerDown;
    public event Action<int,int> OnCellDragOver;
    public event Action<int,int> OnCellPointerUp;
    
    public void RaisePointerDown(int x, int y) => OnCellPointerDown?.Invoke(x, y);
    public void RaiseDragOver(int x, int y) => OnCellDragOver?.Invoke(x, y);
    public void RaisePointerUp(int x, int y) => OnCellPointerUp?.Invoke(x, y);

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
        blockDB.InitializeNormalBlocks();
    }
    
    public void BuildBoard()
    {
        if (cells != null && blocks != null)
            return;
        
        blocks = new BasicBlock[width, height];
        cells = new CellView[width, height];
        
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            var cell = Instantiate(cellPrefab, boardRoot);
            cell.Init(x, y);
            
            cells[x, y] = cell;
            blocks[x, y] = null;
        }
        
        inputCatcher.transform.SetAsLastSibling();
    }
    
    public void DestroyAllCells()
    {
        if (cells != null)
        {
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                if (cells[x, y] != null)
                    Destroy(cells[x, y].gameObject);
            }
        }

        // 清空数据引用，表示“棋盘未构建”
        cells = null;
        blocks = null;
    }

    public bool IsEmpty(int x, int y) => blocks[x, y] == null;

    public BasicBlock Get(int x, int y) => blocks[x, y];

    public bool TrySpawn(int x, int y, BasicBlock block)
    {
        if (!IsEmpty(x, y) || block == null) 
            return false;
        blocks[x, y] = block;
        cells[x, y].SetBlock(block);
        return true;
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

            SpawnRandom(spawnPerTick);

            int damage = ResolveFullLinesAndClear();
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

    public void Clear(int x, int y)
    {
        blocks[x, y] = null;
        cells[x, y].SetEmpty();
    }
    
    public void ClearAllBlocks()
    {
        if (blocks == null || cells == null) return;

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
            Clear(x, y); // 你已有 Clear(x,y) 会同步把 view 清空
    }

    public List<Vector2Int> GetAllEmptyCells()
    {
        var list = new List<Vector2Int>();
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
            if (blocks[x, y] == null)
                list.Add(new Vector2Int(x, y));
        return list;
    }

    public void SpawnRandom(int count)
    {
        var empties = GetAllEmptyCells();
        if (empties.Count == 0) return;

        int spawnCount = Mathf.Min(count, empties.Count);

        for (int i = 0; i < spawnCount; i++)
        {
            int idx = UnityEngine.Random.Range(0, empties.Count);
            var pos = empties[idx];
            empties.RemoveAt(idx);

            var block = blockDB.GetNormalBlockRandom();
            TrySpawn(pos.x, pos.y, block);
        }
        
        // TODO : 特殊块的生成
    }

    // —— 行列检测：返回本次满的行数/列数，并把这些线上的格子全部清空
    public int ResolveFullLinesAndClear()
    {
        var fullRows = new List<int>();
        var fullCols = new List<int>();

        // Rows
        for (int y = 0; y < height; y++)
        {
            bool full = true;
            for (int x = 0; x < width; x++)
            {
                if (blocks[x, y] == null) { full = false; break; }
            }
            if (full) fullRows.Add(y);
        }

        // Cols
        for (int x = 0; x < width; x++)
        {
            bool full = true;
            for (int y = 0; y < height; y++)
            {
                if (blocks[x, y] == null) { full = false; break; }
            }
            if (full) fullCols.Add(x);
        }

        if (fullRows.Count == 0 && fullCols.Count == 0) return 0;

        // 同时清空：用 HashSet 避免交叉重复清
        var toClear = new HashSet<Vector2Int>();
        foreach (var y in fullRows)
            for (int x = 0; x < width; x++)
                toClear.Add(new Vector2Int(x, y));

        foreach (var x in fullCols)
            for (int y = 0; y < height; y++)
                toClear.Add(new Vector2Int(x, y));

        foreach (var p in toClear)
            Clear(p.x, p.y);

        // 扣血按“线条数”叠加：行数 + 列数
        return fullRows.Count + fullCols.Count;
    }
    
    public Vector2Int ClearAllMatching(BasicPattern pattern)
    {
        if (pattern == null) return new Vector2Int(0, 0);
        
        int cleared = 0;
        int clearedBaseScore = 0;
        
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            var block = blocks[x, y];
            if (block != null && block.pattern == pattern)
            {
                clearedBaseScore += block.baseScore;
                Clear(x, y);
                cleared++;
            }
        }
        return new Vector2Int(cleared, clearedBaseScore);
    }
    
    public void ClearAllHighlights()
    {
        if (cells == null) return;
        
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
            cells[x, y].SetHighlighted(false);
    }

    public void HighlightCells(IEnumerable<Vector2Int> cells)
    {
        ClearAllHighlights();
        foreach (var p in cells)
            this.cells[p.x, p.y].SetHighlighted(true);
    }

    #region Effect
    public bool TryBombRandom3x3()
    {
        if (blocks == null || cells == null)
            return false;

        List<Vector2Int> occupiedCells = GetAllOccupiedCells();
        if (occupiedCells.Count == 0)
            return false;

        int randomIndex = UnityEngine.Random.Range(0, occupiedCells.Count);
        Vector2Int center = occupiedCells[randomIndex];

        int clearedCount = ClearArea(center.x - 1, center.x + 1, center.y - 1, center.y + 1);
        return clearedCount > 0;
    }

    private List<Vector2Int> GetAllOccupiedCells()
    {
        List<Vector2Int> list = new();

        if (blocks == null)
            return list;

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            if (blocks[x, y] != null)
                list.Add(new Vector2Int(x, y));
        }

        return list;
    }

    private int ClearArea(int minX, int maxX, int minY, int maxY)
    {
        int clearedCount = 0;

        int clampedMinX = Mathf.Clamp(minX, 0, width - 1);
        int clampedMaxX = Mathf.Clamp(maxX, 0, width - 1);
        int clampedMinY = Mathf.Clamp(minY, 0, height - 1);
        int clampedMaxY = Mathf.Clamp(maxY, 0, height - 1);

        for (int y = clampedMinY; y <= clampedMaxY; y++)
        for (int x = clampedMinX; x <= clampedMaxX; x++)
        {
            if (blocks[x, y] == null)
                continue;

            Clear(x, y);
            clearedCount++;
        }

        return clearedCount;
    }
    #endregion
}