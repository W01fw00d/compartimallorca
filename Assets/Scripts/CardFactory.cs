using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Xml;
using System.IO;

public class CardFactory : MonoBehaviour
{
    public Sprite blankSprite;
    public List<Sprite> sprites;
    private List<Sprite> freeSprites;

    private string simpleRoutesXMLPath = "Assets/Scripts/Content/simple_routes.xml";
    private int simpleRoutesXMLLength = 13;

    private GameObject[] carCards;
    private GameObject[] passengerCards;

    private GameManager gameManager;

    private Coroutine createCardCarCoroutine;
    private Coroutine createPassengerCarCoroutine;

    void Start () {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        freeSprites = sprites;

        TrackInactiveTaggedCarCards();
        TrackInactiveTaggedPassengerCards();

        CreateCarCard();
        CreatePassengerCard();
        CreatePassengerCard();
        CreatePassengerCard();
        CreatePassengerCard();
        CreateCarCard();

        float carCreationPeriod = 5.0F; //Balanced 5.0F
        float passengerCreationPeriod = 10.0F; //Balanced 10.0F

        createCardCarCoroutine = StartCoroutine(CreateCarCardRoutine(carCreationPeriod));
        createPassengerCarCoroutine = StartCoroutine(CreatePassengerCardRoutine(passengerCreationPeriod));
    }

    IEnumerator CreateCarCardRoutine(float waitTime)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);
            CreateCarCard();
        }
    }

    IEnumerator CreatePassengerCardRoutine(float waitTime)
    {
        float minWaitTime = 1.0F;

        int counter = 0;
        int speedUpFrequency = 6; //Balanced <=6?
        float speedUpFactor = 0.1F; //Balanced 0.1F

        while (true)
        {
            if (
                waitTime >= minWaitTime &&
                counter % speedUpFrequency == 0
            ) {
                waitTime -= speedUpFactor;
            }

            yield return new WaitForSeconds(waitTime);
            CreatePassengerCard();

            counter++;
        }
    }

    public void AddFreeSprite(Sprite sprite)
    {
        //freeSprites.Add(sprite);
    }

    public void StopCreateCardCoroutines()
    {
        StopCoroutine(createCardCarCoroutine);
        StopCoroutine(createPassengerCarCoroutine);
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
            Debug.Log("Cannot find any inactive car card; cannot create a new car card");
        }
    }

    private void CreateCarCardGameObject(GameObject newCard)
    {
        GameObject characterAvatar = newCard.gameObject.transform.FindObjectsWithTag("CharacterAvatar")[0];
        characterAvatar.gameObject.GetComponent<Image>().sprite = GetUnusedRandomCharacterAvatar();

        SetCarCardSeatSlotsRandom(characterAvatar);
        SetCarCarRandomComplexRoute(newCard);
        SetCarCardRandomTimer(newCard);

        newCard.gameObject.SetActive(true);
        newCard.gameObject.GetComponent<Expirable>().cardActive = true;
        newCard.GetComponent<Expirable>().FadeIn();
    }

    private void SetCarCardRandomTimer(GameObject newCard)
    {
        float randomExpirationLimit = (int)Math.Round(UnityEngine.Random.value * 40.0f) + 20.0f;

        newCard.GetComponent<Expirable>().SetExpiresIn(randomExpirationLimit);
    }

    private void SetCarCardSeatSlotsRandom(GameObject characterAvatar)
    {
        List<GameObject> seatSlotGameObjects = characterAvatar.gameObject.transform.FindObjectsWithTag("EmptySeat");
        int n_seatSlots = (int)Math.Round(UnityEngine.Random.value * (seatSlotGameObjects.Count - 1));

        while (n_seatSlots > 0)
        {
            seatSlotGameObjects[n_seatSlots].SetActive(false);
            seatSlotGameObjects[n_seatSlots].tag = "BlockedSeat";
            n_seatSlots--;
        }
    }

    private void SetCarCarRandomComplexRoute(GameObject newCard)
    {
        newCard.GetComponent<Expirable>().complexRoute = GetRandomComplexRoute();
        Transform routeTransform = newCard.gameObject.transform.Find("Route");

        routeTransform.Find("FromText").GetComponent<Text>().text =
            newCard.GetComponent<Expirable>().complexRoute.GetOriginRouteOriginCity();

        routeTransform.Find("ToText").GetComponent<Text>().text =
            newCard.GetComponent<Expirable>().complexRoute.GetDestinationRouteDestinationCity();
    }

    private void CreatePassengerCard()
    {
        GameObject newCard = GetFirstInactivePassengerCard();

        if (newCard)
        {
            CreatePassengerCardGameObject(newCard);

        } else
        {
            //No deberia pasar nunca
            Debug.Log("Cannot find any inactive passenger card; cannot create a new passenger card");
        }
    }

    private void CreatePassengerCardGameObject(GameObject newCard)
    {
        GameObject characterAvatar = newCard.gameObject.transform.FindObjectsWithTag("CharacterAvatar")[0];
        characterAvatar.gameObject.GetComponent<Image>().sprite = GetUnusedRandomCharacterAvatar();

        SimpleRoute randomSimpleRoute = GetRandomSimpleRoute();

        if (randomSimpleRoute != null)
        {
            newCard.GetComponent<Draggable>().simpleRoute = randomSimpleRoute;

            Transform routeTransform = newCard.gameObject.transform.Find("Route");

            routeTransform.Find("FromText").GetComponent<Text>().text =
                newCard.GetComponent<Draggable>().simpleRoute.Origin;

            routeTransform.Find("ToText").GetComponent<Text>().text =
                newCard.GetComponent<Draggable>().simpleRoute.Destination;

            newCard.gameObject.SetActive(true);
            newCard.GetComponent<Draggable>().cardActive = true;

            newCard.GetComponent<Draggable>().FadeIn();

            // Unificar este bloque con el de GameManager
            int n_nextCards = GetInactivePassengerCards().Count;
            gameManager.PaintBackground(n_nextCards);

            if (n_nextCards == 0)
            {
                gameManager.ShowGameOverText();
                gameManager.LaunchGameOverSequence();
            }
            else if (n_nextCards == 1)
            {
                gameManager.StartCoroutineShowBlinkingGameOverText();
            }
            else if (n_nextCards == 2)
            {
                gameManager.ShowGameOverText();
            }
            else
            {
                gameManager.HideGameOverText();
            }

        } else
        {
            //Debug.Log("Cannot find any car card; cannot create a new passenger card");
        }
    }

    private SimpleRoute GetRandomSimpleRoute()
    {
        GameObject carCard = GetRandomActiveCarCard();
        if (carCard)
        {
            ComplexRoute complexRoute = carCard.GetComponent<Expirable>().complexRoute;

            bool shallGetOrigin = (int)Math.Round(UnityEngine.Random.value * 1) == 1;

            return shallGetOrigin ?
                complexRoute.OriginRoute :
                complexRoute.DestinationRoute;
        } else
        {
            return null;
        }
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
            //freeSprites.RemoveAt(sprite_index);
        }

        return sprite;
    }

    private GameObject GetFirstInactiveCarCard()
    {
        List<GameObject> inactiveCards = new List<GameObject>();

        foreach (GameObject card in carCards)
        {
            if (!card.GetComponent<Expirable>().cardActive)
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

    private GameObject GetRandomActiveCarCard()
    {
        List<GameObject> activeCards = new List<GameObject>();

        foreach (GameObject card in carCards)
        {
            if (card.activeSelf)
            {
                activeCards.Add(card);
            }
        }

        if (activeCards.Count > 0)
        {
            return activeCards[(int)Math.Round(UnityEngine.Random.value * (activeCards.Count - 1))];
        }
        else
        {
            return null;
        }
    }

    private GameObject GetFirstInactivePassengerCard()
    {
        List<GameObject> inactiveCards = GetInactivePassengerCards();

        if (inactiveCards.Count > 0)
        {
            return inactiveCards[0];
        }
        else
        {
            return null;
        }
    }

    public List<GameObject> GetInactivePassengerCards()
    {
        List<GameObject> inactiveCards = new List<GameObject>();

        foreach (GameObject card in passengerCards)
        {
            if (!card.GetComponent<Draggable>().cardActive)
            {
                inactiveCards.Add(card);
            }
        }

        return inactiveCards;
    }
}
