using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public GameObject pointsText;
    public GameObject pointsAddedText;

    public bool isGameOn = false;

    private GameObject draggedCard;
    private CardFactory cardFactory;

    private GameObject gameOverText;
    private GameObject gameOverButton;
    private GameObject gameOverButtonText;

    private int totalPoints;

    private bool isGameOverTextBlinking = false;

    private Coroutine showBlinkingGameOverTextCoroutine;

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

    void Start()
    {
        cardFactory = GameObject.Find("CardFactory").GetComponent<CardFactory>();

        isGameOn = true;

        gameOverText = GameObject.Find("GameOverText");
        gameOverText.SetActive(false);

        gameOverButtonText = GameObject.Find("GameOverButtonText");
        gameOverButtonText.SetActive(false);
        gameOverButtonText.GetComponent<Text>().CrossFadeAlpha(0.0f, 0.0f, false);
        gameOverButton = GameObject.Find("GameOverButton");
        gameOverButton.SetActive(false);
        gameOverButton.GetComponent<Image>().CrossFadeAlpha(0.0f, 0.0f, false);
    }

    IEnumerator ShowBlinkingGameOverText(float waitTime)
    {
        while (true)
        {
            gameOverText.GetComponent<Text>().CrossFadeAlpha(1.0f, 1.0f, true);
            
            Invoke("Hide", 1.1F);
            yield return new WaitForSeconds(waitTime);
        }
    }

    private void Hide()
    {
        gameOverText.GetComponent<Text>().CrossFadeAlpha(0.0f, 1.0f, true);
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    //public void LaunchGameStartSequence()
    //{

    //}

    public void LaunchGameOverSequence()
    {
        isGameOn = false;
        FallAllActiveCards();
        FallCities();
        Invoke("ShowGameOverUI", 3.0f);
    }

    public void AddPoints(int points)
    {
        totalPoints += points;
        pointsText.GetComponent<Text>().text = "Punts: " + totalPoints.ToString("D5");

        pointsAddedText.GetComponent<Text>().text = "+" + points;
        pointsAddedText.SetActive(true);
        Invoke("HidePointsAddedText", 5.0f);
    }

    public void PaintBackgroundByInactivePassengerCards()
    {
        PaintBackground(cardFactory.GetInactivePassengerCards().Count);
    }

    public void PaintBackground(float opacityFactor)
    {
        Color color = new Color(1, 1, 1, opacityFactor * 0.2F);

        SpriteRenderer background = GameObject.Find("Background").GetComponent<SpriteRenderer>();

        if (background.color != color)
        {
            background.color = color;
        }
    }

    public void ShowGameOverTextByInactivePassengerCards()
    {
        int n_inactive_cards = cardFactory.GetInactivePassengerCards().Count;

        if (n_inactive_cards == 1)
        {
            StartCoroutineShowBlinkingGameOverText();
        } else if (n_inactive_cards == 2)
        {
            ShowGameOverText();
        }
        else
        {
            HideGameOverText();
        }
    }

    public void StartCoroutineShowBlinkingGameOverText()
    {
        if (showBlinkingGameOverTextCoroutine == null)
        {
            gameOverText.SetActive(true);

            showBlinkingGameOverTextCoroutine = StartCoroutine(ShowBlinkingGameOverText(2.2F));
        }
    }

    private void StopCoroutineShowBlinkingGameOverText()
    {
        if (showBlinkingGameOverTextCoroutine != null)
        {
            StopCoroutine(showBlinkingGameOverTextCoroutine);
            showBlinkingGameOverTextCoroutine = null;
        }
    }

    public void ShowGameOverText()
    {
        StopCoroutineShowBlinkingGameOverText();
        gameOverText.SetActive(true);
        gameOverText.GetComponent<Text>().CrossFadeAlpha(1.0f, 2.0f, false);
    }

    public void HideGameOverText()
    {
        StopCoroutineShowBlinkingGameOverText();
        gameOverText.SetActive(false);
        gameOverText.GetComponent<Text>().CrossFadeAlpha(0.0f, 2.0f, false);
    }

    private void FallAllActiveCards()
    {
        GameObject[] carCards = GameObject.FindGameObjectsWithTag("CarCard");
        foreach (GameObject card in carCards)
        {
            card.GetComponent<Expirable>().Fall();
        }

        GameObject[] passengerCards = GameObject.FindGameObjectsWithTag("PassengerCard");
        foreach (GameObject card in passengerCards)
        {
            card.GetComponent<Draggable>().Fall();
        }
    }

    private void FallCities()
    {
        GameObject.Find("Cities").GetComponent<Rigidbody2D>().gravityScale = 1;
    }

    private void ShowGameOverUI()
    {
        gameOverButton.SetActive(true);
        gameOverButton.GetComponent<Image>().CrossFadeAlpha(1.0f, 2.0f, false);
        gameOverButtonText.SetActive(true);
        gameOverButtonText.GetComponent<Text>().CrossFadeAlpha(1.0f, 2.0f, false);
    }

    private void HidePointsAddedText()
    {
        pointsAddedText.SetActive(false);
    }

    public void OnClickGameOverButtion()
    {
        Invoke("LoadMainMenuScene", 1.0f);
    }

    private void LoadMainMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
