using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

    private bool playing_intro = false;

	// Use this for initialization
	void Start () {
        GameObject.Find("IntroText").GetComponent<Text>().CrossFadeAlpha(0.0f, 0.0f, false);

        GameObject.Find("TitleText").GetComponent<Text>().CrossFadeAlpha(0.0f, 0.0f, false);

        GameObject.Find("StartButton").GetComponent<Image>().CrossFadeAlpha(0.0f, 0.0f, false);
        GameObject.Find("TutorialButton").GetComponent<Image>().CrossFadeAlpha(0.0f, 0.0f, false);
        GameObject.Find("ExitButton").GetComponent<Image>().CrossFadeAlpha(0.0f, 0.0f, false);
        float fadeInTime = 2.0F;
        GameObject.Find("TitleText").GetComponent<Text>().CrossFadeAlpha(1.0f, fadeInTime, false);

        GameObject.Find("StartButton").GetComponent<Image>().CrossFadeAlpha(1.0f, fadeInTime, false);
        GameObject.Find("TutorialButton").GetComponent<Image>().CrossFadeAlpha(1.0f, fadeInTime, false);
        GameObject.Find("ExitButton").GetComponent<Image>().CrossFadeAlpha(1.0f, fadeInTime, false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    public void EnterGameIntro()
    {
        if (!playing_intro)
        {
            playing_intro = true;

            GameObject.Find("TitleText").GetComponent<Text>().CrossFadeAlpha(0.0f, 1.0f, false);

            GameObject.Find("StartButton").GetComponent<Image>().CrossFadeAlpha(0.0f, 1.0f, false);
            GameObject.Find("TutorialButton").GetComponent<Image>().CrossFadeAlpha(0.0f, 1.0f, false);
            GameObject.Find("ExitButton").GetComponent<Image>().CrossFadeAlpha(0.0f, 1.0f, false);
            GameObject.Find("StartText").GetComponent<Text>().CrossFadeAlpha(0.0f, 1.0f, false);
            GameObject.Find("TutorialText").GetComponent<Text>().CrossFadeAlpha(0.0f, 1.0f, false);
            GameObject.Find("ExitText").GetComponent<Text>().CrossFadeAlpha(0.0f, 1.0f, false);
            Invoke("DisableButtons", 1.0f);
            Invoke("ShowIntroText", 4.0f);
        }
    }

    private void DisableButtons()
    {
        GameObject.Find("StartButton").SetActive(false);
        GameObject.Find("TutorialButton").SetActive(false);
        GameObject.Find("ExitButton").SetActive(false);
    }

    private void ShowIntroText()
    {
        GameObject.Find("IntroText").GetComponent<Text>().CrossFadeAlpha(1.0f, 1.0f, false);
        Invoke("HideIntroText", 6.0f);
    }

    private void HideIntroText()
    {
        GameObject.Find("IntroText").GetComponent<Text>().CrossFadeAlpha(0.0f, 1.0f, false);
        Invoke("LoadGameScene", 2.0f);
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void LoadTutorialScene()
    {
        if (!playing_intro)
        {
            SceneManager.LoadScene("Tutorial");
        }
    }

    public void QuitGame()
    {
        if (!playing_intro)
        {
            Application.Quit();
        }
    }
}
