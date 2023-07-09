using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonUndo : MonoBehaviour
{
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Undo");
            Player._i.Undo();
        }
    }
}
