using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public abstract class Display_Base<T> : MonoBehaviour
{
    protected TextMeshProUGUI textMesh;

    protected virtual void Awake()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    protected virtual void Start()
    {
        UpdateDisplay(GetInitialValue());
    }
    
    protected virtual void OnEnable()
    {
        Subscribe();
    }

    protected virtual void OnDisable()
    {
        Unsubscribe();
    }

    protected abstract void Subscribe();
    protected abstract void Unsubscribe();

    protected abstract T GetInitialValue();
    protected abstract void UpdateDisplay(T value);
}
