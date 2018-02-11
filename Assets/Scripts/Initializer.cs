using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Initializer : MonoBehaviour {

    public Game game;
 
    // Use this for initialization
    void Start()
    {
        SavedGame.Load("test8");
        
        game.Initialize();
	}
}
// (File.Exists(saveFilePath + fileName))