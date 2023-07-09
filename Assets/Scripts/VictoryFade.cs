using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryFade : MonoBehaviour
{
    Coroutine hCoroutine;
    Vector3 pos;
    Color c;
    SpriteRenderer sr;
    SpriteRenderer sr_child;

    public void play()
    {
        gameObject.SetActive(true);
        sr = GetComponent<SpriteRenderer>();
        sr_child = GetComponentInChildren<SpriteRenderer>();
        pos = transform.position;
        pos.y = 3.5f;
        transform.position = pos;

        c = sr.color;
        c.a = 0;
        sr.color = c;
        sr_child.color = c;
        StartCoroutine(fadeIn());
    }

    public void hide()
    {
        if (hCoroutine == null)
            StopCoroutine(hCoroutine);
        gameObject.SetActive(false);
        pos = transform.position;
        pos.y = 4;
        transform.position = pos;
        c.a = 0;
        sr.color = c;
        sr_child.color = c;
        gameObject.SetActive(false);
    }

    IEnumerator fadeIn()
    {
        gameObject.SetActive(true);
        float t = 0;
        while (t < 2)
        {
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
            if (c.a < 1)
            {
                c.a += Time.deltaTime;
                sr.color = c;
                sr_child.color = c;
            }
            else
            {
                c.a = 1;
                sr.color = c;
                sr_child.color = c;
            }

            pos = transform.position;
            pos.y = 3.5f - t/2;
            transform.position = pos;
            sr.color = c;
            sr_child.color = c;
        }
        hCoroutine = null;
    }
}
