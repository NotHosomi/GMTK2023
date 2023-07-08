using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpHover : MonoBehaviour
{
    [SerializeField] GameObject helpPanel;
    [SerializeField] float fadeRate;
    Coroutine hCoroutine;
    Color c;
    SpriteRenderer sr;
    private void Awake()
    {
        sr = helpPanel.GetComponent<SpriteRenderer>();
        c = sr.color;
        c.a = 0;
        sr.color = c;
    }

    private void OnMouseEnter()
    {
        if (hCoroutine != null)
            StopCoroutine(hCoroutine);
        hCoroutine = StartCoroutine(fadeIn());
    }
    private void OnMouseExit()
    {
        if (hCoroutine != null)
            StopCoroutine(hCoroutine);
        hCoroutine = StartCoroutine(fadeOut());
    }

    IEnumerator fadeIn()
    {
        helpPanel.SetActive(true);
        while (c.a < 255)
        {
            yield return new WaitForEndOfFrame();

            c.a += fadeRate * Time.deltaTime;
            if (c.a > 255)
                c.a = 255;
            sr.color = c;
        }
        hCoroutine = null;
    }

    IEnumerator fadeOut()
    {
        while (c.a > 0)
        {
            yield return new WaitForEndOfFrame();

            c.a -= fadeRate * Time.deltaTime;
            if (c.a < 0)
                c.a = 0;
            sr.color = c;
        }
        helpPanel.SetActive(false);
        hCoroutine = null;
    }
}
