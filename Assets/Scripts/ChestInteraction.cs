using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    private GameObject player;
    private UIUpdater uiUpdater;
    private Animator animator;
    private string item;
    private string[] items = new[] {"key", "scroll", "potionred", "potionyellow", "potiongreen", "potionblue", "coins"};
    
    void Start()
    {
        player = GameObject.Find("Player");
        uiUpdater = GameObject.Find("UI").GetComponent<UIUpdater>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if ((player.transform.position - transform.position).sqrMagnitude <= 1)
        {
            uiUpdater.SetInteract(true);
        }
    }

    void SetItem(string i)
    {
        if (i.Equals("random"))
            item = items[Random.Range(0, 6)];
        else
            item = i;
    }

    public void Open()
    {
        animator.SetTrigger("Open");
    }

    public void Close()
    {
        animator.SetTrigger("Close");
    }
    
}
