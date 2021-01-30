using System.Collections.Generic;
using UnityEngine;

public class ChestController : MonoBehaviour
{
    public GameObject player;
    public DungeonManager dungeonManager;
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
            ScoreManager.AddCollection(1);
            opened = true;
            animator.SetTrigger("Open");
            dungeonManager.PlaceItem(item, new Vector3(transform.position.x, transform.position.y + 1, 0));
            return true;
        }
        return false;
    }
    
}
