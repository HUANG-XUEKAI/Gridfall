using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }
    
    [SerializeField] private BasicItem[] itemsDatabase;
    
    private Dictionary<string, BasicItem> dict_ItemsDB;
    public IReadOnlyList<BasicItem> ItemsDatabase => itemsDatabase;
    private AccountDataCenter ADC => AccountDataCenter.Instance;
    private MatchDataCenter MDC => MatchDataCenter.Instance;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        
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

    #region 局内改变道具
    /*public bool AddCarriedItem(string itemId)
    {
        var item = GetItemById(itemId);
        if (item == null) return false;
        return MDC.AddCarriedItem(item);
    }

    public void CostCarriedItem(int index)
    {
        MDC.CostCarriedItem(index);
    }*/
    
    public bool TryUseCarriedItem(int slotIndex)
    {
        if (GameFlowStateMachine.Instance == null) return false;
        if (!GameFlowStateMachine.Instance.IsInState(GameFlowState.GamePlay)) return false;
        if (MDC == null || MDC.CurrentMatch == null) return false;

        if (slotIndex < 0 || slotIndex >= MatchData.MaxCarriedCount)
            return false;

        BasicItem item = MDC.CurrentMatch.carriedItems[slotIndex];
        if (item == null) return false;
        if (item.itemClass != ItemClass.Consumable) return false;

        bool success = TryApplyItemEffect(item);

        if (!success)
            return false;

        MDC.CostCarriedItem(slotIndex);
        return true;
    }

    private bool TryApplyItemEffect(BasicItem item)
    {
        if (item == null) return false;

        switch (item.effect)
        {
            case ItemEffect.AddHP:
                return TryApplyAddHP(item);

            case ItemEffect.Bomb3x3Random:
                return TryApplyBomb3x3Random(item);

            default:
                Debug.LogWarning($"未处理的道具效果: {item.effect}");
                return false;
        }
    }

    private bool TryApplyAddHP(BasicItem item)
    {
        if (MDC == null || MDC.CurrentMatch == null) return false;
        if (MDC.CurrentMatch.currentHP >= MatchData.MaxHP) return false;

        int healAmount = Mathf.Max(1, item.effectValue);
        MDC.AddHP(healAmount);
        return true;
    }

    private bool TryApplyBomb3x3Random(BasicItem item)
    {
        if (BoardManager.Instance == null)
        {
            Debug.LogWarning("BoardManager is null.");
            return false;
        }

        return BoardManager.Instance.TryBombRandom3x3();
    }
    #endregion 
    
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
