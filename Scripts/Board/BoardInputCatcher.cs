using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoardInputCatcher : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private BoardManager board;

    // 缓存，避免拖动时同一格重复刷预览
    private Vector2Int lastCell = new Vector2Int(-999, -999);
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (TryGetCellUnderPointer(eventData, out var cell))
        {
            lastCell = cell;
            board.RaisePointerDown(cell.x, cell.y);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (TryGetCellUnderPointer(eventData, out var cell))
        {
            if (cell != lastCell)
            {
                lastCell = cell;
                board.RaiseDragOver(cell.x, cell.y);
            }
        }
        //Debug.Log("Dragging...");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (TryGetCellUnderPointer(eventData, out var cell))
        {
            board.RaisePointerUp(cell.x, cell.y);
        }
        else
        {
            // 松开在棋盘外：取消本次选择，清掉预览高亮
            board.ClearAllHighlights();

            // 可选：通知 HandManager “这次Up无效”
            board.RaisePointerUp(-1, -1);
        }

        lastCell = new Vector2Int(-999, -999);
    }

    private bool TryGetCellUnderPointer(PointerEventData eventData, out Vector2Int cell)
    {
        cell = default;

        if (EventSystem.current == null) return false;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        for (int i = 0; i < results.Count; i++)
        {
            var go = results[i].gameObject;

            // 跳过自己（Overlay）
            if (go == gameObject) continue;

            var cellView = go.GetComponentInParent<CellView>();
            if (cellView != null)
            {
                cell = new Vector2Int(cellView.X, cellView.Y);
                return true;
            }
        }

        return false;
    }
}