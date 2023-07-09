using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningHover : MonoBehaviour
{
    public string text = "";
    [SerializeField] GameObject WarningBox;
    [SerializeField] Text WarningText;
    public void SetWarning(bool visible, string text = "")
    {
        gameObject.SetActive(visible);
        if (visible)
        {
            WarningText.text = text;
        }
        else
        {
            WarningBox.SetActive(false);
        }
    }

    private void OnMouseEnter()
    {
        WarningBox.SetActive(true);
    }
    private void OnMouseExit()
    {
        WarningBox.SetActive(false);
    }
}
