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

    //private int points;

    void Start()
    {
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
        //if (GameObject.Find("GameManager").GetComponent<GameManager>().DraggedCard == null)
        //{
            distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            dragging = true;
            //MoveToFront();

            //GameObject.Find("GameManager").GetComponent<GameManager>().DraggedCard = gameObject;
        //}
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
        //GameObject.Find("GameManager").GetComponent<GameManager>().DraggedCard = null;

        if (carCard && carCard.GetComponent<Expirable>().TryEnterCar(gameObject))
        {
            gameObject.SetActive(false);
            lineDrawer.ClearPassengerRoute(
                simpleRoute
            );
        }

        //Invoke("ResetPosition", 2.0f);
        ResetPosition();
    }


    public void OnTriggerStay2D(Collider2D collider)
    {
        if (!carCard && collider.gameObject.GetComponent<Expirable>())
        {
            carCard = collider.gameObject;
            carCard.GetComponent<Expirable>().Link(gameObject);

            //We could do the TryEnterCar here and feedback the user about it
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        carCard = null;
    }

    public bool IsLinked(GameObject gameObject)
    {
        return carCard == gameObject;
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

    //private void MoveToFront()
    //{
    //    var oldPosition = transform.position;
    //    transform.position = new Vector3(oldPosition.x, oldPosition.y, 10.0f);
    //}
}
