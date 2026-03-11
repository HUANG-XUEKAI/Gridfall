using System;

[Serializable]
public class PlayerAccountSaveData
{
    public PlayerProfileData profile = new();
    public PlayerInventoryData inventory = new();
    public PlayerProgressData progress = new();
}