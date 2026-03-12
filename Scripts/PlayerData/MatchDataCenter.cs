using System;
using UnityEngine;
using System.Linq;

public class MatchDataCenter : MonoBehaviour
{
    public static MatchDataCenter Instance { get; private set; }
    
    public MatchData CurrentMatch { get; private set; } = new();
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    
    public void CreatNewMatchData()
    {
        CurrentMatch = new MatchData
        {
            currentHP = MatchData.DefaultHP,
            currentScore = 0,
            isGaming = false,
            isPausing = false,
        };
    }

    public void StartMatch()
    {
        CurrentMatch.isGaming = true;
        
        GameEvents.RaiseGameStared(new GameStartedEvent());
    }

    public void EndMatch()
    {
        CurrentMatch.isGaming = false;
        
        GameEvents.RaiseGameOver(new GameOverEvent
        {
            finalScore = CurrentMatch.currentScore,
        });
    }
    
    public void PauseMatch()
    {
        if (CurrentMatch == null) return;
        if (CurrentMatch.isPausing) return;

        CurrentMatch.isPausing = true;
        GameEvents.RaiseGamePaused(true);
    }

    public void ResumeMatch()
    {
        if (CurrentMatch == null) return;
        if (!CurrentMatch.isPausing) return;

        CurrentMatch.isPausing = false;
        GameEvents.RaiseGamePaused(false);
    }
    
    public void AddScore(int amount, string reason)
    {
        if (amount <= 0) return;
        CurrentMatch.currentScore += amount;
        
        GameEvents.RaiseScoreChanged(new ScoreChangedEvent
        {
            currentScore = CurrentMatch.currentScore,
            delta = amount,
            reason = reason
        });
    }

    public void ResetScore()
    {
        CurrentMatch.currentScore = 0;
        
        GameEvents.RaiseScoreChanged(new ScoreChangedEvent
        {
            currentScore = 0,
            delta = 0,
            reason = "Reset"
        });
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0 || !CurrentMatch.isGaming) return;

        int oldHP = CurrentMatch.currentHP;
        CurrentMatch.currentHP -= amount;
        int delta = oldHP - CurrentMatch.currentHP;
        
        if (CurrentMatch.currentHP <= 0)
        {
            CurrentMatch.currentHP = 0;
            delta = oldHP - CurrentMatch.currentHP;
            EndMatch();
        }
        
        GameEvents.RaiseHPChanged(new HPChangedEvent
        {
            currentHP = CurrentMatch.currentHP,
            delta = delta
        });
    }

    public void AddHP(int amount)
    {
        if (amount <= 0) return;

        int oldHP = CurrentMatch.currentHP;
        CurrentMatch.currentHP = Mathf.Min(
            MatchData.MaxHP,
            CurrentMatch.currentHP + amount
        );
        int delta = CurrentMatch.currentHP - oldHP;
        
        GameEvents.RaiseHPChanged(new HPChangedEvent
        {
            currentHP = CurrentMatch.currentHP,
            delta = delta
        });
    }
    
    
    public void ClearCarriedItems()
    {
        if (CurrentMatch == null) return;

        Array.Clear(CurrentMatch.carriedItems, 0, 2);
        GameEvents.RaiseCarriedItemsChanged();
    }

    public bool CanCarryMoreItem()
    {
        if (CurrentMatch == null) return false;

        foreach (var item in CurrentMatch.carriedItems)
        {
            if (item == null) 
                return true;
        }
        
        return false;
    }
    
    public bool AddCarriedItem(BasicItem item)
    {
        if (CurrentMatch == null) return false;
        if (item == null || 
            string.IsNullOrEmpty(item.itemId) || 
            item.itemClass != ItemClass.Consumable) return false;
        
        for (int i = 0; i < CurrentMatch.carriedItems.Length; i++)
        {
            if (CurrentMatch.carriedItems[i] == null)
            {
                CurrentMatch.carriedItems[i] = Instantiate(item);
                GameEvents.RaiseCarriedItemsChanged();
                return true;
            }
        }
        
        return false;
    }
    
    public void CostCarriedItem(int index)
    {
        if (CurrentMatch == null) return;

        int clampedIndex = Mathf.Clamp(index, 0, MatchData.MaxCarriedCount -1);
        CurrentMatch.carriedItems[clampedIndex] = null;
        
        GameEvents.RaiseCarriedItemsChanged();
    }

    public void ResetCarriedItems(BasicItem[] items)
    {
        if (CurrentMatch == null || items == null) return;
        
        CurrentMatch.carriedItems = items;
        
        GameEvents.RaiseCarriedItemsChanged();
    }
}
