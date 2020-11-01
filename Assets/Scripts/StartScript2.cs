using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartScript2 : MonoBehaviour
{
    public Image fadeImage;

    void Start()
    {
        StartCoroutine("Fade");
    }

    IEnumerator Fade()
    {
        FadeFromBlack();
        yield return new WaitForSeconds(2);
    }
    void FadeFromBlack()
    {
        fadeImage.color = Color.black;
        fadeImage.canvasRenderer.SetAlpha(1.0f);
        fadeImage.CrossFadeAlpha(0.0f, 1, false);
    }

}
