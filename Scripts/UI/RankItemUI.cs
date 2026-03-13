using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankItemUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject selfMark;
    [SerializeField] private Image background;

    [Header("Style")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selfColor = new Color(1f, 0.95f, 0.65f, 1f);

    public void Bind(int rankIndex, RankEntry entry)
    {
        if (entry == null)
            return;

        if (rankText != null)
            rankText.text = rankIndex.ToString();

        if (nameText != null)
            nameText.text = entry.isSelf ? $"{entry.displayName} (你)" : entry.displayName;

        if (scoreText != null)
            scoreText.text = entry.bestScore.ToString();

        if (selfMark != null)
            selfMark.SetActive(entry.isSelf);

        if (background != null)
            background.color = entry.isSelf ? selfColor : normalColor;
    }
}