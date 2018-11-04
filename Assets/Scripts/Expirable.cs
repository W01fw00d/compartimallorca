using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Expirable : 
    MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    private float xOriginal;
    private float yOriginal;

    public Sprite blankSprite;
    public LineDrawer lineDrawer;

    public ComplexRoute complexRoute;
    public int n_seatSlots = 3;

    public bool cardActive = false;

    private float expirationLimit;
    public float expires_in;

    private Color black = new Color(0, 0, 0, 1);
    private Color red = new Color(0.8f, 0, 0, 1);
    private Color originalColor;
    private Color mouseOverColor = new Color(1, 1, 0, 1); //yellow
    private Color correctLinkColor = new Color(0, 1, 0.5f, 1); //green
    private Color incorrectLinkColor = new Color(0.8f, 0, 0, 1); //red

    private Text timerText;
    private float timerColorChangeLimit = 10.0f;
    private GameObject passengerCard;
    private bool isPassengerCardRouteMatching = false;
    private GameManager gameManager;
    private CardFactory cardFactory;

    private AudioSource carOnAudio;
    private AudioSource carOffAudio;

    public int basePoints;

    void Start()
    {
        originalColor = GetComponent<Image>().color;

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        cardFactory = GameObject.Find("CardFactory").GetComponent<CardFactory>();

        carOnAudio = GameObject.Find("CarOnAudio").GetComponent<AudioSource>();
        carOffAudio = GameObject.Find("CarOffAudio").GetComponent<AudioSource>();

        xOriginal = transform.position.x;
        yOriginal = transform.position.y;

        Transform panel = gameObject.transform.Find("CharacterAvatar");
        timerText = panel.Find("TimerText").gameObject.GetComponent<Text>();
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

    public bool Link(GameObject linkingGameObject)
    {
        if (cardActive)
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

    public void UnlinkAny()
    {
        GetComponent<Image>().color = originalColor;
        ClearRoute();
        passengerCard = null;
        isPassengerCardRouteMatching = false;
    }

    public bool TryEnterCar(GameObject passengerCard)
    {
        if (cardActive && CanEnterCar(passengerCard.GetComponent<Draggable>().simpleRoute))
        {
            var colliderCardAvatar =
                passengerCard.transform.FindObjectsWithTag("CharacterAvatar")[0].gameObject.GetComponent<Image>().sprite;

            GameObject emptySeat = transform.FindObjectsWithTag("EmptySeat")[0].gameObject;

            emptySeat.gameObject.GetComponent<Image>().sprite = colliderCardAvatar;
            cardFactory.AddFreeSprite(colliderCardAvatar);

            emptySeat.tag = "OccupiedSeat";

            UnlinkAny();

            if (!ExistEmptySeats())
            {
                cardFactory.AddFreeSprite(gameObject.transform.FindObjectsWithTag("CharacterAvatar")[0].gameObject.GetComponent<Image>().sprite);
                LaunchCar(true);
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    public void FadeIn()
    {
        gameObject.GetComponent<Image>().CrossFadeAlpha(0.0f, 0.0f, true);
        gameObject.GetComponent<Image>().CrossFadeAlpha(1.0f, 1.0f, true);

        FadeInChildrenByName("Panel");
        FadeInChildrenByName("CharacterAvatar");
        FadeInCharacterAvatarTextChildrenByName("CharacterText");
        FadeInCharacterAvatarTextChildrenByName("TimerText");


        FadeInRouteTextChildrenByName("FromText");
        FadeInRouteTextChildrenByName("ArrowFakeIcon");
        FadeInRouteTextChildrenByName("ToText");
    }

    private void FadeInChildrenByName(string childrenName)
    {
        gameObject.transform.Find(childrenName).GetComponent<Image>().CrossFadeAlpha(0.0f, 0.0f, true);
        gameObject.transform.Find(childrenName).GetComponent<Image>().CrossFadeAlpha(1.0f, 1.0f, true);
    }

    private void FadeInCharacterAvatarTextChildrenByName(string childrenName)
    {
        gameObject.transform.Find("CharacterAvatar").transform.Find(childrenName).GetComponent<Text>().CrossFadeAlpha(0.0f, 0.0f, true);
        gameObject.transform.Find("CharacterAvatar").transform.Find(childrenName).GetComponent<Text>().CrossFadeAlpha(1.0f, 1.0f, true);
    }

    private void FadeInSeatSlots()
    {
        Transform seatsSlotsWrapper = gameObject.transform.Find("CharacterAvatar").Find("SeatsSlotsWrapper");

        seatsSlotsWrapper.Find("SeatSlot1").GetComponent<Image>().CrossFadeAlpha(0.0f, 0.0f, true);
        seatsSlotsWrapper.Find("SeatSlot2").GetComponent<Image>().CrossFadeAlpha(0.0f, 0.0f, true);
        seatsSlotsWrapper.Find("SeatSlot3").GetComponent<Image>().CrossFadeAlpha(0.0f, 0.0f, true);
    }

    private void FadeInRouteTextChildrenByName(string childrenName)
    {
        gameObject.transform.Find("Route").transform.Find(childrenName).GetComponent<Text>().CrossFadeAlpha(0.0f, 0.0f, true);
        gameObject.transform.Find("Route").transform.Find(childrenName).GetComponent<Text>().CrossFadeAlpha(1.0f, 1.0f, true);
    }

    private void LaunchCar(bool isCompleted)
    {
        int points = GetPoints(isCompleted);

        //Debug.Log("Total Points");
        //Debug.Log(points);
        //Debug.Log("------------");

        gameManager.AddPoints(
            points
        );

        Launch();
    }

    private int GetPoints(bool isCarCompleted)
    {
        int basePoints = 200;
        int bonusForCompletion = isCarCompleted ? 2 : 1; // More points if all seats are occupied
        int bonusForSpeed = isCarCompleted ? 
            (int)(expires_in / expirationLimit * 100) : // More points if card was managed quickly
            0;

        //Debug.Log("Seats Points");
        //Debug.Log(basePoints * transform.FindObjectsWithTag("OccupiedSeat").Count);
        //Debug.Log("With Completion bonus");
        //Debug.Log(basePoints * bonusForCompletion * transform.FindObjectsWithTag("OccupiedSeat").Count);
        //Debug.Log("Expiration Time Limit");
        //Debug.Log(-(int)expirationLimit);
        //Debug.Log("Expiration Time left");
        //Debug.Log(-(int)expires_in);

        return
            (basePoints * bonusForCompletion * transform.FindObjectsWithTag("OccupiedSeat").Count)
            //- (int)expirationLimit // More points for cards with little expiration max time
            + bonusForSpeed
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
                carOffAudio.Play();
            }

            cardFactory.AddFreeSprite(gameObject.transform.FindObjectsWithTag("CharacterAvatar")[0].gameObject.GetComponent<Image>().sprite);
            UnlinkAny();

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
        UnlinkAny();
        gameObject.SetActive(false);

        List<GameObject> occupiedSeats = gameObject.transform.FindObjectsWithTag("OccupiedSeat");
        foreach (GameObject occupiedSeat in occupiedSeats)
        {
            occupiedSeat.tag = "EmptySeat";
            occupiedSeat.GetComponent<Image>().sprite = blankSprite;
        }

        List<GameObject> blockedSeatSlots = gameObject.transform.FindObjectsWithTag("BlockedSeat");
        foreach (GameObject blockedSeatSlot in blockedSeatSlots)
        {
            blockedSeatSlot.tag = "EmptySeat";
            blockedSeatSlot.SetActive(true);
        }

        ResetPosition();
    }

    private void ResetPosition()
    {
        var oldPosition = transform.position;
        transform.position = new Vector3(xOriginal, yOriginal, 0.0f);
    }

    private void Launch()
    {
        AnimateMovement(-1);
        carOnAudio.Play();
    }

    public void Fall()
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
