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
    // Define all properties that are needed to instantiate a Game
    // Must be public
    public Game.TurnState turnState;
    public bool gameFinished;
    public bool testMode;
    public int currentPlayerID;
    //public List<List<string>> player0;
    //public List<List<string>> player1;
    //public List<List<string>> player2;
    public List<List<string>> players;
    public string badger;

    // Fetches all data when called and assigns to properties
    public void SetupGameData(Game game)
    {
        // Game properties
        this.turnState = game.GetTurnState();
        this.gameFinished = game.IsFinished();
        this.testMode = game.GetTestMode();
        this.currentPlayerID = game.GetPlayerID(game.GetCurrentPlayer());
        this.badger = "hello badger";

        //Player Properties
        this.players = new List<List<string>>();
        int i = 0;
        foreach(Player player in game.players)
        {
            // Need to think of a better way of doing this
            List<string> playerProp = new List<string>();
            playerProp.Add(this.badger);
            //playerProp.Add(player.GetKnowledge);
            //playerProp.Add(player.GetColor);
            //playerProp.Add(player.IsHuman);
            //playerProp.Add(player.IsActive);

            this.players.Add(playerProp);
            i ++;
        }
        
    }
}
