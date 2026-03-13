using UnityEngine;
using UnityEngine.UI;
using System;

public class CardView : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject selectedFrame;
    [SerializeField] private Button button;
    
    public BasicCard Card { get; private set; }
    
    [Header("Selection Visual")]
    [SerializeField] private RectTransform visualRoot; // ★新增：只动它！
    [SerializeField] private float selectedYOffset = 20f;
    private Vector2 visualOriginalPos;
    private bool isSelected;
    
    public event Action<CardView> OnClicked;
    
    private void Awake()
    {
        if (visualRoot != null)
            visualOriginalPos = visualRoot.anchoredPosition;

        if (button != null)
            button.onClick.AddListener(NotifyClicked);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(NotifyClicked);
    }

    private void NotifyClicked()
    {
        OnClicked?.Invoke(this);
    }
    
    public void Init(BasicCard card)
    {
        if (card == null)
        {
            Debug.LogWarning("Card is null");
            return;
        }
        
        if (iconImage == null)
        {
            Debug.LogError("Icon Image is null!");
            return;
        }
        
        Card = card;
        iconImage.enabled = true;
        iconImage.sprite = card.pattern.sprite;
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        if (selectedFrame) 
            selectedFrame.SetActive(selected);
        
        ApplyOffset();
    }

    private void ApplyOffset()
    {
        if (visualRoot == null) return;
        visualRoot.anchoredPosition = isSelected
            ? visualOriginalPos + Vector2.up * selectedYOffset
            : visualOriginalPos;
    }
}