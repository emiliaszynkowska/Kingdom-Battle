using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public SoundManager soundManager;
    public QuestManager questManager;
    public PlayerTutorial player;
    public WiggTutorial wigg;
    public GameObject sword;
    public GameObject shield;
    public Image transition;
    public Image background;
    public CircleCollider2D weaponsCollider;
    public CircleCollider2D chestsCollider;
    public CircleCollider2D monsterCollider1;
    public CircleCollider2D monsterCollider2;
    public CircleCollider2D specialCollider;
    
    void Start()
    {
        background.canvasRenderer.SetAlpha(0);
        transition.CrossFadeAlpha(0, 5, true);
        background.CrossFadeAlpha(1, 5, true);
    }

    public void DisableWeapons()
    {
        sword.SetActive(false);
        shield.SetActive(false);
        questManager.Event("Find a weapon", 0, false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.IsTouching(weaponsCollider) && other.name.Equals("Player"))
        {
            weaponsCollider.enabled = false;
            StartCoroutine(wigg.FindWeapons());
        }
        else if (other.IsTouching(chestsCollider) && other.name.Equals("Player"))
        {
            
        }
        else if (other.IsTouching(monsterCollider1) && other.name.Equals("Player"))
        {
            
        }
        else if (other.IsTouching(monsterCollider2) && other.name.Equals("Player"))
        {
            
        }
        else if (other.IsTouching(specialCollider) && other.name.Equals("Player"))
        {
            
        }
    }
}
