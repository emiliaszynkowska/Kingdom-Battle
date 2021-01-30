using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public Image startImage;
    public Image fade;

    private void Start()
    {
        StartCoroutine(FadeOut());
    }

    public void StartGame()
    {
        StartCoroutine(FadeIn());
        SceneManager.LoadScene("Main");
    }

    public void Easy()
    {
        StartCoroutine(FadeIn());
        SceneManager.LoadScene("Easy");
    }
    
    public void Medium()
    {
        StartCoroutine(FadeIn());
        SceneManager.LoadScene("Medium");
    }
    
    public void Hard()
    {
        StartCoroutine(FadeIn());
        SceneManager.LoadScene("Hard");
    }

    public void Boss()
    {
        StartCoroutine(FadeIn());
        SceneManager.LoadScene("Boss");
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