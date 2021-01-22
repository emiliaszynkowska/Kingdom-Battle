using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : MonoBehaviour
{
    public GameObject player;
    public PlayerController playerController;
    public UIManager uiManager;
    public string npcName;
    public string message;
    public bool active = false;
    public bool complete = false;
    public bool talking = false;
    
    void Start()
    {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        List<string> npcNames = new List<string>() {"Rhys", "Samuel", "Lucius", "Jedediah", "Matthew", "David"};
        npcName = npcNames[Random.Range(0, 6)];
    }

    public IEnumerator Talk()
    {
        if (!talking)
        {
            if (active && complete)
            {
                talking = true;
                message = "Thank you for helping me!";
                uiManager.StartSpeak(npcName, message);
                yield return new WaitForSeconds(0.1f);
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                uiManager.StopSpeak();
                StartCoroutine(Disappear());
            }
            else
            {
                talking = true;
                uiManager.StartSpeak(npcName, message);
                yield return new WaitForSeconds(0.1f);
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));
                uiManager.StopSpeak();
                talking = false;
                active = true;
            }
        }
    }

    IEnumerator Disappear()
    {
        for (int i=0; i < 3000; i++)
        {
            transform.Translate(Vector3.right * (Time.deltaTime * 2));
            yield return null;
        }
        gameObject.SetActive(false);
    }
    
}