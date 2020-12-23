using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public Image startImage;
    public Text startText;
    public Image fadeImage;
    private bool blink = true;

    void Start()
    {
        StartCoroutine("FlashText");
    }

    void Update()
    {
        if(Input.anyKeyDown)
        {
            StopCoroutine("FlashText");
            StartCoroutine("FasterFlashText");
        }
    }

    public IEnumerator FlashText()
    {
        while (blink)
        {
            startImage.CrossFadeAlpha(0, 1, false);
            startText.CrossFadeAlpha(0, 1, false);
            yield return new WaitForSeconds(1);
            startImage.CrossFadeAlpha(1, 1, false);
            startText.CrossFadeAlpha(1, 1, false);
            yield return new WaitForSeconds(1);
        }
    }

    public IEnumerator FasterFlashText()
    {
        for (int i = 0; i < 5; i++)
        {
            startImage.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            startImage.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.1f);
        }
        FadeToBlack();
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Intro");
    }

    void FadeToBlack()
    {
        fadeImage.color = Color.black;
        fadeImage.canvasRenderer.SetAlpha(0);
        fadeImage.CrossFadeAlpha(1, 1, false);
    }

}