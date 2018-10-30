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

    private GameObject passengerCard;

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
        UpdateTimerText();
    }

    public bool Link(GameObject gameObject)
    {
        if (!passengerCard)
        {
            passengerCard = gameObject;
            GetComponent<Image>().color = mouseOverColor; //Use special color for linking of two cards?

            return true;
        }

        return false;
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
        if (
            !passengerCard &&
            collider.gameObject.GetComponent<Draggable>() &&
            collider.gameObject.GetComponent<Draggable>().IsLinked(gameObject)
        ) {
            Link(collider.gameObject);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        GetComponent<Image>().color = originalColor;

        passengerCard = null;
    }

    private void UpdateTimerText()
    {
        if (expirates_in <= 0.0f)
        {
            Fall();

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

    private void ResetObject()
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        gameObject.SetActive(false);
    }

    public void Launch()
    {
        AnimateMovement(-1);
    }

    private void Fall()
    {
        AnimateMovement(1);
    }

    private void AnimateMovement(int gravity)
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = gravity;
        Invoke("ResetObject", 10.0f);
    }
}
