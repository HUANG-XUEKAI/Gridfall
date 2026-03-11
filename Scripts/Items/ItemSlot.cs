using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    public ConsumableDefinition Item { get; private set; }
    public int Count { get; private set; }

    public Button Button => button;

    public void Bind(ConsumableDefinition item, int count)
    {
        Item = item;
        Count = count;
        RefreshView();
    }

    public void SetEmpty()
    {
        Item = null;
        Count = 0;
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

        if (countText != null)
        {
            countText.text = hasItem ? Count.ToString() : "";
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