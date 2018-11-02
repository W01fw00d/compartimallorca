﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject.Find("IntroText").GetComponent<Text>().CrossFadeAlpha(0.0f, 0.0f, false);
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
        GameObject.Find("TitleText").GetComponent<Text>().CrossFadeAlpha(0.0f, 1.0f, false);

        GameObject.Find("StartButton").GetComponent<Image>().CrossFadeAlpha(0.0f, 1.0f, false);
        GameObject.Find("TutorialButton").GetComponent<Image>().CrossFadeAlpha(0.0f, 1.0f, false);
        GameObject.Find("ExitButton").GetComponent<Image>().CrossFadeAlpha(0.0f, 1.0f, false);
        GameObject.Find("StartText").GetComponent<Text>().CrossFadeAlpha(0.0f, 1.0f, false);
        GameObject.Find("TutorialText").GetComponent<Text>().CrossFadeAlpha(0.0f, 1.0f, false);
        GameObject.Find("ExitText").GetComponent<Text>().CrossFadeAlpha(0.0f, 1.0f, false);
        Invoke("DisableStartButton", 1.0f);
        Invoke("ShowIntroText", 4.0f);
    }

    private void DisableStartButton()
    {
        GameObject.Find("StartButton").SetActive(false);
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

    public void QuitGame()
    {
        Application.Quit();
    }
}
