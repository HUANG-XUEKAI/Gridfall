using System.Collections.Generic;
using UnityEngine;

public class ItemDisplay : MonoBehaviour
{
    [Header("Carry Slots")]
    [SerializeField] private ItemSlot[] carrySlotsInPrepare = new ItemSlot[MatchData.MaxCarriedCount];
    [SerializeField] private ItemSlot[] carrySlotsInGame = new ItemSlot[MatchData.MaxCarriedCount];

    [Header("Inventory Pages")]
    [SerializeField] private Transform page1Root;
    [SerializeField] private Transform page2Root;
    [SerializeField] private Transform page3Root;

    [Header("Prefabs")]
    [SerializeField] private ItemSlot itemSlotPrefab;
    
    private BasicItem[] carriedForDisplay = new BasicItem[MatchData.MaxCarriedCount];
    private List<BasicItem> inventoryForDisplay = new();
    
    private readonly List<ItemSlot> spawnedSlots = new();
    private AccountDataCenter ADC => AccountDataCenter.Instance;
    private MatchDataCenter MDC => MatchDataCenter.Instance;
    
    private void OnValidate()
    {
        CorrectLength(carrySlotsInPrepare);
        CorrectLength(carrySlotsInGame);
    }

    void CorrectLength(ItemSlot[] slots)
    {
        // 写一个自动校正数组长度为MatchData.MaxCarriedCount的方法
    }
    
    private void OnEnable()
    {
        // 订阅
        // GameEvents.InventoryItemsChanged
        // GameEvents.CarriedItemsChanged
        // GameFlowStateMachine.OnStateChanged
    }

    private void OnDisable()
    {
        // 取消订阅
        // GameEvents.InventoryItemsChanged
        // GameEvents.CarriedItemsChanged
        // GameFlowStateMachine.OnStateChanged
    }

    private void BuildInventoryDisplay(GameFlowState oldState, GameFlowState newState)
    {
        // Prepare状态：
        // 清理掉 carriedForDisplay 和 inventoryForDisplay
        // 拉取当前账号库存数据，复制一份给 inventoryForDisplay 用于显示
        // 生成对应数量的 itemSlotPrefab 到 pageRoot 下面
        // 将 Item 绑定至对应的 itemSlot
        // 绑定 spawnedSlots 里槽位按钮的点击事件，和 carrySlotsInPrepare 里槽位按钮的点击事件
        
        // GamePlay状态：
        
        // Shopping状态：
    }

    public void TakeThisItem()
    {
        // 绑定在 spawnedSlots 的槽位按钮上，点击后弹一个到 carriedForDisplay 里
        // 如果该槽位上的 Item 数量为 0，则把该槽位设为空槽位
        // 刷新显示
        // 注意条件：carriedForDisplay 里还有空位
    }

    public void PutThisItem()
    {
        // 绑定在 carrySlotsInPrepare 的槽位按钮上，点击后弹回到 inventoryForDisplay 里
        // 刷新显示
    }

    public void ApplyCarried()
    {
        MDC.ResetCarriedItems(carriedForDisplay);
        ADC.ResetInventory(inventoryForDisplay);
    }

    public void UpdateDisplay()
    {
        // 刷新槽位显示（图标、数量）
    }
}