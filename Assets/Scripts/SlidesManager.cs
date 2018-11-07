using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SlidesManager : MonoBehaviour {

    public GameObject[] slides;

    private int current_slide_index = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GoToNextSlide()
    {
        int next_slide_index = current_slide_index + 1;

        if (next_slide_index == slides.Length)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }

        slides[next_slide_index].SetActive(true);
        slides[current_slide_index].SetActive(false);
        current_slide_index = next_slide_index;
    }

    public void GoToPrevSlide()
    {
        int prev_slide_index = current_slide_index - 1;

        if (prev_slide_index < 0)
        {
            SceneManager.LoadScene("MainMenu");
            return;
        }

        slides[prev_slide_index].SetActive(true);
        slides[current_slide_index].SetActive(false);
        current_slide_index = prev_slide_index;
    }
}
