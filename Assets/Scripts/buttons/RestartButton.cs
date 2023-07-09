using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartButton : MonoBehaviour
{
    [SerializeField] bool changeStart;

    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (changeStart)
                ++Board.start_id;
            Debug.Log("Restart");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
