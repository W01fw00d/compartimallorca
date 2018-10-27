using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Expirable : 
    MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    public int n_seatSlots = 3;
    public float expirates_in = 60.0f;

    private Text timerText;
    private float timerColorChangeLimit = 10.0f;
    private Color red = new Color(0.8f, 0, 0, 1);

    private Color originalColor;
    //private Color mouseOverColor = new Color(1, 0.53f, 0, 1); //orange
    private Color mouseOverColor = new Color(1, 1, 0, 1); //yellow


    //private int[,] complex_route = new int[,] {
    //    { 1, 2 },
    //    { 3, 4 },
    //    { 5, 6 },
    //    { 7, 8 }
    //};

    void Start() {
        originalColor = GetComponent<Image>().color;

        Transform panel = gameObject.transform.Find("CharacterAvatar");

        Transform slots_wrapper = panel.Find("SeatsSlotsWrapper");
        timerText = panel.Find("TimerText").gameObject.GetComponent<Text>();

        for (int i = 1; i <= n_seatSlots; i++)
        {
            slots_wrapper.transform.Find("SeatSlot" + i).gameObject.SetActive(true);
        }
    }

    void Update () {
        updateTimerText();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Image>().color = mouseOverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Image>().color = originalColor;
    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        if (!collider.gameObject.GetComponent<Draggable>().dragging)
        {
            Debug.Log(collider.gameObject.transform.FindObjectsWithTag("CharacterAvatar").Count);

            var colliderCardAvatar =
                collider.gameObject.transform.FindObjectsWithTag("CharacterAvatar")[0].gameObject.GetComponent<Image>().sprite;
            transform.FindObjectsWithTag("SeatSlot")[0].gameObject.GetComponent<Image>().sprite = colliderCardAvatar;

            launch();
        } else
        {
            GetComponent<Image>().color = mouseOverColor;
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        GetComponent<Image>().color = originalColor;
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
            timerText.text = expirates_in.ToString("0") + " s";

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
}
