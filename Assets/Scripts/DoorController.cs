using UnityEngine;

public class DoorController : MonoBehaviour
{
    public GameObject player;
    public UIManager uiManager;
    public SoundManager soundManager;
    public BoxCollider2D boxCollider;
    public CapsuleCollider2D capsuleCollider;
    public SpriteRenderer doorRenderer;
    public Sprite closedDoor;
    public Sprite openDoor;
    public bool opened;
    public bool boss;

    void Start()
    {
        player = GameObject.Find("Player");
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        soundManager = GameObject.Find("Main Camera").GetComponent<SoundManager>();
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
            soundManager.PlaySound(soundManager.open);
            ScoreManager.AddPuzzleSolving(1);
            ScoreManager.AddExploration(1);
            if (boss)
            {
                soundManager.PlaySound(soundManager.win);
                uiManager.LevelNext();
            }
            return true;
        }
        return false;
    }
    
}