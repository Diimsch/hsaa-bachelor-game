using System.Collections;
using UnityEngine;
using TMPro;

public class Dialog : MonoBehaviour
{
    public float writeSpeed = 50f;
    public TMP_Text textLabel;
    
    private IEnumerator coroutine;

    public Canvas canvas;
    // Start is called before the first frame update
    void Start()
    {
        canvas.enabled = false;
    }

    public void PushText(string text)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        coroutine = TypeText(text);
        StartCoroutine(coroutine);
    }

    public void Clear()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        canvas.enabled = false;
    }

    private IEnumerator TypeText(string text)
    {
        canvas.enabled = true;
        
        float t = 0;
        int i = 0;

        while (i < text.Length)
        {
            t += Time.deltaTime * writeSpeed;
            i = Mathf.Clamp(Mathf.FloorToInt(t), 0, text.Length);

            textLabel.text = text.Substring(0, i);

            yield return null;
        }
        SoundManagerScript.StopSound();
    }
}
