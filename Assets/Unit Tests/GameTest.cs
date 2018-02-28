﻿using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameTest 
{
    private Game game;
    private Map map;
    private Player[] players;
	private PlayerUI[] gui;
    
    private void Setup()
    {
        TestSetup t = new TestSetup();
        this.game = t.GetGame();
        this.map = t.GetMap();
        this.players = t.GetPlayers();
        this.gui = t.GetPlayerUIs();
    }

    [UnityTest]
    public IEnumerator CreatePlayers_FourPlayersAreHuman() {
        
        Setup();
        game.CreatePlayers(false);
        // ensure creation of 4 players is accurate
        Assert.IsTrue(game.GetComponent<Game>().players[0].IsHuman());
        Assert.IsTrue(game.GetComponent<Game>().players[1].IsHuman());
        Assert.IsTrue(game.GetComponent<Game>().players[2].IsHuman());
        Assert.IsTrue(game.GetComponent<Game>().players[3].IsHuman());

        yield return null;
    }

    // Test added by Owain
    [UnityTest]
    public IEnumerator CreatePlayers_ThreePlayersHumanAndOneNeutral()
    {
        Setup();
        game.CreatePlayers(true);

        // ensure game with three players and one neutral is accurate

        Assert.IsTrue(game.GetComponent<Game>().players[0].IsHuman());
        Assert.IsTrue(game.GetComponent<Game>().players[1].IsHuman());
        Assert.IsTrue(game.GetComponent<Game>().players[2].IsHuman());
        Assert.IsTrue(game.GetComponent<Game>().players[3].IsNeutral());

        yield return null;
    }


    [UnityTest]
    public IEnumerator InitializeMap_OneLandmarkAllocatedWithUnitPerPlayer() {
        
        // MAY BE MADE OBSELETE BY TESTS OF THE INDIVIDUAL METHODS
        Setup();
        game.InitializeMap();

        // ensure that each player owns 1 sector and has 1 unit at that sector
        List<Sector> listOfAllocatedSectors = new List<Sector>();
        foreach (Player player in players)
        {
            Assert.IsTrue(player.ownedSectors.Count == 1);
            Assert.IsNotNull(player.ownedSectors[0].GetLandmark());
            Assert.IsTrue(player.units.Count == 1);

            Assert.AreSame(player.ownedSectors[0], player.units[0].GetSector());

            listOfAllocatedSectors.Add(player.ownedSectors[0]);
        }


        foreach (Sector sector in map.sectors)
        {
            if (sector.GetOwner() != null && !listOfAllocatedSectors.Contains(sector)) // any sector that has an owner but is not in the allocated sectors from above
            {
                Assert.Fail(); // must be an error as only sectors owned should be landmarks from above
            }
        }

        yield return null;    
    }

    [UnityTest]
    public IEnumerator NoUnitSelected_ReturnsFalseWhenUnitIsSelected() {
        
        Setup();
        game.Initialize(false);

        // clear any selected units
        foreach (Player player in game.players)
        {
            foreach (Unit unit in player.units)
            {
                unit.SetSelected(false);
            }
        }

        game.InitializeMap();

        // assert that NoUnitSelected returns true
        Assert.IsTrue(game.NoUnitSelected());

        // select a unit
        players[0].units[0].SetSelected(true);

        // assert that NoUnitSelected returns false
        Assert.IsFalse(game.NoUnitSelected());

        yield return null;
    }


    [UnityTest]
    public IEnumerator NextPlayer_CurrentPlayerChangesToNextPlayerEachTime() {
        
        Setup();
        yield return null;

        Player playerA = players[0];
        Player playerB = players[1];
        Player playerC = players[2];
        Player playerD = players[3];

        // set the current player to the first player
        game.currentPlayer = playerA;
        playerA.SetActive(true);

        // ensure that NextPlayer changes the current player
        // from player A to player B
        game.NextPlayer();
        Assert.IsTrue(game.currentPlayer == playerB);
        Assert.IsFalse(playerA.IsActive());
        Assert.IsTrue(playerB.IsActive());

        // ensure that NextPlayer changes the current player
        // from player B to player C
        game.NextPlayer();
        Assert.IsTrue(game.currentPlayer == playerC);
        Assert.IsFalse(playerB.IsActive());
        Assert.IsTrue(playerC.IsActive());

        // ensure that NextPlayer changes the current player
        // from player C to player D
        game.NextPlayer();
        Assert.IsTrue(game.currentPlayer == playerD);
        Assert.IsFalse(playerC.IsActive());
        Assert.IsTrue(playerD.IsActive());

        // ensure that NextPlayer changes the current player
        // from player D to player A
        game.NextPlayer();
        Assert.IsTrue(game.currentPlayer == playerA);
        Assert.IsFalse(playerD.IsActive());
        Assert.IsTrue(playerA.IsActive());

        yield return null;
    }

    [UnityTest]
    public IEnumerator NextPlayer_EliminatedPlayersAreSkipped() {
        
        Setup();

        Player playerA = players[0];
        Player playerB = players[1];
        Player playerC = players[2];
        Player playerD = players[3];

        game.currentPlayer = playerA;

        playerC.units.Add(MonoBehaviour.Instantiate(playerC.GetUnitPrefab()).GetComponent<Unit>()); // make player C not eliminated
        playerD.units.Add(MonoBehaviour.Instantiate(playerD.GetUnitPrefab()).GetComponent<Unit>()); // make player D not eliminated

        game.SetTurnState(Game.TurnState.EndOfTurn);
        game.UpdateAccessible(); // removes players that should be eliminated (A and B)

        // ensure eliminated players are skipped
        Assert.IsTrue(game.currentPlayer == playerC);
        Assert.IsFalse(playerA.IsActive());
        Assert.IsFalse(playerB.IsActive());
        Assert.IsTrue(playerC.IsActive());

        yield return null;
    }

    // Test added by Owain
    [UnityTest]
    public IEnumerator NeutralPlayerTurn_EnsureNeutralPlayerMovesCorrectly()
    {
        Setup();

        game.CreatePlayers(true);
        game.InitializeMap();

        List<Unit> units = game.currentPlayer.units;
        Unit selectedUnit = units[UnityEngine.Random.Range(0, units.Count)];
        Sector[] adjacentSectors = selectedUnit.GetSector().GetAdjacentSectors();
        List<Sector> possibleSectors = new List<Sector>();
        for (int i = 0; i < adjacentSectors.Length; i++)
        {
            bool neutralOrEmpty = adjacentSectors[i].GetOwner() == null || adjacentSectors[i].GetOwner().IsNeutral();
            if (neutralOrEmpty && !adjacentSectors[i].IsVC())
                possibleSectors.Add(adjacentSectors[i]);
        }

        Sector chosenSector = null;

        if (possibleSectors.Count > 0)
        {
            chosenSector = possibleSectors[UnityEngine.Random.Range(0, possibleSectors.Count - 1)];
        }
    
        // Check that the neutral player chooses a sector to move into
        // Check that the neutral player is only moving to sectors that do not already contain units
        // Check that the neutral player is not moving to a sector containing the vice chancellor
        foreach (Sector sector in adjacentSectors)
        {
            Assert.IsNotNull(chosenSector);
            Assert.IsTrue(chosenSector.GetOwner() == null || sector.GetOwner().IsNeutral() && !sector.IsVC());
        }

        yield return null;
    }

/*
 * WARNING: THIS TEST CRASHES UNITY FOR SOME REASON
 * 
    [UnityTest]
    public IEnumerator NextTurnState_TurnStateProgressesCorrectly() {

        Debug.Log("starting");

        Setup();

        Debug.Log("setup complete");

        // initialize turn state to Move1
        game.SetTurnState(Game.TurnState.Move1);

        Debug.Log("turn state set to Move1");

        // ensure NextTurnState changes the turn state
        // from Move1 to Move2
        game.NextTurnState();

        Debug.Log("turn state set to Move2");

        Assert.IsTrue(game.GetTurnState() == Game.TurnState.Move2);

        Debug.Log("Move2 assertion passed");

        // ensure NextTurnState changes the turn state
        // from Move2 to EndOfTurn
        game.NextTurnState();

        Debug.Log("turn state set to EndOfTurn");

        Assert.IsTrue(game.GetTurnState() == Game.TurnState.EndOfTurn);

        Debug.Log("EndOfTurn assertion passed");

        // ensure NextTurnState changes the turn state
        // from EndOfTurn to Move1
        game.NextTurnState();

        Debug.Log("turn state set to Move1");

        Assert.IsTrue(game.GetTurnState() == Game.TurnState.Move1);

        Debug.Log("Move1 assertion passed");

        // ensure NextTurnState does not change turn state
        // if the current turn state is NULL
        game.SetTurnState(Game.TurnState.NULL);

        Debug.Log("turn state set to NULL");

        game.NextTurnState();

        Debug.Log("turn state set to NULL");

        Assert.IsTrue(game.GetTurnState() == Game.TurnState.NULL);

        Debug.Log("NULL assertion passed");

        Debug.Log("finished");

        yield return null;
    }
*/   
    [UnityTest]
    public IEnumerator GetWinner_OnePlayerWithLandmarksAndUnitsWins() {
        
        Setup();
        yield return null;

        Sector landmark1 = map.sectors[0];
        Player playerA = players[0];

        // ensure 'landmark1' is a landmark
        landmark1.Initialize();
        Assert.IsNotNull(landmark1.GetLandmark());

        // ensure winner is found if only 1 player owns a landmark
        ClearSectorsAndUnitsOfAllPlayers();
        playerA.ownedSectors.Add(landmark1);
        playerA.units.Add(MonoBehaviour.Instantiate(playerA.GetUnitPrefab()).GetComponent<Unit>());
        Assert.IsNotNull(game.GetWinner());

        yield return null;
    }

    [UnityTest]
    public IEnumerator GetWinner_NoWinnerWhenMultiplePlayersOwningLandmarks() {
        
        Setup();
        yield return null;

        Sector landmark1 = map.sectors[0];
        Sector landmark2 = map.sectors[10];
        Player playerA = players[0];
        Player playerB = players[1];

        // ensure'landmark1' and 'landmark2' are landmarks
        landmark1.Initialize();
        landmark2.Initialize();
        Assert.IsNotNull(landmark1.GetLandmark());
        Assert.IsNotNull(landmark2.GetLandmark());

        // ensure no winner is found if >1 players own a landmark
        ClearSectorsAndUnitsOfAllPlayers();
        playerA.ownedSectors.Add(landmark1);
        playerB.ownedSectors.Add(landmark2);
        Assert.IsNull(game.GetWinner());

        yield return null;
    }

    [UnityTest]
    public IEnumerator GetWinner_NoWinnerWhenMultiplePlayersWithUnits() {
        
        Setup();

        Player playerA = players[0];
        Player playerB = players[1];

        // ensure no winner is found if >1 players have a unit
        ClearSectorsAndUnitsOfAllPlayers();
        playerA.units.Add(MonoBehaviour.Instantiate(playerA.GetUnitPrefab()).GetComponent<Unit>());
        playerB.units.Add(MonoBehaviour.Instantiate(playerB.GetUnitPrefab()).GetComponent<Unit>());
        Assert.IsNull(game.GetWinner());

        yield return null;
    }

    [UnityTest]
    public IEnumerator GetWinner_NoWinnerWhenAPlayerHasLandmarkAndAnotherHasUnits() {
        
        Setup();
        yield return null;

        Sector landmark1 = map.sectors[0];
        Player playerA = players[0];
        Player playerB = players[1];

        // ensure 'landmark1' is a landmark
        landmark1.Initialize();
        Assert.IsNotNull(landmark1.GetLandmark());

        // ensure no winner is found if 1 player has a landmark
        // and another player has a unit
        ClearSectorsAndUnitsOfAllPlayers();
        playerA.ownedSectors.Add(landmark1);
        playerB.units.Add(MonoBehaviour.Instantiate(playerB.GetUnitPrefab()).GetComponent<Unit>());
        Assert.IsNull(game.GetWinner());

        yield return null;
    }
        
    [UnityTest]
    public IEnumerator EndGame_GameEndsCorrectlyWithNoCurrentPlayerAndNoActivePlayersAndNoTurnState() {
        
        Setup();
        yield return null;

        game.currentPlayer = game.players[0];
        game.InitializeMap();
        for (int i = 1; i < game.players.Length; i++)
        {
            game.players[i].ownedSectors = new List<Sector>();
            game.players[i].units = new List<Unit>();
        }


        game.EndGame();

        // ensure the game is marked as finished
        Assert.IsTrue(game.IsFinished());

        // ensure the current player is null
        Assert.IsNull(game.currentPlayer);

        // ensure no players are active
        foreach (Player player in game.players)
            Assert.IsFalse(player.IsActive());

        // ensure turn state is NULL
        Assert.IsTrue(game.GetTurnState() == Game.TurnState.NULL);

        yield return null;
    }


    

    private void ClearSectorsAndUnitsOfAllPlayers() {
        
        foreach (Player player in game.players)
        {
            ClearSectorsAndUnits(player);
        }
    }

    private void ClearSectorsAndUnits(Player player) {
        
        player.units = new List<Unit>();
        player.ownedSectors = new List<Sector>();
    }
}