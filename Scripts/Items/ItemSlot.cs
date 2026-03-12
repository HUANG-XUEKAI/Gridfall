using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private bool showQuantity = true;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private TextMeshProUGUI priceText;

    public BasicItem Item { get; private set; }
    public event Action<ItemSlot> OnSlotClicked;

    private void Awake()
    {
        if (button != null)
            button.onClick.AddListener(HandleClick);
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(HandleClick);
    }

    private void HandleClick()
    {
        if (Item == null) return;
        OnSlotClicked?.Invoke(this);
    }

    public void Bind(BasicItem item)
    {
        Item = item;
        RefreshView();
    }

    public void SetEmpty()
    {
        Item = null;
        RefreshView();
    }

    public void RefreshView()
    {
        if (Item != null && Item.quantity <= 0)
            Item = null;

        bool hasItem = Item != null;

        if (iconImage != null)
        {
            iconImage.enabled = hasItem;
            iconImage.sprite = hasItem ? Item.icon : null;
        }

        if (quantityText != null)
        {
            if (showQuantity && hasItem)
                quantityText.text = Item.quantity.ToString();
            else
                quantityText.text = "";
        }

        if (priceText != null)
        {
            if (hasItem)
                priceText.text = Item.price.ToString();
            else
                quantityText.text = "";
        }

        if (button != null)
            button.interactable = hasItem;
    }

    public bool IsEmpty()
    {
        return Item == null;
    }
}