using System.Collections.Generic;
using UnityEngine;

public class AccountDataCenter : MonoBehaviour
{
    public static AccountDataCenter Instance { get; private set; }

    public PlayerAccountData AccountData { get; private set; }  = new PlayerAccountData();
    public PlayerAccountData.ProfileData Profile { get; private set; } = new();
    public PlayerAccountData.InventoryData Inventory { get; private set; } = new();
    public PlayerAccountData.ProgressData Progress { get; private set; } = new();
    
    // 测试用
    public BasicItem initialItem;

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
        NotifyProfileLoaded();
        
        /*// 调试代码
        AddConsumable("hp_potion", 3);
        AddConsumable("bomb_3x3", 1);

        Debug.Log(GetConsumableCount("hp_potion"));
        Debug.Log(GetConsumableCount("bomb_3x3"));*/
    }

    private void LoadOrCreateAccount()
    {
        var loaded = AccountLocalSave.LoadAccount();

        if (loaded != null)
        {
            Profile = loaded.profile ?? new PlayerAccountData.ProfileData();
            Inventory = loaded.inventory ?? new PlayerAccountData.InventoryData();
            Progress = loaded.progress ?? new PlayerAccountData.ProgressData();
            return;
        }

        InitializeDefaultData();
        SaveAccount();
    }
    
    private void SaveAccount()
    {
        AccountLocalSave.SaveAccount(new PlayerAccountData
        {
            profile = Profile,
            inventory = Inventory,
            progress = Progress
        });
    }


    private void InitializeDefaultData()
    {
        AccountData.profile.playerId = "LocalPlayer";
        AccountData.profile.playerName = "Player";
        AccountData.profile.energy = 20;
        AccountData.profile.gold = 0;
        AccountData.profile.diamond = 0;
        AccountData.profile.bestScore = 0;
        AccountData.profile.tutorialFinished = false;

        // 测试用
        initialItem.quantity = 100;
        AccountData.inventory.ownedItems[0] = initialItem;
        
        Profile = AccountData.profile;
        Inventory = AccountData.inventory;
        Progress = AccountData.progress;
    }

    private void SaveProfile()
    {
        AccountLocalSave.SaveAccount(AccountData);
    }

    private void RaiseProfileChanged()
    {
        GameEvents.RaiseProfileChanged(new ProfileChangedEvent
        {
            energy = Profile.energy,
            gold = Profile.gold,
            diamond = Profile.diamond
        });
    }

    public void NotifyProfileLoaded()
    {
        RaiseProfileChanged();
    }

    public bool CostEnergy(int amount)
    {
        if (amount <= 0) return true;
        if (Profile.energy < amount) return false;

        Profile.energy -= amount;
        SaveProfile();
        RaiseProfileChanged();
        return true;
    }

    public void AddEnergy(int amount)
    {
        if (amount <= 0) return;

        Profile.energy += amount;
        SaveProfile();
        RaiseProfileChanged();
    }

    public bool CostGold(int amount)
    {
        if (amount <= 0) return true;
        if (Profile.gold < amount) return false;

        Profile.gold -= amount;
        SaveProfile();
        RaiseProfileChanged();
        return true;
    }

    public void AddGold(int amount)
    {
        if (amount <= 0) return;

        Profile.gold += amount;
        SaveProfile();
        RaiseProfileChanged();
    }

    public bool CostDiamond(int amount)
    {
        if (amount <= 0) return true;
        if (Profile.diamond < amount) return false;

        Profile.diamond -= amount;
        SaveProfile();
        RaiseProfileChanged();
        return true;
    }

    public void AddDiamond(int amount)
    {
        if (amount <= 0) return;

        Profile.diamond += amount;
        SaveProfile();
        RaiseProfileChanged();
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
                if ((ownedItem.quantity + amount) >= ownedItem.maximumQuantity) 
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
        if (Inventory == null || Inventory.ownedItems == null) return false;
        if (item == null || string.IsNullOrEmpty(item.itemId) || amount <= 0) return false;

        for (int i = 0; i < Inventory.ownedItems.Count; i++)
        {
            var ownedItem = Inventory.ownedItems[i];
            if (ownedItem.itemId == item.itemId)
            {
                if ((ownedItem.quantity - amount) <= 0) 
                    return false;
                
                ownedItem.quantity -= amount;
                SaveAccount();
                GameEvents.RaiseInventoryItemsChanged();
                return true;
            }
        }

        return false;
    }

    public void ResetInventory(List<BasicItem> items)
    {
        if (items == null || Inventory == null) return;
        Inventory.ownedItems = items;
        SaveAccount();
        GameEvents.RaiseInventoryItemsChanged();
    }
}