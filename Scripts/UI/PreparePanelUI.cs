using System.Collections.Generic;
using UnityEngine;

public class PreparePanelUI : MonoBehaviour
{
    [Header("Carry Slots")]
    [SerializeField] private ItemSlot carrySlot1;
    [SerializeField] private ItemSlot carrySlot2;

    [Header("Inventory Pages")]
    [SerializeField] private Transform page1Root;
    [SerializeField] private Transform page2Root;
    [SerializeField] private Transform page3Root;

    [Header("Prefabs")]
    [SerializeField] private ItemSlot itemSlotPrefab;
    
    [Header("Definitions")]
    [SerializeField] private List<BasicItem> ConsumableDB = new();

    private readonly List<ItemSlot> spawnedInventorySlots = new();

    private AccountDataCenter ADC => AccountDataCenter.Instance;
    private MatchDataCenter MDC => MatchDataCenter.Instance;

    private void OnEnable()
    {
        GameEvents.InventoryChanged += RefreshAll;
        GameEvents.PreparedConsumablesChanged += RefreshAll;

        RefreshAll();
    }

    private void OnDisable()
    {
        GameEvents.InventoryChanged -= RefreshAll;
        GameEvents.PreparedConsumablesChanged -= RefreshAll;
    }

    private void RefreshAll()
    {
        RefreshCarrySlots();
        RefreshInventorySlots();
    }

    private void RefreshCarrySlots()
    {
        RefreshCarrySlotByIndex(carrySlot1, 0);
        RefreshCarrySlotByIndex(carrySlot2, 1);
    }

    private void RefreshCarrySlotByIndex(ItemSlot slot, int index)
    {
        if (slot == null)
            return;

        var match = MDC?.CurrentMatch;
        if (match == null || match.carriedItems == null || index >= match.carriedItems.Count)
        {
            slot.SetEmpty();
            slot.SetClick(null);
            return;
        }

        var dataSlot = match.carriedItems[index];
        if (dataSlot == null || string.IsNullOrEmpty(dataSlot.item.itemId))
        {
            slot.SetEmpty();
            slot.SetClick(null);
            return;
        }

        var def = GetDefinition(dataSlot.item.itemId);
        if (def == null)
        {
            slot.SetEmpty();
            slot.SetClick(null);
            return;
        }

        slot.Bind(def, 1);
        slot.SetClick(() =>
        {
            MDC.RemovePreparedConsumable(def.itemId);
        });
    }

    private void RefreshInventorySlots()
    {
        ClearSpawnedInventorySlots();

        if (ADC == null || ADC.Inventory == null || ADC.Inventory.consumables == null)
            return;

        List<Transform> pages = new() { page1Root, page2Root, page3Root };
        int pageIndex = 0;
        int itemCountInCurrentPage = 0;

        foreach (var stack in ADC.Inventory.consumables)
        {
            if (stack == null || string.IsNullOrEmpty(stack.item.itemId) || stack.count <= 0)
                continue;

            int preparedCount = GetPreparedCount(stack.item.itemId);
            int availableCount = stack.count - preparedCount;
            if (availableCount <= 0)
                continue;

            var def = GetDefinition(stack.item.itemId);
            if (def == null)
                continue;

            if (pageIndex >= pages.Count)
                break;

            CreateInventorySlot(pages[pageIndex], def, availableCount);

            itemCountInCurrentPage++;
            if (itemCountInCurrentPage >= 9)
            {
                itemCountInCurrentPage = 0;
                pageIndex++;
            }
        }
    }

    private void CreateInventorySlot(Transform parent, BasicItem def, int count)
    {
        if (parent == null || itemSlotPrefab == null || def == null)
            return;

        var slot = Instantiate(itemSlotPrefab, parent);
        spawnedInventorySlots.Add(slot);

        slot.Bind(def, count);
        slot.SetClick(() =>
        {
            MDC.AddPreparedConsumable(def.itemId);
        });
    }

    private void ClearSpawnedInventorySlots()
    {
        foreach (var slot in spawnedInventorySlots)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }

        spawnedInventorySlots.Clear();
    }

    private BasicItem GetDefinition(string itemId)
    {
        return ConsumableDB.Find(x => x != null && x.itemId == itemId);
    }

    private int GetPreparedCount(string itemId)
    {
        var match = MDC?.CurrentMatch;
        if (match == null || match.carriedItems == null)
            return 0;

        int count = 0;
        foreach (var slot in match.carriedItems)
        {
            if (slot != null && slot.item.itemId == itemId)
                count++;
        }

        return count;
    }
}