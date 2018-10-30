using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarRouteCard : MonoBehaviour
{
    //private int points;
    //private int expiration_time;
    //private int n_slots;
    //private int occupied_n_slots = 0;
    //private int max_n_slots = 3;

    public ComplexRoute complexRoute;

    public bool TryEnterCar(GameObject passengerCard)
    {
        if (AreSeatsAvailable() && ContainsSimpleRoute())
        {
            var colliderCardAvatar =
                passengerCard.transform.FindObjectsWithTag("CharacterAvatar")[0].gameObject.GetComponent<Image>().sprite;
            transform.FindObjectsWithTag("SeatSlot")[0].gameObject.GetComponent<Image>().sprite = colliderCardAvatar;

            gameObject.GetComponent<Expirable>().Launch();

            return true;
        } else
        {
            return false;
        }
    }

    private bool AreSeatsAvailable()
    {
        return true;
    }

    private bool ContainsSimpleRoute()
    {
        return true;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
