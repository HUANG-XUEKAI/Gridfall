using System.IO;
using UnityEngine;

public static class AccountLocalSave
{
    private const string FileName = "player_account.json";

    private static string FilePath =>
        Path.Combine(Application.persistentDataPath, FileName);

    public static void SaveAccount(PlayerAccountData data)
    {
        if (data == null) return;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(FilePath, json);
    }

    public static PlayerAccountData LoadAccount()
    {
        if (!File.Exists(FilePath))
            return null;

        string json = File.ReadAllText(FilePath);
        if (string.IsNullOrEmpty(json))
            return null;

        return JsonUtility.FromJson<PlayerAccountData>(json);
    }

    public static bool HasAccountSave()
    {
        return File.Exists(FilePath);
    }

    public static void DeleteAccountSave()
    {
        if (File.Exists(FilePath))
            File.Delete(FilePath);
    }
}