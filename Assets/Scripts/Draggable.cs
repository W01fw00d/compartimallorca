using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

class Draggable : 
    MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Color mouseOverColor = new Color(0, 1, 0.5f, 1);
    private Color originalColor;
    private bool dragging = false;
    private float distance;

    void Start()
    {
        originalColor = GetComponent<Image>().color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Image>().color = mouseOverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Image>().color = originalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        distance = Vector3.Distance(transform.position, Camera.main.transform.position);
        dragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }

    public void Update()
    {
        if (dragging)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 rayPoint = ray.GetPoint(distance);
            transform.position = rayPoint;
        }
    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        //Debug.Log("OnTriggerStay2D");

        if (!dragging)
        {
            var thisCardAvatar = transform.FindObjectsWithTag("CharacterAvatar")[0].gameObject.GetComponent<Image>().sprite;
            collider.gameObject.transform.FindObjectsWithTag("SeatSlot")[0].gameObject.GetComponent<Image>().sprite = thisCardAvatar;

            gameObject.SetActive(false);

            collider.gameObject.GetComponent<Expirable>().launch();
        }
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