using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PurchasePopup : ConfirmationPopup
{
    [NonSerialized] public BasicItem item;
    int buyAmount = 1;
    
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName_text;
    [SerializeField] private TextMeshProUGUI buyAmount_text;
    [SerializeField] private TextMeshProUGUI itemPrice_text;
    [SerializeField] private TextMeshProUGUI requiredGold_text;
    
    [SerializeField] private Button addButton;
    [SerializeField] private Button minusButton;
    
    private void Awake()
    {
        if (addButton != null)
            addButton.onClick.AddListener(AddAmount);

        if (minusButton != null)
            minusButton.onClick.AddListener(MinusAmount);
    }
    
    private void OnDestroy()
    {
        if (addButton != null)
            addButton.onClick.RemoveListener(AddAmount);

        if (minusButton != null)
            minusButton.onClick.RemoveListener(MinusAmount);
    }
    
    public void Set(BasicItem targetItem)
    {
        item = targetItem;
        if (itemImage != null) itemImage.sprite = item != null ? item.icon : null;
        if (itemName_text != null) itemName_text.text = item != null ? item.displayName : "";
        if (itemPrice_text != null) itemPrice_text.text = item != null ? item.price.ToString() : "0";
        
        BindConfirmAction(ConfirmBuy);
        BindCancelAction(()=>Destroy(gameObject));
        RefreshView();
    }
    
    private void ConfirmBuy()
    {
        if (item == null) return;
        
        AccountDataCenter adc = AccountDataCenter.Instance;
        
        int needAmount = item.price * buyAmount;
        if (adc.CostGold(needAmount))
        {
            if (!adc.AddItem(item, buyAmount))
            {
                adc.EarnGold(needAmount);
            }
        }
        
        Destroy(gameObject);
    }
    
    private void AddAmount()
    {
        buyAmount++;
        RefreshView();
    }

    private void MinusAmount()
    {
        int result = buyAmount - 1;
        if (result <= 0) return;
        buyAmount = result;
        RefreshView();
    }
    
    private void RefreshView()
    {
        if (buyAmount_text != null)
            buyAmount_text.text = buyAmount.ToString();

        if (requiredGold_text != null)
        {
            int totalPrice = item != null ? item.price * buyAmount : 0;
            requiredGold_text.text = totalPrice.ToString();
        }
        
        if (minusButton != null)
            minusButton.interactable = (buyAmount > 1);
    }
}
