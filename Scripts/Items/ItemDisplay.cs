using System.Collections.Generic;
using UnityEngine;

public class ItemDisplay : MonoBehaviour
{
    [Header("Prepare")]
    [SerializeField] private ItemSlot[] carrySlotsInPrepare = new ItemSlot[MatchData.MaxCarriedCount];
    
    // 先只做一页
    [SerializeField] private ItemSlot[] inventorySlotsInPrepare1 = new ItemSlot[9];
    [SerializeField] private ItemSlot[] inventorySlotsInPrepare2 = new ItemSlot[9];
    [SerializeField] private ItemSlot[] inventorySlotsInPrepare3 = new ItemSlot[9];
    [SerializeField] private CanvasGroup page1Root;
    [SerializeField] private CanvasGroup page2Root;
    [SerializeField] private CanvasGroup page3Root;
    
    private List<BasicItem> inventoryForDisplay = new();
    //private BasicItem[] carriedForDisplay = new BasicItem[MatchData.MaxCarriedCount];
    
    [Header("Gameplay")]
    [SerializeField] private ItemSlot[] carrySlotsInGame = new ItemSlot[MatchData.MaxCarriedCount]; //先不管
    
    [Header("Shopping")]
    
    [Header("Prefabs")]
    [SerializeField] private ItemSlot itemSlotPrefab;
    
    private AccountDataCenter ADC => AccountDataCenter.Instance;
    private MatchDataCenter MDC => MatchDataCenter.Instance;

    // 保证配置界面不会改变长度
    private void OnValidate()
    {
        carrySlotsInPrepare = CorrectLength(carrySlotsInPrepare);
        carrySlotsInGame = CorrectLength(carrySlotsInGame);
    }
    
    private ItemSlot[] CorrectLength(ItemSlot[] slots)
    {
        int targetLength = MatchData.MaxCarriedCount;

        if (slots == null)
            return new ItemSlot[targetLength];

        if (slots.Length == targetLength)
            return slots;

        ItemSlot[] corrected = new ItemSlot[targetLength];
        int copyLength = Mathf.Min(slots.Length, corrected.Length);

        for (int i = 0; i < copyLength; i++)
            corrected[i] = slots[i];

        return corrected;
    }

    private void OnEnable()
    {
        GameFlowStateMachine.OnStateChanged += BuildInventoryDisplay;
        RegisterPrepareSlotsEvent();
    }

    private void OnDisable()
    {
        GameFlowStateMachine.OnStateChanged -= BuildInventoryDisplay;
        UnregisterPrepareSlotsEvent();
    }
    
    private void BuildInventoryDisplay(GameFlowState oldState, GameFlowState newState)
    {
        switch (newState)
        {
            case GameFlowState.Prepare:
            {
                BuildDisplayData();
                FillInSlotsInPrepare();
                UpdatePrepareSlotsDisplay();
                return;
            }
        }
        // 之后再做其他状态
    }
    
    private void BuildDisplayData()
    {
        // 重置 inventoryForDisplay：从 ADC.Inventory.ownedItems 拷贝一份
        inventoryForDisplay.Clear();

        if (ADC != null &&
            ADC.Inventory != null &&
            ADC.Inventory.ownedItems != null)
        {
            foreach (BasicItem item in ADC.Inventory.ownedItems)
            {
                if (item == null) continue;
                if (string.IsNullOrEmpty(item.itemId)) continue;
                if (item.quantity <= 0) continue;

                inventoryForDisplay.Add(Instantiate(item));
            }
        }
        // 把 carrySlotsInPrepare 两个槽位都 SetEmpty()
        foreach (var slot in carrySlotsInPrepare)
        {
            if (slot == null) continue;
            slot.SetEmpty();
        }
    }
    
    private void FillInSlotsInPrepare()
    {
        // 把 inventoryForDisplay 里的道具 按顺序塞进 inventorySlotsInPrepare1 里，把道具绑定到槽上，并且顺便刷新一下该槽
        // 先只做第一页 inventorySlotsInPrepare1，另外的 inventorySlotsInPrepare2 和 inventorySlotsInPrepare3 先放着不管
        // 如果 inventoryForDisplay 超过了 inventorySlotsInPrepare1 先按不显示处理就好，之后再做多页逻辑
        
        SetPageVisible(page1Root, true);
        SetPageVisible(page2Root, false);
        SetPageVisible(page3Root, false);

        foreach (var slot in inventorySlotsInPrepare1)
        {
            if (slot == null) continue;
            slot.SetEmpty();
        }

        int fillCount = Mathf.Min(inventoryForDisplay.Count, inventorySlotsInPrepare1.Length);

        for (int i = 0; i < fillCount; i++)
        {
            ItemSlot slot = inventorySlotsInPrepare1[i];
            if (slot == null) continue;

            slot.Bind(inventoryForDisplay[i]);
        }
    }
    
    private void RegisterPrepareSlotsEvent()
    {
        foreach (var slot in carrySlotsInPrepare)
        {
            if (slot == null) continue;
            // HandlePrepareSlotClicked 订阅道具槽点击事件
            slot.OnSlotClicked -= HandlePrepareSlotClicked;
            slot.OnSlotClicked += HandlePrepareSlotClicked;
        }
        
        // 只做一页
        foreach (var slot in inventorySlotsInPrepare1)
        {
            if (slot == null) continue;
            // HandlePrepareSlotClicked 订阅道具槽点击事件
            slot.OnSlotClicked -= HandlePrepareSlotClicked;
            slot.OnSlotClicked += HandlePrepareSlotClicked;
        }
    }

    private void UnregisterPrepareSlotsEvent()
    {
        foreach (var slot in carrySlotsInPrepare)
        {
            if (slot == null) continue;
            // 取消 HandlePrepareSlotClicked 订阅道具槽点击事件
            slot.OnSlotClicked -= HandlePrepareSlotClicked;
        }
        
        // 只做一页
        foreach (var slot in inventorySlotsInPrepare1)
        {
            if (slot == null) continue;
            // 取消 HandlePrepareSlotClicked 订阅道具槽点击事件
            slot.OnSlotClicked -= HandlePrepareSlotClicked;
        }
    }
    
    private void HandlePrepareSlotClicked(ItemSlot slot)
    {
        // 如果这个 slot 是来自 inventorySlotsInPrepare1 的 => TakeThisItem(slot.Item)
        if (ContainsSlot(inventorySlotsInPrepare1, slot))
        {
            TakeThisItem(slot.Item);
        }
        // 如果这个 slot 是来自 carrySlotsInPrepare 的 => PutThisItem(slot.Item)
        else if (ContainsSlot(carrySlotsInPrepare, slot))
        {
            PutThisItem(slot.Item);
        }
        
        UpdatePrepareSlotsDisplay();
    }
    
    private void UpdatePrepareSlotsDisplay()
    {
        // 单纯的刷新：
        // 遍历 carrySlotsInPrepare 和 inventorySlotsInPrepare1，每个槽给它 RefreshView() 就好
        foreach (var slot in carrySlotsInPrepare)
        {
            if (slot == null) continue;
            slot.RefreshView();
        }

        foreach (var slot in inventorySlotsInPrepare1)
        {
            if (slot == null) continue;
            slot.RefreshView();
        }
    }
    
    void TakeThisItem(BasicItem item)
    {
        // 响应 inventorySlotsInPrepare1 槽位的点击：
        
        /*
        弹一个到 carrySlotsInPrepare，逻辑如下：
        1.如果这个槽位是空的（slot.Item == null），则不处理本次事件；
        2.如果不是空的，就看下 carrySlotsInPrepare 还有没有空的 Slot（slot.Item == null），没空位的话也 return掉；
        3.如果 carrySlotsInPrepare 有空位的话，则复制一份 quantity = 1 的当前槽位的 Item 给 carrySlotsInPrepare 的那个空槽，
        当前 slot.Item 的数量 -1（如果减为 0 则给他 SetEmpty()）。
        */
        
        if (item == null) return;

        ItemSlot sourceSlot = FindSlotByItemRef(inventorySlotsInPrepare1, item);
        if (sourceSlot == null) return;
        if (sourceSlot.Item == null) return;

        ItemSlot emptyCarrySlot = FindFirstEmptySlot(carrySlotsInPrepare);
        if (emptyCarrySlot == null) return;

        BasicItem copiedItem = CloneSingleItem(sourceSlot.Item);
        if (copiedItem == null) return;

        emptyCarrySlot.Bind(copiedItem);

        sourceSlot.Item.quantity -= 1;

        if (sourceSlot.Item.quantity <= 0)
        {
            inventoryForDisplay.Remove(sourceSlot.Item);
            sourceSlot.SetEmpty();
        }
        else
        {
            sourceSlot.RefreshView();
        }
    }

    void PutThisItem(BasicItem item)
    {
        // 响应 carrySlotsInPrepare 槽位的点击：
        
        /*
        将其放回到 inventorySlotsInPrepare1，逻辑如下：
        1.如果这个槽位是空的（slot.Item == null），则不处理本次事件；
        2.如果不是空的，就看下 inventorySlotsInPrepare1 中有没有相同道具（itemId相同）的 slot，如果有相同道具，且那个道具的数量
        小于其最大限制数量时（Item.quantity < Item.maximumQuantity），当前carry槽位清空，inventory 中的那个槽位道具 quantity += 1；
        3.如果 inventorySlotsInPrepare1 中没有相同道具，则找一个空槽位，把当前 carry 槽位道具复制到空槽位上，carry 槽位清空。
        */
        
        if (item == null) return;

        ItemSlot carrySlot = FindSlotByItemRef(carrySlotsInPrepare, item);
        if (carrySlot == null) return;
        if (carrySlot.Item == null) return;

        ItemSlot sameItemSlot = FindInventorySlotByItemId(carrySlot.Item.itemId);

        if (sameItemSlot != null &&
            sameItemSlot.Item != null &&
            sameItemSlot.Item.quantity < sameItemSlot.Item.maximumStack)
        {
            sameItemSlot.Item.quantity += 1;
            sameItemSlot.RefreshView();
            carrySlot.SetEmpty();
            return;
        }

        ItemSlot emptyInventorySlot = FindFirstEmptySlot(inventorySlotsInPrepare1);
        if (emptyInventorySlot == null) return;

        BasicItem copiedItem = CloneSingleItem(carrySlot.Item);
        if (copiedItem == null) return;

        emptyInventorySlot.Bind(copiedItem);
        inventoryForDisplay.Add(copiedItem);

        carrySlot.SetEmpty();
    }
    
    public void ApplyCarried()
    {
        //ApplyCarried 表示当前选择道具流程结束了，所以 inventoryForDisplay 也完成了它的使命，
        //直接将他赋给 ADC.Inventory.ownedItems就行，
        //下次进 Prepare 阶段反正会重新复制一份新的inventoryForDisplay，所以不用担心直接赋过去就好。
        ADC.ResetInventory(inventoryForDisplay);
        
        // carrySlotsInPrepare同理
        BasicItem[] appliedItems = new BasicItem[MatchData.MaxCarriedCount];
        for (int i = 0; i < MatchData.MaxCarriedCount; i++)
        {
            appliedItems[i] = carrySlotsInPrepare[i] != null ? carrySlotsInPrepare[i].Item : null;
        }
        MDC.ResetCarriedItems(appliedItems);
    }
    
    // =========================
    // 你可以加的小工具方法写在下面
    // =========================
    
    private bool ContainsSlot(ItemSlot[] slots, ItemSlot target)
    {
        if (slots == null || target == null) return false;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == target)
                return true;
        }

        return false;
    }

    private ItemSlot FindFirstEmptySlot(ItemSlot[] slots)
    {
        if (slots == null) return null;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;
            if (slots[i].Item == null)
                return slots[i];
        }

        return null;
    }

    private ItemSlot FindSlotByItemRef(ItemSlot[] slots, BasicItem item)
    {
        if (slots == null || item == null) return null;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;
            if (slots[i].Item == item)
                return slots[i];
        }

        return null;
    }

    private ItemSlot FindInventorySlotByItemId(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return null;

        for (int i = 0; i < inventorySlotsInPrepare1.Length; i++)
        {
            ItemSlot slot = inventorySlotsInPrepare1[i];
            if (slot == null || slot.Item == null) continue;

            if (slot.Item.itemId == itemId)
                return slot;
        }

        return null;
    }

    private BasicItem CloneSingleItem(BasicItem source)
    {
        if (source == null) return null;

        BasicItem copied = Instantiate(source);
        copied.quantity = 1;
        return copied;
    }

    private void SetPageVisible(CanvasGroup page, bool visible)
    {
        if (page == null) return;

        page.alpha = visible ? 1f : 0f;
        page.interactable = visible;
        page.blocksRaycasts = visible;
    }
}