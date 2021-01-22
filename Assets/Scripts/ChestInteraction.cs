using UnityEngine;

public class ChestInteraction : MonoBehaviour
{
    private GameObject player;
    private UIManager uiManager;
    private Animator animator;
    private string item;
    private string[] items = new[] {"key", "scroll", "potionred", "potionyellow", "potiongreen", "potionblue", "coins"};
    
    void Start()
    {
        player = GameObject.Find("Player");
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        animator = GetComponent<Animator>();
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
