using System;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Size")]
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;

    [Header("Refs")]
    [SerializeField] private Transform boardRoot; // GridLayoutGroup 挂这
    [SerializeField] private CellView cellPrefab;
    [SerializeField] private BoardInputCatcher inputCatcher;

    [Header("Config")]
    [SerializeField] private BlockDatabase blockDB;

    private BasicBlock[,] blocks;
    private CellView[,] views;

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
        blockDB.InitializeNormalBlocks();
    }
    
    public void BuildBoard()
    {
        if (views != null && blocks != null)
            return;
        
        blocks = new BasicBlock[width, height];
        views = new CellView[width, height];
        
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            var cell = Instantiate(cellPrefab, boardRoot);
            cell.Init(x, y);
            
            views[x, y] = cell;
            blocks[x, y] = null;
        }
        
        inputCatcher.transform.SetAsLastSibling();
    }

    public bool IsEmpty(int x, int y) => blocks[x, y] == null;

    public BasicBlock Get(int x, int y) => blocks[x, y];

    public bool TrySpawn(int x, int y, BasicBlock block)
    {
        if (!IsEmpty(x, y) || block == null) 
            return false;
        blocks[x, y] = block;
        views[x, y].SetBlock(block);
        return true;
    }

    public void Clear(int x, int y)
    {
        blocks[x, y] = null;
        views[x, y].SetEmpty();
    }
    
    public void ClearAllShapes()
    {
        if (blocks == null || views == null) return;

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
            Clear(x, y); // 你已有 Clear(x,y) 会同步把 view 清空
    }
    
    public void DestroyAllCells()
    {
        if (views != null)
        {
            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                if (views[x, y] != null)
                    Destroy(views[x, y].gameObject);
            }
        }

        // 清空数据引用，表示“棋盘未构建”
        views = null;
        blocks = null;
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
        if (views == null) return;
        
        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
            views[x, y].SetHighlighted(false);
    }

    public void HighlightCells(IEnumerable<Vector2Int> cells)
    {
        ClearAllHighlights();
        foreach (var p in cells)
            views[p.x, p.y].SetHighlighted(true);
    }
}