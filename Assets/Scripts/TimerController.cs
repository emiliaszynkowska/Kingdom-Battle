using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimerController : MonoBehaviour {

    private bool canAttack;
    public Image fill;
    public Image icon;
    public Text timeText;
    public string text;
    public int time;
    private int totalTime;

    void Start ()
    {
        Reset();
    }

    public void Reset()
    {
        canAttack = false;
        timeText.text = time.ToString();
        totalTime += time;
        icon.canvasRenderer.SetAlpha(0.5f);
        StartCoroutine(Second());
    }

    void Update ()
    {
        if (totalTime == 0)
        {
            canAttack = true;
            fill.fillAmount = 1;
            timeText.text = text;
            icon.canvasRenderer.SetAlpha(1);
        }
    }

    public bool CanAttack()
    {
        return canAttack;
    }
    
    IEnumerator Second()
    {
        while (totalTime > 0)
        {
            yield return new WaitForSeconds(1);
            FillLoading();
            timeText.text = totalTime.ToString();
        }
    }

    void FillLoading()
    {
        totalTime--;
        float fillAmount = (float)totalTime/time;
        fill.fillAmount = fillAmount;
    }
}