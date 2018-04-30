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
            // start new game with no neutral player
            case 0:
                game.Initialize(false); 
                break;

            // start new game with neutal player
            case 1:
                game.Initialize(true); 
                break;

            // load saved game
            case 2:
                game.Initialize(SavedGame.Load("test1")); 
                break;

            // load back from the mini-game
            case 3:
                game.Initialize(SavedGame.Load("_tmp")); 
                game.GiveReward();
                break;
        }
    }
}