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
    
    
    public void ClearPreparedConsumables()
    {
        if (CurrentMatch == null) return;

        CurrentMatch.ClearCarriedItems();
        GameEvents.RaisePreparedConsumablesChanged();
    }

    public bool CanAddPreparedConsumable()
    {
        if (CurrentMatch == null) return false;
        return CurrentMatch.CanCarryMore();
    }
    
     public bool AddPreparedConsumable(string itemId)
    {
        /*if (CurrentMatch == null) return false;

        bool success = CurrentMatch.AddCarriedItem(itemId);
        if (!success) return false;

        GameEvents.RaisePreparedConsumablesChanged();*/
        return true;
    }
    
     public bool RemovePreparedConsumable(string itemId)
    {
        if (CurrentMatch == null) return false;

        bool success = CurrentMatch.RemoveCarriedItem(itemId);
        if (!success) return false;

        GameEvents.RaisePreparedConsumablesChanged();
        return true;
    }
}
