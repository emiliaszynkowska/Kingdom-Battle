using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public SoundManager soundManager;
    public Image startImage;
    public Image startButton;
    public Image quitButton;
    public Text text;
    public Image fade;

    public void Start()
    {
        StartCoroutine(FadeOut());
        StartCoroutine(FadeText());
    }

    public void StartGame()
    {
        soundManager.PlaySound(soundManager.play);
        StartCoroutine(FadeIn());
        SceneManager.LoadScene("Main");
    }

    public void Quit()
    {
        soundManager.PlaySound(soundManager.clickButton);
        StartCoroutine(FadeOut());
        Application.Quit();
    }

    public IEnumerator FadeText()
    {
        while (true)
        {
            text.CrossFadeColor(new Color(0.3f, 0, 0, 1), 1, true, false);
            yield return new WaitForSecondsRealtime(1);
            text.CrossFadeColor(new Color(1, 0, 0, 1), 1, true, false);
            yield return new WaitForSecondsRealtime(1);
        }
    }
    
    public IEnumerator FadeIn()
    {
        fade.CrossFadeAlpha(1.0f, 1, true);
        yield return new WaitForSecondsRealtime(1);
    }
    
    public IEnumerator FadeOut()
    {
        fade.CrossFadeAlpha(0.0f, 1, true);
        yield return new WaitForSecondsRealtime(1);
    }

}