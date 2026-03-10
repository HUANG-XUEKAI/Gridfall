using System.IO;
using UnityEngine;

public static class AccountLocalSave
{
    private const string FileName = "player_profile.json";

    private static string FilePath =>
        Path.Combine(Application.persistentDataPath, FileName);

    public static void SaveProfile(PlayerProfileData data)
    {
        if (data == null) return;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(FilePath, json);
    }

    public static PlayerProfileData LoadProfile()
    {
        if (!File.Exists(FilePath))
            return null;

        string json = File.ReadAllText(FilePath);
        if (string.IsNullOrEmpty(json))
            return null;

        return JsonUtility.FromJson<PlayerProfileData>(json);
    }

    public static bool HasProfileSave()
    {
        return File.Exists(FilePath);
    }

    public static void DeleteProfileSave()
    {
        if (File.Exists(FilePath))
            File.Delete(FilePath);
    }
}