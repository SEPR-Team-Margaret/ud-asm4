using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Initializer : MonoBehaviour {

    public Game game;
 
    // Use this for initialization
    void Start()
    {
        game.saveFilePath = Application.persistentDataPath + "/";
        //game.Load("test6");
        game.Initialize();
	}
}
// (File.Exists(saveFilePath + fileName))