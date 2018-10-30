using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    private GameObject draggedCard;

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
