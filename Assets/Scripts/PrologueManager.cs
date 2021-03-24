using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PrologueManager : MonoBehaviour
{
    public GameObject dialog;
    public Text message;
    public Text text;
    public Image fade;
    
    void Start()
    {
        StartCoroutine(FadeOut());
        StartCoroutine(Dialog());
    }

    public IEnumerator Dialog()
    {
        dialog.GetComponent<Image>().CrossFadeAlpha(0, 0, true);
        message.CrossFadeAlpha(0, 0, true);
        text.CrossFadeAlpha(0, 0, true);
        yield return new WaitForSeconds(2);
        dialog.GetComponent<Image>().CrossFadeAlpha(0.4f, 2, true);
        message.CrossFadeAlpha(0.8f, 2, true);
        StartCoroutine(FadeText());
        yield return Speak("Our story begins in the mighty kingdom of Astreron...");
        yield return Speak("This glorious nation was home to the strongest warriors, sharpest minds, and most dazzling beauties across all the dimensions.");
        yield return Speak("It stood as gleaming beacon of hope, prosperity and peace to all people. But now the mighty have fallen... This once powerful nation has been reduced to shell of its former self.");
        yield return Speak("For generations the kingdom flourished until the reign of King Eldar the Evil. The young king was corrupted by power and began to destroy what his ancestors had created.");
        yield return Speak("His subjects fled as the kingdom fell to ruin, with only a few brave wizards choosing to remain in an attempt to save there home from total destruction.");
        yield return Speak("The kingdom is now overrun by monsters and demons with many trying and many failing to stop the tyranny of the evil king.");
        yield return Speak("A young elf has journeyed across the kingdom and now stands at the foot of evil king's castle. But is he too late?");
        yield return FadeIn();
        SceneManager.LoadScene("Castle");
    }

    public IEnumerator Speak(string text)
    {
        message.text = text;
        yield return new WaitForSecondsRealtime(0.1f);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
    }
    
    public IEnumerator FadeText()
    {
        while (true)
        {
            text.CrossFadeColor(new Color(1, 1, 1, 0), 1, true, true);
            yield return new WaitForSecondsRealtime(1);
            text.CrossFadeColor(Color.white, 1, true, true);
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
