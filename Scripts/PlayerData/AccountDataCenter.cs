using UnityEngine;

public class AccountDataCenter : MonoBehaviour
{
    public static AccountDataCenter Instance { get; private set; }

    public PlayerProfileData Profile { get; private set; } = new();
    public PlayerInventoryData Inventory { get; private set; } = new();
    public PlayerProgressData Progress { get; private set; } = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadOrCreateProfile();
    }
    
    private void Start()
    {
        NotifyProfileLoaded();
        
        AddConsumable("hp_potion", 3);
        AddConsumable("bomb_3x3", 1);

        Debug.Log(GetConsumableCount("hp_potion"));
        Debug.Log(GetConsumableCount("bomb_3x3"));
    }

    private void LoadOrCreateProfile()
    {
        var loaded = AccountLocalSave.LoadProfile();

        if (loaded != null)
        {
            Profile = loaded;
            return;
        }

        InitializeDefaultData();
        SaveProfile();
    }

    private void InitializeDefaultData()
    {
        Profile.playerId = "LocalPlayer";
        Profile.playerName = "Player";
        Profile.energy = 20;
        Profile.gold = 0;
        Profile.diamond = 0;
        Profile.bestScore = 0;
        Profile.tutorialFinished = false;
    }

    private void SaveProfile()
    {
        AccountLocalSave.SaveProfile(Profile);
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
    
    public int GetConsumableCount(string itemId)
    {
        if (Inventory == null || Inventory.consumables == null || string.IsNullOrEmpty(itemId))
            return 0;

        var stack = Inventory.consumables.Find(x => x.itemId == itemId);
        return stack != null ? stack.count : 0;
    }

    public void AddConsumable(string itemId, int amount)
    {
        if (Inventory == null || Inventory.consumables == null) return;
        if (string.IsNullOrEmpty(itemId) || amount <= 0) return;

        var stack = Inventory.consumables.Find(x => x.itemId == itemId);
        if (stack == null)
        {
            stack = new InventoryItemStack
            {
                itemId = itemId,
                count = amount
            };
            Inventory.consumables.Add(stack);
        }
        else
        {
            stack.count += amount;
        }

        //SaveInventory(); //保存数据
        //RaiseInventoryChanged(); //广播通知
    }

    public bool CostConsumable(string itemId, int amount)
    {
        if (Inventory == null || Inventory.consumables == null) return false;
        if (string.IsNullOrEmpty(itemId) || amount <= 0) return false;

        var stack = Inventory.consumables.Find(x => x.itemId == itemId);
        if (stack == null || stack.count < amount)
            return false;

        stack.count -= amount;

        if (stack.count <= 0)
            Inventory.consumables.Remove(stack);

        //SaveInventory(); //保存数据
        //RaiseInventoryChanged(); //广播通知
        return true;
    }
}