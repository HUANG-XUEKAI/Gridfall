using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private bool showQuantity = true;
    [SerializeField] private TextMeshProUGUI quantityText;

    public BasicItem Item { get; private set; }
    public Button Button => button;

    public void Bind(BasicItem item, int count)
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
        bool hasItem = Item != null;

        if (iconImage != null)
        {
            iconImage.enabled = hasItem;
            iconImage.sprite = hasItem ? Item.icon : null;
        }

        if (quantityText != null && showQuantity)
        {
            quantityText.text = hasItem ? Item.quantity.ToString() : "";
        }

        if (button != null)
        {
            button.interactable = hasItem;
        }
    }

    public void SetClick(System.Action onClick)
    {
        if (button == null) return;

        button.onClick.RemoveAllListeners();
        if (onClick != null)
            button.onClick.AddListener(() => onClick());
    }
}