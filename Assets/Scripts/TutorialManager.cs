using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public SoundManager soundManager;
    public QuestManager questManager;
    public UIManager uiManager;
    public PlayerTutorial player;
    public WiggTutorial wigg;
    public GameObject sword;
    public GameObject shield;
    public GameObject shine1;
    public GameObject shine2;
    public Image transition;
    public Image background;
    public CircleCollider2D weaponsCollider;
    public CircleCollider2D chestsCollider;
    public CircleCollider2D monsterCollider;
    public CircleCollider2D monstersCollider;
    public CircleCollider2D specialCollider;
    
    void Start()
    {
        background.canvasRenderer.SetAlpha(0);
        transition.CrossFadeAlpha(0, 5, true);
        background.CrossFadeAlpha(1, 5, true);
        uiManager.SetDifficulty(5);
        PlayerData.Difficulty = 5;
    }

    public void DisableWeapons()
    {
        sword.SetActive(false);
        shield.SetActive(false);
        shine1.SetActive(false);
        shine2.SetActive(false);
        questManager.Event("Find a weapon", 0, false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.IsTouching(weaponsCollider) && other.name != null && other.name.Equals("Player"))
        {
            weaponsCollider.enabled = false;
            StartCoroutine(wigg.FindWeapons());
        }
        else if (other.IsTouching(chestsCollider) && other.name != null && other.name.Equals("Player"))
        {
            chestsCollider.enabled = false;
            StartCoroutine(wigg.FindChests());
        }
        else if (other.IsTouching(monsterCollider) && other.name != null && other.name.Equals("Player"))
        {
            monsterCollider.enabled = false;
            StartCoroutine(wigg.FindMonster());
        }
        else if (other.IsTouching(monstersCollider) && other.name != null && other.name.Equals("Player"))
        {
            monstersCollider.enabled = false;
            StartCoroutine(wigg.FindMonsters());
        }
        else if (other.IsTouching(specialCollider) && other.name != null && other.name.Equals("Player"))
        {
            specialCollider.enabled = false;
            StartCoroutine(wigg.FindSpecial());
        }
    }
}
