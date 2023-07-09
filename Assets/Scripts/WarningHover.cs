using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningHover : MonoBehaviour
{
    public string text = "";
    [SerializeField] GameObject WarningBox;
    [SerializeField] Text WarningText;
    List<Vector2> hints = new List<Vector2>();
    Color colorBuffer = Color.white* 0.8f;
    public void SetWarning(bool visible, List<Vector2> _hints, string text = "")
    {
        hints = _hints;
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
        colorBuffer = gameObject.GetComponent<SpriteRenderer>().color;
        gameObject.GetComponent<SpriteRenderer>().color = Color.white * 0.8f;
        foreach(Vector2 coord in hints)
        {
            Square s = Board._i.getSquare(coord);
            if (s != null)
                s.setFailHint(true);
        }
    }
    private void OnMouseExit()
    {
        gameObject.GetComponent<SpriteRenderer>().color = colorBuffer;
        WarningBox.SetActive(false);
        for (int i = 0; i < 64; ++i)
            Board._i.getSquare(i).setFailHint(false);
    }
}
