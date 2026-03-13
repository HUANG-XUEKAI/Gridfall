using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ItemDisplay : MonoBehaviour
{
    [Header("Prepare")]
    [SerializeField] private ItemSlot[] carrySlotsInPrepare = new ItemSlot[MatchData.MaxCarriedCount];
    
    // 先只做一页
    [SerializeField] private ItemSlot[] inventorySlotsInPrepare1 = new ItemSlot[9];
    [SerializeField] private ItemSlot[] inventorySlotsInPrepare2 = new ItemSlot[9];
    [SerializeField] private ItemSlot[] inventorySlotsInPrepare3 = new ItemSlot[9];
    [SerializeField] private CanvasGroup preparePage1Root;
    [SerializeField] private CanvasGroup preparePage2Root;
    [SerializeField] private CanvasGroup preparePage3Root;
    
    private List<BasicItem> inventoryForDisplay;
    
    [Header("Gameplay")]
    [SerializeField] private ItemSlot[] carrySlotsInGame = new ItemSlot[MatchData.MaxCarriedCount]; //先不管
    
    [Header("Shopping")]
    [SerializeField] private ItemSlot[] itemSlotsInShopping1;
    [SerializeField] private CanvasGroup shoppingPage1Root;
    //private PurchasePopup purchasePopup;
    
    [Header("Prefabs")]
    [SerializeField] private ItemSlot itemSlotPrefab;
    [SerializeField] private PurchasePopup purchasePopupPrefab;
    
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
        GameEvents.OnStateChanged += BuildInventoryDisplay;
    }

    private void OnDisable()
    {
        GameEvents.OnStateChanged -= BuildInventoryDisplay;
    }
    
    private void BuildInventoryDisplay(GameFlowState oldState, GameFlowState newState)
    {
        ExitOldState(oldState);
        EnterNewState(newState);
    }

    void EnterNewState(GameFlowState newState)
    {
        switch (newState)
        {
            case GameFlowState.Prepare:
            {
                BuildDisplayData();
                RegisterPrepareSlotsEvent();
                FillInSlotsInPrepare();
                UpdatePrepareSlotsDisplay();
                break;
            }
            case GameFlowState.GamePlay:
            {
                RegisterGamePlaySlotsEvent();
                FillInSlotsInGame();
                UpdateGamePlaySlotsDisplay();
                break;
            }
            case GameFlowState.Shopping:
            {
                FillInSlotsInShopping();
                RegisterShoppingSlotsEvent();
                break;
            }
        }
    }

    void ExitOldState(GameFlowState oldState)
    {
        switch (oldState)
        {
            case GameFlowState.Prepare:
            {
                ClearPrepareData();
                UnregisterPrepareSlotsEvent();
                return;
            }
            case GameFlowState.GamePlay:
            {
                UnregisterGamePlaySlotsEvent();
                return;
            }
            case GameFlowState.Shopping:
            {
                ClearSlotsInShopping();
                UnregisterShoppingSlotsEvent();
                break;
            }
        }
    }

    #region Prepare
    private void BuildDisplayData()
    {
        // 重置 inventoryForDisplay：从 ADC.Inventory.ownedItems 拷贝一份
        inventoryForDisplay = new List<BasicItem>();

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
        
        SetPageVisible(preparePage1Root, true);
        SetPageVisible(preparePage2Root, false);
        SetPageVisible(preparePage3Root, false);

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
    
    void ApplyCarried()
    {
        //ApplyCarried 表示当前选择道具流程结束了，所以 inventoryForDisplay 也完成了它的使命，
        //直接将他赋给 ADC.Inventory.ownedItems就行，
        //下次进 Prepare 阶段反正会重新复制一份新的inventoryForDisplay，所以不用担心直接赋过去就好。
        ADC.ResetInventory(inventoryForDisplay);
        
        // carrySlotsInPrepare同理
        BasicItem[] appliedItems = new BasicItem[MatchData.MaxCarriedCount];
        for (int i = 0; i < MatchData.MaxCarriedCount; i++)
        {
            if (carrySlotsInPrepare[i].IsEmpty()) continue;
            appliedItems[i] = carrySlotsInPrepare[i].Item;
        }
        MDC.ResetCarriedItems(appliedItems);
    }

    void ClearPrepareData()
    {
        if (GameFlowStateMachine.Instance.CurrentState == GameFlowState.GamePlay)
            ApplyCarried();
        
        inventoryForDisplay = null;
        
        foreach (var slot in inventorySlotsInPrepare1)
        {
            slot.SetEmpty();
        }

        foreach (var slot in carrySlotsInPrepare)
        {
            slot.SetEmpty();
        }
    }
    #endregion
    
    #region GamePlay
    private void FillInSlotsInGame()
    {
        for (int i = 0; i < carrySlotsInGame.Length; i++)
        {
            ItemSlot slot = carrySlotsInGame[i];
            if (slot == null) continue;

            BasicItem item = null;

            if (MDC != null &&
                MDC.CurrentMatch != null &&
                MDC.CurrentMatch.carriedItems != null &&
                i < MDC.CurrentMatch.carriedItems.Length)
            {
                item = MDC.CurrentMatch.carriedItems[i];
            }

            if (item == null)
                slot.SetEmpty();
            else
                slot.Bind(item);
        }
    }

    private void RegisterGamePlaySlotsEvent()
    {
        foreach (var slot in carrySlotsInGame)
        {
            if (slot == null) continue;
            slot.OnSlotClicked -= HandleGamePlaySlotClicked;
            slot.OnSlotClicked += HandleGamePlaySlotClicked;
        }
    }

    private void UnregisterGamePlaySlotsEvent()
    {
        foreach (var slot in carrySlotsInGame)
        {
            if (slot == null) continue;
            slot.OnSlotClicked -= HandleGamePlaySlotClicked;
        }
    }

    private void HandleGamePlaySlotClicked(ItemSlot slot)
    {
        int slotIndex = FindSlotIndex(carrySlotsInGame, slot);
        if (slotIndex < 0) return;
        if (ItemManager.Instance == null) return;

        bool used = ItemManager.Instance.TryUseCarriedItem(slotIndex);

        if (!used)
            return;

        FillInSlotsInGame();
        UpdateGamePlaySlotsDisplay();
    }

    private void UpdateGamePlaySlotsDisplay()
    {
        foreach (var slot in carrySlotsInGame)
        {
            if (slot == null) continue;
            slot.RefreshView();
        }
    }

    private int FindSlotIndex(ItemSlot[] slots, ItemSlot target)
    {
        if (slots == null || target == null) return -1;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == target)
                return i;
        }

        return -1;
    }
    #endregion

    #region Shopping
    void RegisterShoppingSlotsEvent()
    {
        foreach (var slot in itemSlotsInShopping1)
        {
            if (slot == null) continue;
            slot.OnSlotClicked -= HandleShoppingSlotClicked;
            slot.OnSlotClicked += HandleShoppingSlotClicked;
        }
    }

    void UnregisterShoppingSlotsEvent()
    {
        foreach (var slot in itemSlotsInShopping1)
        {
            if (slot == null) continue;
            slot.OnSlotClicked -= HandleShoppingSlotClicked;
        }
    }

    private void FillInSlotsInShopping()
    {
        SetPageVisible(shoppingPage1Root, true);
        
        if (ItemManager.Instance == null) return;
        if (ItemManager.Instance.ItemsDatabase == null) return;
        
        int fillCount = Mathf.Min(itemSlotsInShopping1.Length, ItemManager.Instance.ItemsDatabase.Count);
        for (int i = 0; i < fillCount; i++)
        {
            BasicItem item = ItemManager.Instance.ItemsDatabase[i];
            
            if (itemSlotsInShopping1[i] == null ||
                item == null || 
                string.IsNullOrEmpty(item.itemId)) 
                continue;

            BasicItem displayItem = Instantiate(item);
            displayItem.quantity = 1;
            itemSlotsInShopping1[i].Bind(displayItem);
        }
    }

    private void ClearSlotsInShopping()
    {
        SetPageVisible(shoppingPage1Root, false);

        foreach (var slot in itemSlotsInShopping1)
        {
            slot.SetEmpty();
        }
    }

    void HandleShoppingSlotClicked(ItemSlot slot)
    {
        if (slot == null ||
            slot.Item == null ||
            purchasePopupPrefab == null)
            return;
        
        if (ADC == null || ADC.Profile == null) return;
        
        var purchasePopup = Instantiate(purchasePopupPrefab, shoppingPage1Root.transform.parent);
        purchasePopup.Set(slot.Item);
    }
    #endregion
    
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