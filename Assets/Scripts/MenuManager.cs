using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject.Find("IntroText").GetComponent<Text>().CrossFadeAlpha(0.0f, 0.0f, false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EnterGameIntro()
    {
        //GameObject.Find("IntroCanvas").SetActive(true);

        GameObject.Find("TitleText").GetComponent<Text>().CrossFadeAlpha(0.0f, 1.0f, false);

        GameObject.Find("TitleText").GetComponent<Text>().CrossFadeAlpha(1.0f, 1.0f, false);
    }
}
