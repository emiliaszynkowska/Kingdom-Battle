using UnityEngine;

public class DoorController : MonoBehaviour
{
    public GameObject player;
    public UIManager uiManager;
    public BoxCollider2D boxCollider;
    public CapsuleCollider2D capsuleCollider;
    public SpriteRenderer doorRenderer;
    public Sprite closedDoor;
    public Sprite openDoor;
    public bool opened;

    void Start()
    {
        player = GameObject.Find("Player");
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        boxCollider = GetComponent<BoxCollider2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        doorRenderer = GetComponent<SpriteRenderer>();
    }
    
    public bool Open()
    {
        if (!opened)
        {
            opened = true;
            doorRenderer.sprite = openDoor;
            boxCollider.enabled = false;
            capsuleCollider.enabled = false;
            return true;
        }
        return false;
    }
    
}