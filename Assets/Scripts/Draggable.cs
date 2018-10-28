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

    //private Color mouseOverColor = new Color(0, 1, 0.5f, 1); //green
    private Color mouseOverColor = new Color(0.4810431f, 0.530507f, 0.8867924f, 1); //violet
    private Color originalColor;
    public bool dragging = false;
    private float distance;

    void Start()
    {
        ResetCard();
    }

    private void ResetCard()
    {
        originalColor = GetComponent<Image>().color;
        xOriginal = transform.position.x;
        yOriginal = transform.position.y;
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
        Invoke("ResetPosition", 2.0f);
    }

    private void ResetPosition()
    {
        transform.SetPositionAndRotation(new Vector3(xOriginal, yOriginal, 0), new Quaternion());
    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        if (!dragging && collider.gameObject.GetComponent<Expirable>())
        {
            gameObject.SetActive(false);
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
