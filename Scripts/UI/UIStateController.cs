using UnityEngine;

public class UIStateController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private CanvasGroup mainMenu;
    [SerializeField] private CanvasGroup preparePanel;
    [SerializeField] private CanvasGroup gameInterface;
    [SerializeField] private CanvasGroup gameOverPanel;
    [SerializeField] private CanvasGroup pausePanel;
    [SerializeField] private CanvasGroup shopInterface;
    
    private void Awake()
    {
        GameEvents.OnStateChanged += HandleStateChanged;
        GameEvents.GamePaused += SetPausePanelVisible;
    }

    private void OnDestroy()
    {
        GameEvents.OnStateChanged -= HandleStateChanged;
        GameEvents.GamePaused -= SetPausePanelVisible;
    }

    private void Start()
    {
        HideAll();
        ApplyState(GameFlowState.None, GameFlowState.MainMenu);
    }

    private void HandleStateChanged(GameFlowState oldState, GameFlowState newState)
    {
        ApplyState(oldState, newState);
    }

    private void ApplyState(GameFlowState oldState, GameFlowState newState)
    {
        HideOldStateUI(oldState);
        ShowNewStateUI(newState);
    }

    private void HideOldStateUI(GameFlowState oldState)
    {
        switch (oldState)
        {
            case GameFlowState.MainMenu:
            {
                Hide(mainMenu);
                break;
            }
            case GameFlowState.Prepare:
            {
                Hide(preparePanel);
                break;
            }
            case GameFlowState.GamePlay:
            {
                Hide(gameInterface);
                break;
            }
            case GameFlowState.GameOver:
            {
                Hide(gameOverPanel);
                break;
            }
            case GameFlowState.Shopping:
            {
                Hide(shopInterface);
                break;
            }
        }
    }

    private void ShowNewStateUI(GameFlowState newState)
    {
        switch (newState)
        {
            case GameFlowState.MainMenu:
            {
                Show(mainMenu);
                break;
            }
            case GameFlowState.Prepare:
            {
                Show(preparePanel);
                break;
            }
            case GameFlowState.GamePlay:
            {
                Show(gameInterface);
                break;
            }
            case GameFlowState.GameOver:
            {
                Show(gameOverPanel);
                break;
            }
            case GameFlowState.Shopping:
            {
                Show(shopInterface);
                break;
            }
        }
    }

    private void HideAll()
    {
        Hide(mainMenu);
        Hide(pausePanel);
        Hide(preparePanel);
        Hide(gameInterface);
        Hide(gameOverPanel);
    }

    private void SetPausePanelVisible(bool visible)
    {
        if (visible)
            Show(pausePanel);
        else
            Hide(pausePanel);
    }

    public static void Show(CanvasGroup gui)
    {
        if (gui != null && !IsVisible(gui))
        {
            gui.alpha = 1;
            gui.blocksRaycasts = true;
            gui.interactable = true;
        }
    }

    public static void Hide(CanvasGroup gui)
    {
        if (gui != null && IsVisible(gui))
        {
            gui.alpha = 0;
            gui.blocksRaycasts = false;
            gui.interactable = false;
        }
    }
    
    public static bool IsVisible(CanvasGroup gui)
    {
        return (gui.alpha > 0.0001f);
    }
}