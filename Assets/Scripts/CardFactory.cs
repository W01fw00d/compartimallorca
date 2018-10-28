using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Xml;
using System.IO;

public class CardFactory : MonoBehaviour {

    //private int nCarCardsLimit = 5;
    //private int nPassengerCardsLimit = 5;

    public Sprite blankSprite;
    public List<Sprite> sprites;
    private List<Sprite> freeSprites;

    private string simpleRoutesXMLPath = "Assets/Scripts/Content/simple_routes.xml";
    private int simpleRoutesXMLLength = 13;

    private GameObject[] carCards;
    private GameObject[] passengerCards;

    void Start () {
        freeSprites = sprites;

        TrackInactiveTaggedCarCards();
        TrackInactiveTaggedPassengerCards();

        //Debug
        int count = 5;
        while (count > 0)
        {
            CreateCarCard();
            count--;
        }
        //
        count = 1;
        while (count > 0)
        {
            CreatePassengerCard();
            
            count--;
        }
        //
    }

    private void TrackInactiveTaggedCarCards()
    {
        carCards = GameObject.FindGameObjectsWithTag("CarCard");

        foreach (GameObject card in carCards)
        {
            card.SetActive(false);
        }
    }

    private void TrackInactiveTaggedPassengerCards()
    {
        passengerCards = GameObject.FindGameObjectsWithTag("PassengerCard");

        foreach (GameObject card in passengerCards)
        {
            card.SetActive(false);
        }
    }

    private void CreateCarCard()
    {
        GameObject newCard = GetFirstInactiveCarCard();

        if (newCard)
        {
            CreateCarCardGameObject(newCard);
        }
        else
        {
            Debug.Log("Cannot find any inactive card; cannot create a new passenger card");
        }
    }

    private void CreateCarCardGameObject(GameObject newCard)
    {
        GameObject characterAvatar = newCard.gameObject.transform.FindObjectsWithTag("CharacterAvatar")[0];
        characterAvatar.gameObject.GetComponent<Image>().sprite = GetUnusedRandomCharacterAvatar();

        newCard.GetComponent<CarRouteCard>().complexRoute = GetRandomComplexRoute();

        Transform routeTransform = newCard.gameObject.transform.Find("Route");

        routeTransform.Find("FromText").GetComponent<Text>().text =
            newCard.GetComponent<CarRouteCard>().complexRoute.GetOriginRouteOriginCity();

        routeTransform.Find("ToText").GetComponent<Text>().text =
            newCard.GetComponent<CarRouteCard>().complexRoute.GetDestinationRouteDestinationCity();

        newCard.gameObject.SetActive(true);
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

    private ComplexRoute GetRandomComplexRoute()
    {
        SimpleRoute originRoute = GetRandomSimpleRouteFromXML();
        SimpleRoute DestinationRoute = 
            GetRandomSimpleRouteFromXMLByFrom(originRoute.Destination, originRoute.Origin);

        return new ComplexRoute(originRoute, DestinationRoute);
    }

    private SimpleRoute GetRandomSimpleRouteFromXML()
    {
        return GetSimpleRouteFromXMLByIndex(
            (int)Math.Round(UnityEngine.Random.value * (simpleRoutesXMLLength - 1)),
            (int)Math.Round(UnityEngine.Random.value) == 1
        );
    }

    private SimpleRoute GetSimpleRouteFromXMLByIndex(int route_index, bool hasInvertedDirection)
    {
        XmlReader xmlReader = XmlReader.Create(simpleRoutesXMLPath);
        while (xmlReader.Read())
        {
            if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "Route"))
            {
                if (xmlReader.HasAttributes && xmlReader.GetAttribute("index") == route_index.ToString())
                {
                    if (hasInvertedDirection)
                    {
                        return new SimpleRoute(
                            xmlReader.GetAttribute("to"),
                            xmlReader.GetAttribute("from")
                        );
                    } else
                    {
                        return new SimpleRoute(
                            xmlReader.GetAttribute("from"),
                            xmlReader.GetAttribute("to")
                        );
                    }
                }
            }
        }

        return null;
    }

    private SimpleRoute GetRandomSimpleRouteFromXMLByFrom(string from, string to)
    {
        XmlReader xmlReader = XmlReader.Create(simpleRoutesXMLPath);
        List<SimpleRoute> matchingRoutes = new List<SimpleRoute>();

        while (xmlReader.Read())
        {
            if ((xmlReader.NodeType == XmlNodeType.Element) && (xmlReader.Name == "Route"))
            {
                if (
                    xmlReader.HasAttributes &&
                    IsCompatibleRoute(xmlReader, from, to)

                ) {
                    matchingRoutes.Add(new SimpleRoute(
                        xmlReader.GetAttribute("from"),
                        xmlReader.GetAttribute("to")
                        )
                    );
                }

                if (
                    xmlReader.HasAttributes &&
                    IsCompatibleRouteWhenInvertedDirection(xmlReader, from, to)

                ) {
                    matchingRoutes.Add(new SimpleRoute(
                        xmlReader.GetAttribute("to"),
                        xmlReader.GetAttribute("from")
                        )
                    );
                }
            }
        }

        return GetRandomSimpleRouteFromList(matchingRoutes);
    }

    private bool IsCompatibleRoute(XmlReader xmlReader, string from, string to)
    {
        return xmlReader.GetAttribute("to") != to && xmlReader.GetAttribute("from") == from;
    }

    private bool IsCompatibleRouteWhenInvertedDirection(XmlReader xmlReader, string from, string to)
    {
        return xmlReader.GetAttribute("from") != to && xmlReader.GetAttribute("to") == from;
    }

    private SimpleRoute GetRandomSimpleRouteFromList(List<SimpleRoute> simpleRoutes)
    {
        int randomIndex = (int)Math.Round(UnityEngine.Random.value * (simpleRoutes.Count - 1));

        return simpleRoutes[randomIndex];
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

    private GameObject GetFirstInactiveCarCard()
    {
        List<GameObject> inactiveCards = new List<GameObject>();

        foreach (GameObject card in carCards)
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
        }
        else
        {
            return null;
        }
    }
}
