using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPopup : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    public void BindConfirmAction(Action confirmAction)
    {
        if (confirmButton == null) return;

        confirmButton.onClick.RemoveAllListeners();

        if (confirmAction != null)
            confirmButton.onClick.AddListener(() => confirmAction.Invoke());
    }

    public void BindCancelAction(Action cancelAction)
    {
        if (cancelButton == null) return;

        cancelButton.onClick.RemoveAllListeners();

        if (cancelAction != null)
            cancelButton.onClick.AddListener(() => cancelAction.Invoke());
    }
}
