using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject pointsText;
    public GameObject pointsAddedText;
    private GameObject draggedCard;
    private int totalPoints;

    public GameObject DraggedCard
    {
        get
        {
            return draggedCard;
        }

        set
        {
            draggedCard = value;
        }
    }

    public int TotalPoints
    {
        get
        {
            return totalPoints;
        }

        set
        {
            totalPoints = value;
        }
    }

    public void AddPoints(int points)
    {
        totalPoints += points;
        pointsText.GetComponent<Text>().text = "Punts: " + totalPoints.ToString("D5");

        pointsAddedText.GetComponent<Text>().text = "+" + points;
        pointsAddedText.SetActive(true);
        Invoke("HidePointsAddedText", 5.0f);
    }

    private void HidePointsAddedText()
    {
        pointsAddedText.SetActive(false);
    }

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
