using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CastleController : MonoBehaviour
{
    // Game Objects
    public SoundManager soundManager;
    public PlayerMinimal player;
    public GameObject portal;
    public Image transition;
    public Image fade;
    // Dialog
    public GameObject dialog;
    public Text title;
    public Text message;
    public Text text;
    
    public void Start()
    {
        StartCoroutine(FadeOut());
        StartCoroutine(FadeText()); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        StartCoroutine(Dialog());
    }

    public IEnumerator Dialog()
    {
        player.active = false;
        yield return Speak("King Eldar", "Well, what have we here? Another brave warrior wants to challenge me?");
        yield return Speak("King Eldar", "Who are you? Speak, elf.");
        yield return Speak("King Eldar", "...");
        yield return Speak("King Eldar", "Oh? It appears our new friend doesn't wish to talk. Well listen here you insolent fool... You have some spirit venturing into my domain.");
        yield return Speak("King Eldar", "But mark my words, courage alone accomplishes nothing. You are weak and pathetic. Begone and you will do well not to return.");
        yield return Speak("King Eldar", "...");
        yield return Speak("King Eldar", "What's this? You dare defy me?");
        yield return Speak("King Eldar", "I will give you one more chance to leave this place and never return, else you will be sure to suffer at my hand.");
        yield return Speak("King Eldar", "...");
        yield return Speak("King Eldar", "Fine...I tried to show you remorse but alas you are too stupid to know when you are out of your depth.");
        yield return Speak("King Eldar", "My pets in the dungeons could do with a new plaything, it's been a while since they've been fed.");
        yield return Speak("King Eldar", "May you meet the same fate as all others that opposed me.");
        soundManager.StopMusic();
        soundManager.PlayMusic(soundManager.portal);
        portal.SetActive(true);
        portal.transform.localScale = new Vector3(0, 0, 0);
        for (int i=0; i<10; i++)
        {
            portal.transform.localScale = new Vector3(portal.transform.localScale.x + 0.1f, portal.transform.localScale.y + 0.1f, portal.transform.localScale.z + 0.1f);
            yield return new WaitForSeconds(0.2f);
        }
        yield return FadeExit();

    }

    public IEnumerator Speak(string npcName, string text)
    {
        dialog.SetActive(true);
        title.text = npcName;
        message.text = text;
        yield return new WaitForSeconds(0.1f);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
        dialog.SetActive(false);
    }

    public IEnumerator FadeText()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {
            text.CrossFadeColor(new Color(1, 1, 1, 0), 1, true, true);
            yield return new WaitForSeconds(1);
            text.CrossFadeColor(Color.white, 1, true, true);
            yield return new WaitForSeconds(1);
        }
    }

    public IEnumerator FadeExit()
    {
        transition.gameObject.SetActive(true);
        transition.CrossFadeAlpha(0, 0, true);
        transition.CrossFadeAlpha(1, 5, true);
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Tutorial");
    }
    
    public IEnumerator FadeIn()
    {
        fade.CrossFadeAlpha(1.0f, 3, true);
        yield return new WaitForSeconds(2);
    }
    
    public IEnumerator FadeOut()
    {
        fade.CrossFadeAlpha(0.0f, 3, true);
        yield return new WaitForSeconds(2);
    }
}
