using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CardFactory : MonoBehaviour {

    //private int nCarCardsLimit = 5;
    //private int nPassengerCardsLimit = 5;

    public Sprite blankSprite;
    public List<Sprite> sprites;
    private List<Sprite> freeSprites;

    private GameObject[] passengerCards;

    void Start () {
        freeSprites = sprites;

        TrackInactiveTaggedCards("PassengerCard");

        //Debug
        CreatePassengerCard();
        CreatePassengerCard();
        CreatePassengerCard();
    }

    private void TrackInactiveTaggedCards(string tag)
    {
        passengerCards = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject passengerCard in passengerCards)
        {
            passengerCard.SetActive(false);
        }
    }

    private void CreatePassengerCard()
    {
        GameObject newCard = GetFirstInactivePassengerCard();

        if (newCard)
        {
            CreatePassengerCardGameObject(newCard);

        } else
        {
            Debug.Log("Cannot find any inactive card; cannot create a new passenger card");
        }
    }

    private void CreatePassengerCardGameObject(GameObject newCard)
    {
        GameObject characterAvatar = newCard.gameObject.transform.FindObjectsWithTag("CharacterAvatar")[0];
        characterAvatar.gameObject.GetComponent<Image>().sprite = GetUnusedRandomCharacterAvatar();

        newCard.gameObject.SetActive(true);
    }

    private Sprite GetUnusedRandomCharacterAvatar()
    {
        Sprite sprite = blankSprite;

        if (freeSprites.Count > 0)
        {
            int sprite_index = (int) Math.Round(UnityEngine.Random.value * (freeSprites.Count - 1));
            sprite = freeSprites[sprite_index];
            freeSprites.RemoveAt(sprite_index);
        }

        return sprite;
    }

    private GameObject GetFirstInactivePassengerCard()
    {
        List<GameObject> inactiveCards = new List<GameObject>();

        foreach (GameObject card in passengerCards)
        {
            if (!card.activeSelf)
            {
                inactiveCards.Add(card);
            }
        }

        if (inactiveCards.Count > 0)
        {
            return inactiveCards[0];
        } else
        {
            return null;
        }
    }
}
