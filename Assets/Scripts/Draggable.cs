using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

class Draggable : 
    MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public LineDrawer lineDrawer;

    public SimpleRoute simpleRoute;
    public bool dragging = false;

    private Color mouseOverColor = new Color(0.4810431f, 0.530507f, 0.8867924f, 1); //violet
    private Color originalColor;

    private float xOriginal;
    private float yOriginal;
    //private float zOriginal;
    private float distance;
    private GameObject carCard;

    public bool cardActive = false;

    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        ResetCard();
    }

    public void Update()
    {
        if (dragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            transform.position = rayPoint;

        }

        //else if (transform.position.z != zOriginal)
        //{
        //    ResetPosition();
        //}
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!dragging)
        {
            GetComponent<Image>().color = mouseOverColor;

            lineDrawer.DrawPassengerRoute(
                simpleRoute
            );
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!dragging)
        {
            GetComponent<Image>().color = originalColor;

            lineDrawer.ClearPassengerRoute(
                simpleRoute
            );
        }  
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
        //MoveToFront();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;

        if (carCard && carCard.GetComponent<Expirable>().TryEnterCar(gameObject))
        {
            gameObject.SetActive(false);
            cardActive = false;
            gameManager.PaintBackgroundByInactivePassengerCards();
            gameManager.ShowGameOverTextByInactivePassengerCards();
            lineDrawer.ClearPassengerRoute(
                simpleRoute
            );
        }

        carCard = null;
        ResetPosition();
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        TryToLink(collider.gameObject);
    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        TryToLink(collider.gameObject);
    }

    private void TryToLink(GameObject colliderGameObject)
    {
        if (carCard == null && colliderGameObject.GetComponent<Expirable>())
        {
            if (colliderGameObject.GetComponent<Expirable>().Link(gameObject))
            {
                carCard = colliderGameObject;
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (carCard != null && carCard == collider.gameObject)
        {
            carCard.GetComponent<Expirable>().UnlinkAny();
            carCard = null;
        }
    }

    public void Fall()
    {
        AnimateMovement(1);
    }

    public void FadeIn()
    {
        gameObject.GetComponent<Image>().CrossFadeAlpha(0.0f, 0.0f, true);
        gameObject.GetComponent<Image>().CrossFadeAlpha(1.0f, 1.0f, true);

        FadeInChildrenByName("Panel");
        FadeInChildrenByName("CharacterAvatar");
        FadeInCharacterAvatarTextChildrenByName("CharacterText");
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

    private void FadeInRouteTextChildrenByName(string childrenName)
    {
        gameObject.transform.Find("Route").transform.Find(childrenName).GetComponent<Text>().CrossFadeAlpha(0.0f, 0.0f, true);
        gameObject.transform.Find("Route").transform.Find(childrenName).GetComponent<Text>().CrossFadeAlpha(1.0f, 1.0f, true);
    }

    private void ResetCard()
    {
        originalColor = GetComponent<Image>().color;
        xOriginal = transform.position.x;
        yOriginal = transform.position.y;
        //zOriginal = transform.position.z;
    }

    private void ResetPosition()
    {
        var oldPosition = transform.position;
        transform.position = new Vector3(xOriginal, yOriginal, 0.0f);
        //transform.SetPositionAndRotation(new Vector3(xOriginal, yOriginal, 0), new Quaternion());
    }

    private void AnimateMovement(int gravity)
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = gravity;
        Invoke("ResetObject", 5.0f);
    }

    private void ResetObject()
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
        gameObject.SetActive(false);
        cardActive = false;
        carCard = null;
        ResetPosition();
    }

    //private void MoveToFront()
    //{
    //    var oldPosition = transform.position;
    //    transform.position = new Vector3(oldPosition.x, oldPosition.y, 10.0f);
    //}
}
