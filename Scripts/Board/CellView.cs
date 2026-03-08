using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class CellView : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private Image icon;

    // 新增：用于高亮的背景（最简单：直接用本物体的 Image 当底色）
    [SerializeField] private Image background;

    public int X { get; private set; }
    public int Y { get; private set; }

    public event Action<int,int> OnPointerDownCell;
    public event Action<int,int> OnDragOverCell;
    public event Action<int,int> OnPointerUpCell;

    public void Init(int x, int y)
    {
        X = x; Y = y;
        SetEmpty();
        SetHighlighted(false);
    }

    public void SetEmpty()
    {
        icon.enabled = false;
        icon.sprite = null;
    }

    public void SetBlock(BasicBlock block)
    {
        icon.enabled = true;
        //BasicPattern p = 
        icon.sprite = block != null ? block.pattern.sprite : null;
    }

    public void SetHighlighted(bool on)
    {
        if (background == null) return;
        // 先别玩花活：高亮时变浅色；你可以在 Inspector 里配两种颜色更灵活
        background.color = on ? new Color(1f, 1f, 1f, 0.6f) : Color.white;
    }

    public void OnPointerDown(PointerEventData eventData)
        => OnPointerDownCell?.Invoke(X, Y);

    public void OnDrag(PointerEventData eventData)
        => OnDragOverCell?.Invoke(X, Y);

    public void OnPointerUp(PointerEventData eventData)
        => OnPointerUpCell?.Invoke(X, Y);
}