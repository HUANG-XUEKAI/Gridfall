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
        GameEvents.InventoryItemsChanged += RefreshInventoryOnly;
        GameEvents.CarriedItemsChanged += RefreshCarriedOnly;
        GameFlowStateMachine.OnStateChanged += BuildInventoryDisplay;

        BuildDisplayDataFromCenters();
        RebuildSpawnedInventorySlots();
        UpdateDisplay();
    }

    private void OnDisable()
    {
        GameEvents.InventoryItemsChanged -= RefreshInventoryOnly;
        GameEvents.CarriedItemsChanged -= RefreshCarriedOnly;
        GameFlowStateMachine.OnStateChanged -= BuildInventoryDisplay;
    }
    
    private void RefreshInventoryOnly()
    {
        if (GameFlowStateMachine.Instance == null) return;
        if (!GameFlowStateMachine.Instance.IsInState(GameFlowState.Prepare)) return;

        BuildInventoryForDisplay();
        RebuildSpawnedInventorySlots();
        UpdateDisplay();
    }

    private void RefreshCarriedOnly()
    {
        if (GameFlowStateMachine.Instance == null) return;

        if (GameFlowStateMachine.Instance.IsInState(GameFlowState.Prepare))
        {
            BuildCarriedForDisplay();
            UpdateDisplay();
            return;
        }

        if (GameFlowStateMachine.Instance.IsInState(GameFlowState.GamePlay))
        {
            UpdateInGameCarrySlots();
        }
    }

    // Prepare状态：
    // 清理掉 carriedForDisplay 和 inventoryForDisplay
    // 拉取当前账号库存数据，复制一份给 inventoryForDisplay 用于显示
    // 生成对应数量的 itemSlotPrefab 到 pageRoot 下面
    // 将 Item 绑定至对应的 itemSlot
    // 绑定 spawnedSlots 里槽位按钮的点击事件，和 carrySlotsInPrepare 里槽位按钮的点击事件
        
    // GamePlay状态：
        
    // Shopping状态：
    private void BuildInventoryDisplay(GameFlowState oldState, GameFlowState newState)
    {
        switch (newState)
        {
            case GameFlowState.Prepare:
            {
                BuildDisplayDataFromCenters();
                RebuildSpawnedInventorySlots();
                UpdateDisplay();
                return;
            }
        }
    }
    
    private void BuildDisplayDataFromCenters()
    {
        BuildInventoryForDisplay();
        BuildCarriedForDisplay();
    }

    private void BuildInventoryForDisplay()
    {
        inventoryForDisplay.Clear();

        if (ADC == null || 
            ADC.Inventory == null || 
            ADC.Inventory.ownedItems == null)
            return;

        foreach (var item in ADC.Inventory.ownedItems)
        {
            if (item == null) continue;
            if (string.IsNullOrEmpty(item.itemId)) continue;
            if (item.quantity <= 0) continue;

            inventoryForDisplay.Add(Instantiate(item));
        }
    }
    
    private void BuildCarriedForDisplay()
    {
        carriedForDisplay = new BasicItem[MatchData.MaxCarriedCount];
    }
    
    private void RebuildSpawnedInventorySlots()
    {
        ClearSpawnedSlots();

        if (itemSlotPrefab == null) return;

        Transform[] pages = { page1Root, page2Root, page3Root };
        const int pageCapacity = 9;

        for (int i = 0; i < inventoryForDisplay.Count; i++)
        {
            int pageIndex = i / pageCapacity;
            if (pageIndex >= pages.Length) break;

            Transform parent = pages[pageIndex];
            if (parent == null) continue;

            ItemSlot slot = Instantiate(itemSlotPrefab, parent);
            int capturedIndex = i;

            slot.SetClick(() => TakeThisItem(capturedIndex));
            spawnedSlots.Add(slot);
        }
    }

    private void ClearSpawnedSlots()
    {
        for (int i = 0; i < spawnedSlots.Count; i++)
        {
            if (spawnedSlots[i] != null)
                Destroy(spawnedSlots[i].gameObject);
        }

        spawnedSlots.Clear();
    }
    
    private int FindFirstEmptyCarryIndex()
    {
        for (int i = 0; i < carriedForDisplay.Length; i++)
        {
            if (carriedForDisplay[i] == null)
                return i;
        }

        return -1;
    }

    private int FindInventoryIndexByItemId(string itemId)
    {
        if (string.IsNullOrEmpty(itemId)) return -1;

        for (int i = 0; i < inventoryForDisplay.Count; i++)
        {
            var item = inventoryForDisplay[i];
            if (item != null && item.itemId == itemId)
                return i;
        }

        return -1;
    }

    public void TakeThisItem(int inventoryIndex)
    {
        
    }

    public void PutThisItem(int carryIndex)
    {
        
    }

    public void ApplyCarried()
    {
        MDC.ResetCarriedItems(carriedForDisplay);
        ADC.ResetInventory(inventoryForDisplay);
    }
    
    private BasicItem[] CloneCarriedArray()
    {
        BasicItem[] result = new BasicItem[MatchData.MaxCarriedCount];

        for (int i = 0; i < carriedForDisplay.Length; i++)
        {
            if (carriedForDisplay[i] != null)
                result[i] = Instantiate(carriedForDisplay[i]);
        }

        return result;
    }
    
    private List<BasicItem> CloneInventoryList()
    {
        List<BasicItem> result = new();

        for (int i = 0; i < inventoryForDisplay.Count; i++)
        {
            var item = inventoryForDisplay[i];
            if (item == null) continue;

            result.Add(Instantiate(item));
        }

        return result;
    }

    public void UpdateDisplay()
    {
        for (int i = 0; i < spawnedSlots.Count; i++)
        {
            if (i < inventoryForDisplay.Count && inventoryForDisplay[i] != null)
            {
                spawnedSlots[i].Bind(inventoryForDisplay[i]);

                int capturedIndex = i;
                spawnedSlots[i].SetClick(() => TakeThisItem(capturedIndex));
            }
            else
            {
                spawnedSlots[i].SetEmpty();
                spawnedSlots[i].SetClick(null);
            }
        }

        for (int i = 0; i < carrySlotsInPrepare.Length; i++)
        {
            if (carrySlotsInPrepare[i] == null) continue;

            int capturedIndex = i;

            if (i < carriedForDisplay.Length && carriedForDisplay[i] != null)
            {
                carrySlotsInPrepare[i].Bind(carriedForDisplay[i]);
                carrySlotsInPrepare[i].SetClick(() => PutThisItem(capturedIndex));
            }
            else
            {
                carrySlotsInPrepare[i].SetEmpty();
                carrySlotsInPrepare[i].SetClick(null);
            }
        }

        UpdateInGameCarrySlots();
    }
    
    private void UpdateInGameCarrySlots()
    {
        for (int i = 0; i < carrySlotsInGame.Length; i++)
        {
            if (carrySlotsInGame[i] == null) continue;

            BasicItem item = null;

            if (MDC != null &&
                MDC.CurrentMatch != null &&
                MDC.CurrentMatch.carriedItems != null &&
                i < MDC.CurrentMatch.carriedItems.Length)
            {
                item = MDC.CurrentMatch.carriedItems[i];
            }

            if (item != null)
                carrySlotsInGame[i].Bind(item);
            else
                carrySlotsInGame[i].SetEmpty();
        }
    }
}