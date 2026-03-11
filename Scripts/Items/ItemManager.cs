using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField] private BasicItem[] itemsDatabase;
    private Dictionary<string, BasicItem> dict_ItemsDB;
    
    private AccountDataCenter ADC => AccountDataCenter.Instance;
    private MatchDataCenter MDC => MatchDataCenter.Instance;
    
    private void Awake()
    {
        InitItemsDictionary();
    }
    
    public void InitItemsDictionary()
    {
        if (dict_ItemsDB == null)
            dict_ItemsDB = new Dictionary<string, BasicItem>();

        if (itemsDatabase == null)
        {
            Debug.LogWarning("itemsDatabase is null.");
            return;
        }

        foreach (var item in itemsDatabase)
        {
            if (item == null || 
                string.IsNullOrEmpty(item.itemId) || 
                dict_ItemsDB.ContainsKey(item.itemId))
                continue;
            
            dict_ItemsDB.Add(item.itemId, item);
        }
    }
    
    public BasicItem GetItemById(string itemId)
    {
        if (dict_ItemsDB == null)
        {
            Debug.LogWarning("dict_ItemsDB is null.");
            return null;
        }

        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning("itemId is null or empty.");
            return null;
        }

        if (dict_ItemsDB.TryGetValue(itemId, out BasicItem item))
        {
            return item;
        }

        Debug.LogWarning($"未找到 itemId: {itemId}");
        return null;
    }
    
    // 局内改变道具
    public bool AddCarriedItem(string itemId)
    {
        var item = GetItemById(itemId);
        if (item == null) return false;
        return MDC.AddCarriedItem(item);
    }

    public void CostCarriedItem(int index)
    {
        MDC.CostCarriedItem(index);
    }
    
    // 改变账户道具数据
    public bool AddInventoryItem(string itemId, int amount = 1)
    {
        var item = GetItemById(itemId);
        if (item == null) return false;
        
        return ADC.AddItem(item, amount);
    }

    public bool CostInventoryItem(string itemId, int amount = 1)
    {
        var item = GetItemById(itemId);
        if (item == null) return false;
        
        return ADC.CostItem(item, amount);
    }
}
