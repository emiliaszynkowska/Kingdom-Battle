using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    // Game Objects
    public SoundManager soundManager;
    public UIManager uiManager;
    public GameObject king;
    public GameObject player;
    public GameObject enemies;
    public GameObject spikes;
    public Sprite spikesUp;
    public Image transition;
    public Image radialFade;
    public Image fade;
    // Variables
    private bool active;
    // Prefabs
    public RuntimeAnimatorController elvenKingAnimatorController;
    
    void Start()
    {
        uiManager.fade.CrossFadeAlpha(0, 0, true);
        transition.CrossFadeAlpha(0.0f, 5.0f, true);
        uiManager.Level("King Eldar");
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!active && other.CompareTag("Player") && other is BoxCollider2D)
            StartCoroutine(Dialog());
    }

    public IEnumerator Dialog()
    {
        // Dialog
        active = true;
        uiManager.PauseGame();
        yield return uiManager.Speak("King Eldar", "You...");
        yield return uiManager.Speak("King Eldar", "How? How are you here?");
        yield return uiManager.Speak("King Eldar", "You should be trapped in the dungeons, stuck there for eternity never to escape... or be killed trying.");
        yield return uiManager.Speak("King Eldar", "There is no way a weakling like you could escape without aid.");
        yield return uiManager.Speak("King Eldar", "...");
        yield return uiManager.Speak("King Eldar", "Wigg? Damn him and his wretched coven of fools!");
        yield return uiManager.Speak("King Eldar", "You may have escaped, but you still have to defeat me, and your little friends aren't here to save you.");
        yield return uiManager.Speak("King Eldar", "Slaying those infernal creatures is one thing, but now you will face the might of a king.");
        yield return uiManager.Speak("King Eldar", "I will crush you like the vermin you are... and those pathetic wizards.");
        yield return uiManager.Speak("King Eldar", "Now... die.");
        // Transition
        soundManager.PlaySound(soundManager.jump);
        king.GetComponent<Animator>().SetTrigger("Jump");
        yield return new WaitForSecondsRealtime(0.5f);
        soundManager.PlaySound(soundManager.groundPound);
        king.GetComponent<Rigidbody2D>().AddForce(Vector2.down * 50, ForceMode2D.Impulse);
        player.GetComponent<Rigidbody2D>().AddForce(Vector2.down * 3, ForceMode2D.Impulse);
        yield return new WaitForSecondsRealtime(1);
        king.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        soundManager.PlaySound(soundManager.spikes);
        for (int i = 0; i < spikes.transform.childCount; i++)
        {
            spikes.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = spikesUp;
            spikes.transform.GetChild(i).GetComponent<BoxCollider2D>().enabled = true;
        }
        yield return new WaitForSecondsRealtime(1);
        // Battle
        soundManager.StopMusic();
        soundManager.PlayMusic(soundManager.battleMusic);
        radialFade.gameObject.SetActive(true);
        radialFade.CrossFadeAlpha(0.0f, 0.0f, true);
        radialFade.CrossFadeAlpha(0.4f, 1.0f, true);
        king.GetComponent<KingController>().enabled = true;
        king.GetComponent<KingController>().SetDungeon(new Dungeon(new Rect(-8, 0, 16, 12)));
        uiManager.ResumeGame();
    }

    public IEnumerator Win()
    {
        uiManager.PauseGame();
        player.GetComponent<Animator>().runtimeAnimatorController = elvenKingAnimatorController;
        player.GetComponent<PlayerController>().weapon.SetActive(false);
        player.GetComponent<PlayerController>().playerRenderer.color = Color.white;
        uiManager.timers.SetActive(false);
        while (player.transform.position.x < 0.2f)
        {
            player.transform.position = new Vector3(player.transform.position.x + 0.1f, player.transform.position.y);
            yield return new WaitForSecondsRealtime(0.03f);
        }
        while (player.transform.position.x > 0.2f)
        {
            player.transform.position = new Vector3(player.transform.position.x - 0.1f, player.transform.position.y);
            yield return new WaitForSecondsRealtime(0.03f);
        }
        while (player.transform.position.y < 16.75f)
        {
            player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.1f);
            yield return new WaitForSecondsRealtime(0.03f);
        }
        yield return new WaitForSecondsRealtime(1);
        uiManager.fade.enabled = true;
        yield return uiManager.FadeIn();
        SceneManager.LoadScene("End");
    }
}
