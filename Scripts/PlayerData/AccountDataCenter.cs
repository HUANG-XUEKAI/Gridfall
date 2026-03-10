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
}