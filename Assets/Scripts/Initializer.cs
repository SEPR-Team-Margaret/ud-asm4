using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Initializer : MonoBehaviour {

    public Game game;
 
    /// <summary>
    /// 
    /// start the game
    /// 
    /// </summary>
    void Start()
    {
        switch (PlayerPrefs.GetInt("_gamemode", 0))
        {
            case 0:
                game.Initialize(false);
                break;
            case 1:
                game.Initialize(true);
                break;
            case 2:
                game.Initialize(SavedGame.Load("test1"));
                break;
            case 3:
                game.Initialize(SavedGame.Load("_tmp"));
                game.GiveReward();
                break;
        }
        //game.Initialize();
    }
}