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
        InitializeDefaultData();
    }
    
    private void InitializeDefaultData()
    {
        Profile.playerId = "LocalPlayer";
        Profile.playerName = "Player";
        Profile.energy = 999;
        Profile.gold = 999;
        Profile.diamond = 999;
        Profile.bestScore = 0;
        Profile.tutorialFinished = false;
    }
    
    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        Profile.gold += amount;
    }

    public bool CostEnergy(int amount)
    {
        if (amount <= 0) return true;
        if (Profile.energy < amount) return false;

        Profile.energy -= amount;
        return true;
    }
}
