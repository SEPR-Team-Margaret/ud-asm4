using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml.Serialization;

[System.Serializable]
public class GameData
{
    // Define all properties that are needed to
    //   instantiate a Game
    // Must be public
    public Game.TurnState turnState;
    public bool gameFinished;
    public bool testMode;
    public int currentPlayerID;

    // Fetches all data when called and assigns to properties
    public void SetupGameData(Game game)
    {
        // Game properties
        this.turnState = game.GetTurnState();
        this.gameFinished = game.IsFinished();
        this.testMode = game.GetTestMode();
        this.currentPlayerID = game.GetPlayerID(game.GetCurrentPlayer());

        //Player Properties
    }
}
