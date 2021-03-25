using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    // Game Objects
    public SoundManager soundManager;
    public PlayerTutorial player;
    public Image transition;
    public Image background;
    
    void Start()
    {
        PlayerData.Tutorial = true;
        StartCoroutine(FadeStart());
    }
    
    public IEnumerator FadeStart()
    {
        background.CrossFadeAlpha(0, 0, true);
        transition.CrossFadeAlpha(0, 5, true);
        yield return new WaitForSeconds(5);
        background.CrossFadeAlpha(0.5f, 5, true);
        yield return new WaitForSeconds(5);
    }
}
