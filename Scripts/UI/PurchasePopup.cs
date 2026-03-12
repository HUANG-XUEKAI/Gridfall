using UnityEngine;
using UnityEngine.UI;

public class PurchasePopup : ConfirmationPopup
{
    public BasicItem item;
    public Image itemImage;
    public int buyAmount = 1;
    
    [SerializeField] private Text itemName_text;
    [SerializeField] private Text buyAmount_text;
    [SerializeField] private Text itemPrice_text;
    [SerializeField] private Text requiredGold_text;
    
    [SerializeField] private Button addButton;
    [SerializeField] private Button minusButton;
    
    // 绑定 confirmButton 和 cancelButton
}
