using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Expirable : MonoBehaviour {
    public int n_seatSlots = 3;
    public float expirates_in = 60.0f;

    private Text timerText;
    private float timerColorChangeLimit = 10.0f;
    private Color red = new Color(0.8f, 0, 0, 1);

    //private int[,] complex_route = new int[,] {
    //    { 1, 2 },
    //    { 3, 4 },
    //    { 5, 6 },
    //    { 7, 8 }
    //};

    public void launch()
    {
        animateMovement(-1);
    }

    private void fall()
    {
        animateMovement(1);
    }

    private void animateMovement(int gravity)
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = gravity;
        Invoke("resetObject", 10.0f);
    }

    void Start() {
        GameObject slots_wrapper = gameObject.transform.Find("SeatsSlotsWrapper").gameObject;
        timerText = gameObject.transform.Find("TimerText").gameObject.GetComponent<Text>();

        for (int i = 1; i <= n_seatSlots; i++)
        {
            slots_wrapper.transform.Find("SeatSlot" + i).gameObject.SetActive(true);
        }
    }

    void Update () {
        updateTimerText();
    }

    private void updateTimerText()
    {
        if (expirates_in <= 0.0f)
        {
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 1;
            Invoke("resetObject", 10.0f);

        } else
        {
            expirates_in -= Time.deltaTime;
            timerText.text = expirates_in.ToString("0") + "s";

            if (expirates_in < timerColorChangeLimit)
            {
                timerText.color = red;
            }
        }
    }

    private void resetObject()
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        gameObject.SetActive(false);
    }
}
