using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScript : MonoBehaviour
{
    public Text StartText;
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

    public void SetCountText()
    {
        StartText.text = "Press any button to continue";
    }

    public IEnumerator FlashText()
    {
        while (blink)
        {
            StartText.enabled = false;
            yield return new WaitForSeconds(.5f);
            StartText.enabled = true;
            yield return new WaitForSeconds(.5f);
        }
    }

    public IEnumerator FasterFlashText()
    {
        for (int i = 0; i < 5; i++)
        {
            StartText.enabled = false;
            yield return new WaitForSeconds(.1f);
            StartText.enabled = true;
            yield return new WaitForSeconds(.1f);
        }
        FadeToBlack();
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Intro");
    }

    void FadeToBlack()
    {
        fadeImage.color = Color.black;
        fadeImage.canvasRenderer.SetAlpha(0.0f);
        fadeImage.CrossFadeAlpha(1.0f, 1, false);
    }

}