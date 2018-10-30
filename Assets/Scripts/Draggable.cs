using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

class Draggable : 
    MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private float xOriginal;
    private float yOriginal;
    private float zOriginal;

    //private Color mouseOverColor = new Color(0, 1, 0.5f, 1); //green
    private Color mouseOverColor = new Color(0.4810431f, 0.530507f, 0.8867924f, 1); //violet
    private Color originalColor;
    public bool dragging = false;
    private float distance;

    public LineDrawer lineDrawer;

    private GameObject carCard;

    void Start()
    {
        ResetCard();
    }

    private void ResetCard()
    {
        originalColor = GetComponent<Image>().color;
        xOriginal = transform.position.x;
        yOriginal = transform.position.y;
        zOriginal = transform.position.z;

        Debug.Log("ResetCard");
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
                gameObject.GetComponent<PassengerRouteCard>().simpleRoute
            );
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!dragging)
        {
            GetComponent<Image>().color = originalColor;

            lineDrawer.ClearPassengerRoute(
                gameObject.GetComponent<PassengerRouteCard>().simpleRoute
            );
        }  
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //if (GameObject.Find("GameManager").GetComponent<GameManager>().DraggedCard == null)
        //{
            distance = Vector3.Distance(transform.position, Camera.main.transform.position);
            dragging = true;
            MoveToFront();

            //GameObject.Find("GameManager").GetComponent<GameManager>().DraggedCard = gameObject;
        //}
    }

    private void MoveToFront()
    {
        var oldPosition = transform.position;
        transform.position = new Vector3(oldPosition.x, oldPosition.y, 10.0f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
        //GameObject.Find("GameManager").GetComponent<GameManager>().DraggedCard = null;

        if (carCard && carCard.GetComponent<CarRouteCard>().TryEnterCar(gameObject))
        {
            gameObject.SetActive(false);
        }

        //Invoke("ResetPosition", 2.0f);
        ResetPosition();
    }

    private void ResetPosition()
    {
        var oldPosition = transform.position;
        transform.position = new Vector3(xOriginal, yOriginal, 0.0f);
        //transform.SetPositionAndRotation(new Vector3(xOriginal, yOriginal, 0), new Quaternion());
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
}

public static class TransformExtensions
{
    public static List<GameObject> FindObjectsWithTag(this Transform parent, string tag)
    {
        List<GameObject> taggedGameObjects = new List<GameObject>();

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.tag == tag)
            {
                taggedGameObjects.Add(child.gameObject);
            }
            if (child.childCount > 0)
            {
                taggedGameObjects.AddRange(FindObjectsWithTag(child, tag));
            }
        }
        return taggedGameObjects;
    }
}
