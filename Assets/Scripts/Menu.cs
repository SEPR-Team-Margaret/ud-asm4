using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	public void Play (bool neutralPlayer){
        PlayerPrefs.SetInt("_gamemode", neutralPlayer ? 1 : 0);
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);
	}

    public void PlayLoad()
    {
        PlayerPrefs.SetInt("_gamemode", 2);
        SceneManager.LoadScene(1);
    }

	public void Quit (){
		Application.Quit();
	}
}
