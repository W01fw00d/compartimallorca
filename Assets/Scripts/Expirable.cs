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

    public bool cardActive = false;

    private float expirationLimit;
    public float expires_in;

    private Color black = new Color(0, 0, 0, 1);
    private Color red = new Color(0.8f, 0, 0, 1);
    private Color originalColor = new Color(1, 1, 1, 1); //White
    private Color mouseOverColor = new Color(1, 1, 0, 1); //yellow
    private Color correctLinkColor = new Color(0, 1, 0.5f, 1); //green
    private Color incorrectLinkColor = new Color(0.8f, 0, 0, 1); //red

    private Text timerText;
    private float timerColorChangeLimit = 10.0f;
    private GameObject passengerCard;
    private bool isPassengerCardRouteMatching = false;

    public int basePoints;
    //private int expiration_time;
    //private int n_slots;
    //private int occupied_n_slots = 0;
    //private int max_n_slots = 3;

    void Start() {
        //xOriginal = transform.position.x;
        //yOriginal = transform.position.y;

        Transform panel = gameObject.transform.Find("CharacterAvatar");
        timerText = panel.Find("TimerText").gameObject.GetComponent<Text>();

        //Transform slots_wrapper = panel.Find("SeatsSlotsWrapper");
        //for (int i = 1; i <= n_seatSlots; i++)
        //{
        //    slots_wrapper.transform.Find("SeatSlot" + i).gameObject.SetActive(true);
        //}
    }

    void Update () {
        if (gameObject.activeSelf)
        {
            UpdateTimerText();
        }
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

            GameObject emptySeat = transform.FindObjectsWithTag("EmptySeat")[0].gameObject;

            emptySeat.gameObject.GetComponent<Image>().sprite = colliderCardAvatar;
            emptySeat.tag = "OccupiedSeat";

            if (!ExistEmptySeats())
            {
                LaunchCar(true);
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    private void LaunchCar(bool isCompleted)
    {
        int points = GetPoints(isCompleted);

        Debug.Log("Total Points");
        Debug.Log(points);
        Debug.Log("------------");

        GameObject.Find("GameManager").GetComponent<GameManager>().AddPoints(
            points
        );

        Launch();
    }

    private int GetPoints(bool isCarCompleted)
    {
        int basePoints = 200;
        int bonusForCompletion = isCarCompleted ? 2 : 1;

        Debug.Log("Seats Points");
        Debug.Log(basePoints * transform.FindObjectsWithTag("OccupiedSeat").Count);
        Debug.Log("With Completion bonus");
        Debug.Log(basePoints * bonusForCompletion * transform.FindObjectsWithTag("OccupiedSeat").Count);
        Debug.Log("Expiration Time Limit");
        Debug.Log(-(int)expirationLimit);
        Debug.Log("Expiration Time left");
        Debug.Log(-(int)expires_in);

        return
            (basePoints * bonusForCompletion * transform.FindObjectsWithTag("OccupiedSeat").Count)
            - (int)expirationLimit
            - (int)expires_in
        ;

    }

    public void SetExpiresIn(float newExpiresIn)
    {
        expirationLimit = newExpiresIn;
        expires_in = newExpiresIn;

        if (!timerText)
        {
            Transform panel = gameObject.transform.Find("CharacterAvatar");
            timerText = panel.Find("TimerText").gameObject.GetComponent<Text>();
        }

        timerText.color = black;
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
        if (cardActive && expires_in <= 0.0f)
        {
            cardActive = false;

            if (IsAnySeatOccupied())
            {
                LaunchCar(false);
            } else
            {
                Fall();
            }

        } else
        {
            expires_in -= Time.deltaTime;
            timerText.text = expires_in.ToString("0") + " s";

            if (expires_in < timerColorChangeLimit)
            {
                timerText.color = red;
            }
        }
    }

    private void ResetObject()
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        cardActive = false;
        gameObject.SetActive(false);

        List<GameObject> occupiedSeats = gameObject.transform.FindObjectsWithTag("OccupiedSeat");
        foreach (GameObject occupiedSeat in occupiedSeats)
        {
            occupiedSeat.tag = "EmptySeat";
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
        Invoke("ResetObject", 5.0f);
    }

    private bool CanEnterCar(SimpleRoute simpleRoute)
    {
        return ExistEmptySeats() && isPassengerCardRouteMatching;
    }

    private bool ExistEmptySeats()
    {
        return transform.FindObjectsWithTag("EmptySeat").Count > 0;
    }

    private bool IsAnySeatOccupied()
    {
        return transform.FindObjectsWithTag("OccupiedSeat").Count > 0;
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
