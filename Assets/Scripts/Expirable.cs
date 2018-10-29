using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Expirable : 
    MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    //private float xOriginal;
    //private float yOriginal;

    public int n_seatSlots = 3;
    public float expirates_in = 60.0f;

    private Text timerText;
    private float timerColorChangeLimit = 10.0f;
    private Color red = new Color(0.8f, 0, 0, 1);

    private Color originalColor = new Color(1, 1, 1, 1); //White
    //private Color mouseOverColor = new Color(1, 0.53f, 0, 1); //orange
    private Color mouseOverColor = new Color(1, 1, 0, 1); //yellow

    public LineDrawer lineDrawer;

    void Start() {
        //xOriginal = transform.position.x;
        //yOriginal = transform.position.y;

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

        ComplexRoute complexRoute = gameObject.GetComponent<CarRouteCard>().complexRoute;

        lineDrawer.DrawCarRoute(
            complexRoute.OriginRoute.Origin,
            complexRoute.OriginRoute.Destination,
            complexRoute.DestinationRoute.Destination
        );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Image>().color = originalColor;

        ComplexRoute complexRoute = gameObject.GetComponent<CarRouteCard>().complexRoute;

        lineDrawer.ClearCarRoute(
            complexRoute.OriginRoute.Origin,
            complexRoute.OriginRoute.Destination,
            complexRoute.DestinationRoute.Destination
        );
    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Draggable>())
        {
            if (!collider.gameObject.GetComponent<Draggable>().dragging)
            {
                var colliderCardAvatar =
                    collider.gameObject.transform.FindObjectsWithTag("CharacterAvatar")[0].gameObject.GetComponent<Image>().sprite;
                transform.FindObjectsWithTag("SeatSlot")[0].gameObject.GetComponent<Image>().sprite = colliderCardAvatar;

                launch();
            }
            else
            {
                GetComponent<Image>().color = mouseOverColor;
            }
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
