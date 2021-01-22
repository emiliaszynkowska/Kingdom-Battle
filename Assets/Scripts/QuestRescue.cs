using UnityEngine;

public class QuestRescue : Quest
{
    public GameObject alert;
    public GameObject obj1;
    public GameObject obj2;
    public GameObject obj3;

    void Update()
    {
        if (!obj1.activeSelf && !obj2.activeSelf && !obj3.activeSelf)
        {
            alert.SetActive(false);
            complete = true;
        }
    }
    
}