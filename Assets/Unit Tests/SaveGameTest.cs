using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SaveGameTest
{
    private Game game;
    private Map map;
    private Player[] players;
    private PlayerUI[] gui;
    private GameObject unitPrefab;

    private void Setup()
    {
        TestSetup t = new TestSetup();
        this.game = t.GetGame();
        this.map = t.GetMap();
        this.players = t.GetPlayers();
        this.gui = t.GetPlayerUIs();
        this.unitPrefab = t.GetUnitPrefab();
    }

    private void Cleanup() {

        GameObject.Destroy(game.gameObject);
        GameObject.Destroy(map.gameObject);
        GameObject.Destroy(gui[0].GetComponentInParent<Canvas>().gameObject);

    }

    /// <summary>
    /// Check when saving and loading a game, the game object is the same
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator SaveLoadGame()
    {

        Setup();

        try {
            
   //     Game newGame = MonoBehaviour.Instantiate(this.game);
   //     game.DisableTestMode();
        game.Initialize(true);
        game.InitializeMap();
        SavedGame.Save("saveTest", game);
        Game savedGame = MonoBehaviour.Instantiate(game);
        savedGame.Initialize(SavedGame.Load("saveTest"));
        Assert.AreSame(game, savedGame);
        
        } finally {
            Cleanup();
        }
        yield return null;
    }

    /// <summary>
    /// Check if an incorrect filename is told to load, null is returned
    /// </summary>
    [UnityTest]
    public IEnumerator Load()
    {

        Setup();

        try {

        Assert.IsNull(SavedGame.Load(""));

        } finally {
            Cleanup();
        }
        yield return null;
    }
}