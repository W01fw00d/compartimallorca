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

    public Sprite blankSprite;
    public LineDrawer lineDrawer;

    public ComplexRoute complexRoute;
    public int n_seatSlots = 3;
    public float expirates_in = 60.0f;

    private Color red = new Color(0.8f, 0, 0, 1);
    private Color originalColor = new Color(1, 1, 1, 1); //White
    private Color mouseOverColor = new Color(1, 1, 0, 1); //yellow
    private Color correctLinkColor = new Color(0, 1, 0.5f, 1); //green
    private Color incorrectLinkColor = new Color(0.8f, 0, 0, 1); //red

    private Text timerText;
    private float timerColorChangeLimit = 10.0f;
    private GameObject passengerCard;
    private bool isPassengerCardRouteMatching = false;

    //private int points;
    //private int expiration_time;
    //private int n_slots;
    //private int occupied_n_slots = 0;
    //private int max_n_slots = 3;

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

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Image>().color = mouseOverColor;
        DrawRoute();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Image>().color = originalColor;
        ClearRoute();
    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        if (
            !passengerCard &&
            collider.gameObject.GetComponent<Draggable>() &&
            collider.gameObject.GetComponent<Draggable>().IsLinked(gameObject)
        )
        {
            Link(collider.gameObject);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        GetComponent<Image>().color = originalColor;
        ClearRoute();
        passengerCard = null;
        isPassengerCardRouteMatching = false;
    }

    public bool Link(GameObject linkingGameObject)
    {
        if (!passengerCard)
        {
            passengerCard = linkingGameObject;

            isPassengerCardRouteMatching = ContainsSimpleRoute(
                passengerCard.GetComponent<Draggable>().simpleRoute
            );

            GetComponent<Image>().color = isPassengerCardRouteMatching ?
                correctLinkColor :
                incorrectLinkColor;
            DrawRoute();

            return true;
        }

        return false;
    }

    public bool TryEnterCar(GameObject passengerCard)
    {
        if (CanEnterCar(passengerCard.GetComponent<Draggable>().simpleRoute))
        {
            var colliderCardAvatar =
                passengerCard.transform.FindObjectsWithTag("CharacterAvatar")[0].gameObject.GetComponent<Image>().sprite;

            GameObject seatSlot = transform.FindObjectsWithTag("SeatSlot")[0].gameObject;

            seatSlot.gameObject.GetComponent<Image>().sprite = colliderCardAvatar;
            seatSlot.tag = "OccupiedSeat";

            gameObject.GetComponent<Expirable>().Launch();

            return true;
        }
        else
        {
            return false;
        }
    }

    private void DrawRoute()
    {
        lineDrawer.DrawCarRoute(
            complexRoute.OriginRoute.Origin,
            complexRoute.OriginRoute.Destination,
            complexRoute.DestinationRoute.Destination
        );
    }

    private void ClearRoute()
    {
        lineDrawer.ClearCarRoute(
            complexRoute.OriginRoute.Origin,
            complexRoute.OriginRoute.Destination,
            complexRoute.DestinationRoute.Destination
        );
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

        List<GameObject> occupiedSeats = gameObject.transform.FindObjectsWithTag("OccupiedSeat");
        foreach (GameObject occupiedSeat in occupiedSeats)
        {
            occupiedSeat.tag = "SeatSlot";
            occupiedSeat.GetComponent<Image>().sprite = blankSprite;
        }

        List<GameObject> seatSlots = gameObject.transform.FindObjectsWithTag("SeatSlot");
        foreach (GameObject seatSlot in seatSlots)
        {
            seatSlot.SetActive(true);
        }
    }

    private void Launch()
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

    private bool CanEnterCar(SimpleRoute simpleRoute)
    {
        return AreSeatsAvailable() && isPassengerCardRouteMatching;
    }

    private bool AreSeatsAvailable()
    {
        return transform.FindObjectsWithTag("SeatSlot").Count > 0;
    }

    private bool ContainsSimpleRoute(SimpleRoute simpleRoute)
    {
        // We really need to set ids for those simpleRoutes...
        return
            (
                complexRoute.OriginRoute.Origin == simpleRoute.Origin &&
                complexRoute.OriginRoute.Destination == simpleRoute.Destination
            ) ||
            (
                complexRoute.DestinationRoute.Origin == simpleRoute.Origin &&
                complexRoute.DestinationRoute.Destination == simpleRoute.Destination
            );
    }
}
