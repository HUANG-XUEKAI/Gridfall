using UnityEngine;

public class UIStateController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private CanvasGroup mainMenu;
    [SerializeField] private CanvasGroup gameInterface;
    [SerializeField] private CanvasGroup gameOverPanel;

    private void Awake()
    {
        if (GameFlowStateMachine.Instance != null)
            GameFlowStateMachine.Instance.OnStateChanged += HandleStateChanged;
    }

    private void OnDestroy()
    {
        if (GameFlowStateMachine.Instance != null)
            GameFlowStateMachine.Instance.OnStateChanged -= HandleStateChanged;
    }

    private void Start()
    {
        ApplyState(GameFlowState.MainMenu);
    }

    private void HandleStateChanged(GameFlowState oldState, GameFlowState newState)
    {
        ApplyState(newState);
    }

    private void ApplyState(GameFlowState state)
    {
        HideAll();

        switch (state)
        {
            case GameFlowState.MainMenu:
                Show(mainMenu);
                break;
            
            case GameFlowState.GamePlay:
                Show(gameInterface);
                break;

            case GameFlowState.GameOver:
                Show(gameOverPanel);
                break;
        }
    }

    private void HideAll()
    {
        Hide(mainMenu);
        Hide(gameInterface);
        Hide(gameOverPanel);
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