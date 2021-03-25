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
    
    void Start()
    {
        StartCoroutine(FadeStart());
    }
    
    public IEnumerator FadeStart()
    {
        transition.CrossFadeAlpha(0, 5, true);
        yield return new WaitForSeconds(5);
    }
}
