using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public GameObject player;
    public DungeonManager dungeonManager;
    public SoundManager soundManager;
    public UIManager uiManager;
    public Animator animator;
    public string item;
    public bool opened;
    public List<string> items = new List<string>() {"Coins", "Coins", "Wigg's Brew", "Liquid Luck", "Ogre's Strength", "Elixir of Speed"};
    
    void Start()
    {
        player = GameObject.Find("Player");
        dungeonManager = GameObject.Find("Map").GetComponent<DungeonManager>();
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        soundManager = GameObject.Find("Main Camera").GetComponent<SoundManager>();
        animator = GetComponent<Animator>();
        SetItem("random");
    }

    public void SetItem(string i)
    {
        if (i.Equals("random"))
            item = items[Random.Range(0, 6)];
        else
            item = i;
    }

    public bool Open()
    {
        if (!opened)
        {
            opened = true;
            soundManager.PlaySound(soundManager.open);
            animator.SetTrigger("Open");
            dungeonManager.PlaceItem(item, new Vector3(transform.position.x, transform.position.y + 1, 0));
            return true;
        }
        return false;
    }
    
}
