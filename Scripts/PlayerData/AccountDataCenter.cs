using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AccountDataCenter : MonoBehaviour
{
    public static AccountDataCenter Instance { get; private set; }

    public PlayerAccountData AccountData { get; private set; }  = new PlayerAccountData();
    public PlayerAccountData.ProfileData Profile { get; private set; } = new();
    public PlayerAccountData.InventoryData Inventory { get; private set; } = new();
    public PlayerAccountData.ProgressData Progress { get; private set; } = new();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadOrCreateAccount();
    }
    
    private void Start()
    {
        BuildRuntimeInventoryFromJson();
        //NotifyProfileLoaded();
    }
    
    private void BuildRuntimeInventoryFromJson()
    {
        if (Inventory == null)
            Inventory = new PlayerAccountData.InventoryData();

        Inventory.ownedItems =
            ConvertJsonDataToOwnedItems(Inventory.ownedItems_json);

        //Debug.Log(string.Join(", ", Inventory.ownedItems.Select(x => x.itemId)));
    }

    private void LoadOrCreateAccount()
    {
        var data = AccountLocalSave.LoadAccount();

        if (data != null)
        {
            AccountData = data;
            AccountData.inventory ??= new PlayerAccountData.InventoryData();
            AccountData.profile ??= new PlayerAccountData.ProfileData();
            AccountData.progress ??= new PlayerAccountData.ProgressData();
            
            Profile = AccountData.profile;
            Inventory = AccountData.inventory;
            Progress = AccountData.progress;
            
            return;
        }

        InitializeDefaultData();
        SaveAccount();
    }
    
    private void SaveAccount()
    {
        Inventory.ownedItems_json = ConvertOwnedItemsToJsonData(Inventory.ownedItems);
        PlayerAccountData data = new PlayerAccountData
        {
            profile = Profile,
            inventory = Inventory,
            progress = Progress
        };
        
        AccountLocalSave.SaveAccount(data);
    }


    private void InitializeDefaultData()
    {
        AccountData.profile.playerId = "LocalPlayer";
        AccountData.profile.playerName = "Player";
        AccountData.profile.energy = 20;
        AccountData.profile.gold = 9999;
        AccountData.profile.diamond = 999;
        AccountData.profile.bestScore = 0;
        AccountData.profile.tutorialFinished = false;
        
        Profile = AccountData.profile;
        Inventory = AccountData.inventory;
        Progress = AccountData.progress;
    }

    private void SaveProfile()
    {
        SaveAccount();
    }

    /*private void RaiseProfileChanged()
    {
        GameEvents.RaiseProfileChanged(new GameEvents.ProfileChangedEvent
        {
            energy = Profile.energy,
            gold = Profile.gold,
            diamond = Profile.diamond
        });
    }*/

    public void NotifyProfileLoaded()
    {
        
    }
    
    public bool TryUpdateBestScore(int score)
    {
        if (score <= Profile.bestScore)
            return false;

        Profile.bestScore = score;
        SaveProfile();
        return true;
    }

    public bool CostEnergy(int amount, string reason = "")
    {
        if (amount <= 0) return true;
        if (Profile.energy < amount) return false;

        Profile.energy -= amount;
        SaveProfile();
        GameEvents.RaiseEnergyChanged(new GameEvents.ChangeData
        {  
            currValue = Profile.energy,
            delta = amount,
            reason = reason
        });
        return true;
    }

    public void AddEnergy(int amount, string reason = "")
    {
        if (amount <= 0) return;

        Profile.energy += amount;
        SaveProfile();
        GameEvents.RaiseEnergyChanged(new GameEvents.ChangeData
        {  
            currValue = Profile.energy,
            delta = amount,
            reason = reason
        });
    }

    public bool CostGold(int amount, string reason = "")
    {
        if (amount <= 0) return true;
        if (Profile.gold < amount) return false;

        Profile.gold -= amount;
        SaveProfile();
        GameEvents.RaiseGoldChanged(new GameEvents.ChangeData
        {  
            currValue = Profile.gold,
            delta = amount,
            reason = reason
        });
        return true;
    }

    public void EarnGold(int amount, string reason = "")
    {
        if (amount <= 0) return;

        Profile.gold += amount;
        SaveProfile();
        GameEvents.RaiseGoldChanged(new GameEvents.ChangeData
        {  
            currValue = Profile.gold,
            delta = amount,
            reason = reason
        });
    }

    public bool CostDiamond(int amount, string reason = "")
    {
        if (amount <= 0) return true;
        if (Profile.diamond < amount) return false;

        Profile.diamond -= amount;
        SaveProfile();
        GameEvents.RaiseDiamondChanged(new GameEvents.ChangeData
        {  
            currValue = Profile.diamond,
            delta = amount,
            reason = reason
        });
        return true;
    }

    public void AddDiamond(int amount, string reason = "")
    {
        if (amount <= 0) return;

        Profile.diamond += amount;
        SaveProfile();
        GameEvents.RaiseDiamondChanged(new GameEvents.ChangeData
        {  
            currValue = Profile.diamond,
            delta = amount,
            reason = reason
        });
    }
    
    public bool AddItem(BasicItem item, int amount)
    {
        if (Inventory == null || Inventory.ownedItems == null) return false;
        if (item == null || string.IsNullOrEmpty(item.itemId) || amount <= 0) return false;
        
        for (int i = 0; i < Inventory.ownedItems.Count; i++)
        {
            var ownedItem = Inventory.ownedItems[i];
            if (ownedItem.itemId == item.itemId)
            {
                if ((ownedItem.quantity + amount) > ownedItem.maximumStack) 
                    return false;
                
                ownedItem.quantity += amount;
                SaveAccount();
                GameEvents.RaiseInventoryItemsChanged();
                return true;
            }
        }

        item.quantity = amount;
        Inventory.ownedItems.Add(Instantiate(item));
        SaveAccount();
        GameEvents.RaiseInventoryItemsChanged();
        return true;
    }

    public bool CostItem(BasicItem item, int amount)
    {
        if (Inventory == null || 
            Inventory.ownedItems == null) 
            return false;
        
        if (item == null || 
            string.IsNullOrEmpty(item.itemId) || 
            amount <= 0) 
            return false;

        for (int i = 0; i < Inventory.ownedItems.Count; i++)
        {
            var ownedItem = Inventory.ownedItems[i];
            if (ownedItem.itemId == item.itemId)
            {
                if (ownedItem.quantity < amount)
                    return false;
                
                ownedItem.quantity -= amount;
                
                if (ownedItem.quantity == 0)
                    Inventory.ownedItems.RemoveAt(i);
                
                SaveAccount();
                GameEvents.RaiseInventoryItemsChanged();
                return true;
            }
        }

        return false;
    }

    public BasicItem FindOwnedItem(string itemId)
    {
        if (Inventory == null || 
            Inventory.ownedItems == null) 
            return null;
        
        if (string.IsNullOrEmpty(itemId)) 
            return null;

        foreach (var i in Inventory.ownedItems)
        {
            if (i != null && i.itemId == itemId)
                return i;
        }
        
        return null;
    }

    public void ResetInventory(List<BasicItem> items)
    {
        if (items == null || Inventory == null) return;
        Inventory.ownedItems = items;
        SaveAccount();
        GameEvents.RaiseInventoryItemsChanged();
    }
    
    #region List<ItemOwnedData>、List<BasicItem>转换
    public static List<PlayerAccountData.ItemOwnedData> ConvertOwnedItemsToJsonData(List<BasicItem> runtimeItems)
    {
        List<PlayerAccountData.ItemOwnedData> result = new();

        if (runtimeItems == null) return result;

        foreach (BasicItem item in runtimeItems)
        {
            if (item == null) continue;
            if (string.IsNullOrEmpty(item.itemId)) continue;
            if (item.quantity <= 0) continue;

            result.Add(new PlayerAccountData.ItemOwnedData
            {
                itemId = item.itemId,
                quantity = item.quantity
            });
        }

        return result;
    }
    
    public static List<BasicItem> ConvertJsonDataToOwnedItems(List<PlayerAccountData.ItemOwnedData> jsonItems)
    {
        List<BasicItem> result = new();

        if (jsonItems == null) return result;
        if (ItemManager.Instance == null) return result;

        foreach (PlayerAccountData.ItemOwnedData data in jsonItems)
        {
            if (data == null) continue;
            if (string.IsNullOrEmpty(data.itemId)) continue;
            if (data.quantity <= 0) continue;

            BasicItem itemDef = ItemManager.Instance.GetItemById(data.itemId);
            if (itemDef == null)
            {
                Debug.LogWarning($"找不到 itemId = {data.itemId} 对应的 BasicItem");
                continue;
            }

            BasicItem runtimeItem = Instantiate(itemDef);
            runtimeItem.quantity = data.quantity;
            result.Add(runtimeItem);
        }

        return result;
    }
    #endregion
}