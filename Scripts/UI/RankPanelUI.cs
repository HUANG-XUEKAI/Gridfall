using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankPanelUI : MonoBehaviour
{
    [Header("List")]
    [SerializeField] private Transform contentRoot;
    [SerializeField] private RankItemUI itemPrefab;

    [Header("Optional")]
    [SerializeField] private TextMeshProUGUI selfBestScoreText;

    private readonly List<RankItemUI> spawnedItems = new();
    
    public void RefreshView()
    {
        List<RankEntry> rankList = LocalMockRankProvider.GetRankList();

        EnsureItemCount(rankList.Count);

        for (int i = 0; i < spawnedItems.Count; i++)
        {
            bool active = i < rankList.Count;
            spawnedItems[i].gameObject.SetActive(active);

            if (!active)
                continue;

            spawnedItems[i].Bind(i + 1, rankList[i]);
        }

        if (selfBestScoreText != null && AccountDataCenter.Instance != null)
            selfBestScoreText.text = $"我的历史最高分：{AccountDataCenter.Instance.Profile.bestScore}";
    }

    private void EnsureItemCount(int targetCount)
    {
        if (contentRoot == null || itemPrefab == null)
            return;

        while (spawnedItems.Count < targetCount)
        {
            RankItemUI item = Instantiate(itemPrefab, contentRoot);
            spawnedItems.Add(item);
        }
    }
}