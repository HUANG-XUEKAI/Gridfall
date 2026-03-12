using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPopup : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    public void BindConfirmAction(Action confirmAction)
    {
        
    }

    public void BindCancelAction(Action cancelAction)
    {
        
    }
}
