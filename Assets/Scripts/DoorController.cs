using UnityEngine;

public class DoorController : MonoBehaviour
{
    public GameObject player;
    public UIManager uiManager;
    public SoundManager soundManager;
    public BoxCollider2D boxCollider;
    public SpriteRenderer doorRenderer;
    public Sprite closedDoor;
    public Sprite openDoor;
    public bool opened;
    public bool boss;
    public bool special;

    void Start()
    {
        player = GameObject.Find("Player");
        uiManager = GameObject.Find("UI").GetComponent<UIManager>();
        soundManager = GameObject.Find("Main Camera").GetComponent<SoundManager>();
    }
    
    public bool Open()
    {
        if (!opened)
        {
            opened = true;
            doorRenderer.sprite = openDoor;
            boxCollider.enabled = false;
            soundManager.PlaySound(soundManager.open);
            ScoreManager.AddExploration(1);
            if (boss)
                StartCoroutine(uiManager.Scores(ScoreManager.GetScores()));
            return true;
        }
        return false;
    }

    public bool Boss()
    {
        if (opened && special)
        {
            uiManager.Boss();
            return true;
        }
        else if (!opened && special)
        {
            StartCoroutine(uiManager.Speak("", "It's locked."));
        }
        return false;
    }
    
}