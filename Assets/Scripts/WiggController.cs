using System.Collections;
using Assets.Scripts;
using UnityEngine;

public class WiggController : MonoBehaviour
{
    public GameObject player;
    public PlayerController playerController;
    public UIManager uiManager;
    public string npcName = "Wigg";
    public string message;
    public bool talking = false;
    
    public void Start()
    {
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
    }
    
    public void FixedUpdate()
    {
        if (player.transform.position.x - transform.position.x >= 0)
            GetComponent<SpriteRenderer>().flipX = false;
        else
            GetComponent<SpriteRenderer>().flipX = true;
    }
        
    IEnumerator Disappear()
    {
        for (int i=0; i < 3000; i++)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            transform.Translate(Vector3.right * (Time.deltaTime * 2));
            yield return null;
        }
        gameObject.SetActive(false);
    }
}