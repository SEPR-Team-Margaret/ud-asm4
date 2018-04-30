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
    private GameObject book;
    private GameObject soundManager;

    private void Setup()
    {
        TestSetup t = new TestSetup();
        this.game = t.GetGame();
        this.map = t.GetMap();
        this.players = t.GetPlayers();
        this.gui = t.GetPlayerUIs();
        this.unitPrefab = t.GetUnitPrefab();
        this.book = t.GetBook();
        this.soundManager = t.GetSoundManager();
    }

    private void Cleanup() {

        GameObject.Destroy(game.gameObject);
        GameObject.Destroy(map.gameObject);
        GameObject.Destroy(gui[0].GetComponentInParent<Canvas>().gameObject);
        GameObject.Destroy(book);
        GameObject.Destroy(soundManager);

    }


    /// <summary>
    /// Check if an incorrect filename is told to load, null is returned
    /// </summary>
    [UnityTest]
    public IEnumerator Load_NullFileName()
    {

        Setup();

        try {

        Assert.IsNull(SavedGame.Load(""));

        } finally {
            Cleanup();
        }
        yield return null;
    }

    /// <summary>
    /// 
    /// Checks if the current player and the number of remaining
    /// actions is are saved and loaded correctly
    /// 
    /// </summary>
    [UnityTest]
    public IEnumerator SaveLoad_Game_TurnState() {

        Setup();

        try {

            game.Initialize(false);
            game.InitializeMap();
            game.currentPlayer = players[1];
            game.SetActionsRemaining(5);
            SavedGame.Save("savetest",game);

            Game game2 = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GameManager 1")).GetComponent<Game>();
            GameData gameData = SavedGame.Load("savetest");
            game2.dialog.texture = game.dialog.texture;
            game2.Initialize(gameData);

            Assert.AreEqual(game.currentPlayer.playerID,game2.currentPlayer.playerID);
            Assert.AreEqual(game.GetActionsRemaining(),game2.GetActionsRemaining());

        } finally {
            Cleanup();
        }
        yield return null;
    }

    /// <summary>
    /// 
    /// Checks if the existence of a neutral player is saved
    /// and loaded correctly
    /// 
    /// </summary>
    [UnityTest]
    public IEnumerator SaveLoad_Game_NeutralPlayer() {

        Setup();

        try {

            game.Initialize(true);
            game.InitializeMap();
            SavedGame.Save("savetest",game);

            Game game2 = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GameManager 1")).GetComponent<Game>();
            GameData gameData = SavedGame.Load("savetest");
            game2.dialog.texture = game.dialog.texture;
            game2.Initialize(gameData);

            Assert.IsFalse(game2.players[3].IsHuman());
            Assert.IsTrue(game2.players[3].IsNeutral());

        } finally {
            Cleanup();
        }
        yield return null;
    }

    /// <summary>
    /// 
    /// Checks if the Skip Turn effect is saved and loaded
    /// correctly 
    /// 
    /// </summary>
    [UnityTest]
    public IEnumerator SaveLoad_Player_Skip() {

        Setup();

        try {

            game.Initialize(false);
            game.InitializeMap();
            game.players[1].SkipTurnOn();
            SavedGame.Save("savetest",game);

            Game game2 = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GameManager 1")).GetComponent<Game>();
            GameData gameData = SavedGame.Load("savetest");
            game2.dialog.texture = game.dialog.texture;
            game2.Initialize(gameData);

            Assert.IsTrue(game2.players[1].skipTurn);

        } finally {
            Cleanup();
        }
        yield return null;
    }

    /// <summary>
    /// 
    /// Checks if the minigame resource bonus is saved and
    /// loaded correctly
    /// 
    /// </summary>
    [UnityTest]
    public IEnumerator SaveLoad_Player_ResourceBonus() {

        Setup();

        try {

            game.Initialize(false);
            game.InitializeMap();
            game.players[1].SetAttackBonus(3);
            game.players[1].SetDefenceBonus(3);
            SavedGame.Save("savetest",game);

            Game game2 = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GameManager 1")).GetComponent<Game>();
            GameData gameData = SavedGame.Load("savetest");
            game2.dialog.texture = game.dialog.texture;
            game2.Initialize(gameData);

            Assert.AreEqual(game.players[1].GetAttackBonus(),game2.players[1].GetAttackBonus());
            Assert.AreEqual(game.players[1].GetDefenceBonus(),game2.players[1].GetDefenceBonus());

        } finally {
            Cleanup();
        }
        yield return null;
    }

    /// <summary>
    /// 
    /// Checks if the Nullify Resources effect is saved and loaded
    /// correctly
    /// 
    /// </summary>
    [UnityTest]
    public IEnumerator SaveLoad_Player_NullifiedResources() {

        Setup();

        try {
            
            game.Initialize(false);
            game.InitializeMap();
            game.players[1].NullifyResources();
            game.players[1].DecrementResourcesNullifiedCounter();
            SavedGame.Save("savetest",game);

            Game game2 = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GameManager 1")).GetComponent<Game>();
            GameData gameData = SavedGame.Load("savetest");
            game2.dialog.texture = game.dialog.texture;
            game2.Initialize(gameData);

            Assert.IsTrue(game2.players[1].GetResourcesNullified());
            Assert.AreEqual(game.players[1].GetResourcesNullifiedCounter(),game2.players[1].GetResourcesNullifiedCounter());

        } finally {
            Cleanup();
        }
        yield return null;
    }

    /// <summary>
    /// 
    /// Checks if the number and type of punishment cards owned
    /// by each player is saved and loaded correctly
    /// 
    /// </summary>
    [UnityTest]
    public IEnumerator SaveLoad_Player_PunismentCards() {

        Setup();

        try {

            game.Initialize(false);
            game.InitializeMap();
            game.players[1].AddPunishmentCards(MonoBehaviour.Instantiate(game.GetPunishmentCardPrefab()).GetComponent<PunishmentCard>());
            game.players[1].AddPunishmentCards(MonoBehaviour.Instantiate(game.GetPunishmentCardPrefab()).GetComponent<PunishmentCard>());
            game.players[1].AddPunishmentCards(MonoBehaviour.Instantiate(game.GetPunishmentCardPrefab()).GetComponent<PunishmentCard>());
            game.players[1].AddPunishmentCards(MonoBehaviour.Instantiate(game.GetPunishmentCardPrefab()).GetComponent<PunishmentCard>());
            SavedGame.Save("savetest",game);

            Game game2 = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GameManager 1")).GetComponent<Game>();
            GameData gameData = SavedGame.Load("savetest");
            game2.dialog.texture = game.dialog.texture;
            game2.Initialize(gameData);

            // count the number of each type of card in the original game
            List<PunishmentCard> cards1 = game.players[1].GetPunishmentCards();
            int freezeCards1 = 0;
            int nullifyCards1 = 0;
            int skipCards1 = 0;

            foreach (PunishmentCard card in cards1) {
                if (card.GetEffect() == PunishmentCard.Effect.FreezeUnit) {
                    freezeCards1 += 1;
                } else if (card.GetEffect() == PunishmentCard.Effect.NullifyResource) {
                    nullifyCards1 += 1;
                } else if (card.GetEffect() == PunishmentCard.Effect.SkipTurn) {
                    skipCards1 += 1;
                }
            }

            // count the number of each type of card in the loaded game
            List<PunishmentCard> cards2 = game2.players[1].GetPunishmentCards();
            int freezeCards2 = 0;
            int nullifyCards2 = 0;
            int skipCards2 = 0;

            foreach (PunishmentCard card in cards2) {
                if (card.GetEffect() == PunishmentCard.Effect.FreezeUnit) {
                    freezeCards2 += 1;
                } else if (card.GetEffect() == PunishmentCard.Effect.NullifyResource) {
                    nullifyCards2 += 1;
                } else if (card.GetEffect() == PunishmentCard.Effect.SkipTurn) {
                    skipCards2 += 1;
                }
            }

            // ensure the numbers of each type are equal across game instances
            Assert.AreEqual(freezeCards1,freezeCards2);
            Assert.AreEqual(nullifyCards1,nullifyCards2);
            Assert.AreEqual(skipCards1,skipCards2);

        } finally {
            Cleanup();
        }
        yield return null;
    }

    /// <summary>
    /// 
    /// Checks if sector ownership is saved and loaded correctly
    /// 
    /// </summary>
    [UnityTest]
    public IEnumerator SaveLoad_Sector_Owner() {

        Setup();

        try {

            game.Initialize(false);
            game.InitializeMap();
            SavedGame.Save("savetest",game);

            Game game2 = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GameManager 1")).GetComponent<Game>();
            GameData gameData = SavedGame.Load("savetest");
            game2.dialog.texture = game.dialog.texture;
            game2.Initialize(gameData);

            // get a list of the landmarked sectors in the original game
            List<Sector> landmarks1 = new List<Sector>();
            foreach (Sector sector in game.sectors) {
                if (sector.GetLandmark() != null) {
                    landmarks1.Add(sector);
                }
            }

            // get a list of the landmarked sectors in the loaded game
            List<Sector> landmarks2 = new List<Sector>();
            foreach (Sector sector in game2.sectors) {
                if (sector.GetLandmark() != null) {
                    landmarks2.Add(sector);
                }
            }

            // ensure the owners of each landmark are the same across game instances
            Assert.AreEqual(landmarks1.Count, landmarks2.Count);
            int i = 0;
            try {
                for (i = 0; i < landmarks1.Count; i++) {
                    Assert.AreEqual(landmarks1[i].GetOwner().playerID, landmarks2[i].GetOwner().playerID);
                }
            } catch (AssertionException e) {
                throw new AssertionException("Landmark " + i.ToString() + " failed assertion");
            }

        } finally {
            Cleanup();
        }
        yield return null;
    }

    /// <summary>
    /// 
    /// Checks if the location of the VC is saved and loaded
    /// correctly
    /// 
    /// </summary>
    [UnityTest]
    public IEnumerator SaveLoad_Sector_VC() {

        Setup();

        try {

            game.Initialize(false);
            game.InitializeMap();
            SavedGame.Save("savetest",game);

            Game game2 = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GameManager 1")).GetComponent<Game>();
            GameData gameData = SavedGame.Load("savetest");
            game2.dialog.texture = game.dialog.texture;
            game2.Initialize(gameData);

            Assert.AreEqual(game.GetVCSectorID(), game2.GetVCSectorID());

        } finally {
            Cleanup();
        }
        yield return null;
    }

    /// <summary>
    /// 
    /// Checks if the placement and type of punishment cards are
    /// saved and loaded correctly
    /// 
    /// </summary>
    [UnityTest]
    public IEnumerator SaveLoad_Sector_PunismentCard() {

        Setup();

        try {

            game.Initialize(false);
            game.InitializeMap();
            game.sectors[1].SetPunishmentCard(MonoBehaviour.Instantiate(game.GetPunishmentCardPrefab()).GetComponent<PunishmentCard>());
            game.sectors[2].SetPunishmentCard(MonoBehaviour.Instantiate(game.GetPunishmentCardPrefab()).GetComponent<PunishmentCard>());
            SavedGame.Save("savetest",game);

            Game game2 = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GameManager 1")).GetComponent<Game>();
            GameData gameData = SavedGame.Load("savetest");
            game2.dialog.texture = game.dialog.texture;
            game2.Initialize(gameData);

            Assert.AreEqual(game.sectors[1].GetPunishmentCard().GetEffect(), game2.sectors[1].GetPunishmentCard().GetEffect());
            Assert.AreEqual(game.sectors[2].GetPunishmentCard().GetEffect(), game2.sectors[2].GetPunishmentCard().GetEffect());

        } finally {
            Cleanup();
        }
        yield return null;
    }

    /// <summary>
    /// 
    /// Checks if unit location is saved and loaded correctly
    /// 
    /// </summary>
    [UnityTest]
    public IEnumerator SaveLoad_Unit_Sector() {

        Setup();

        try {

            game.Initialize(false);
            game.InitializeMap();
            if (game.sectors[6].IsVC()) {
                game.players[1].units[0].MoveTo(game.sectors[5]);
            } else {
                game.players[1].units[0].MoveTo(game.sectors[6]);
            }
            SavedGame.Save("savetest",game);

            Game game2 = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GameManager 1")).GetComponent<Game>();
            GameData gameData = SavedGame.Load("savetest");
            game2.dialog.texture = game.dialog.texture;
            game2.Initialize(gameData);

            if (game.sectors[6].IsVC()) {
                Assert.AreEqual(game.sectors[5].GetUnit().GetOwner().playerID, game2.sectors[5].GetUnit().GetOwner().playerID);
            } else {
                Assert.AreEqual(game.sectors[6].GetUnit().GetOwner().playerID, game2.sectors[6].GetUnit().GetOwner().playerID);
            }

        } finally {
            Cleanup();
        }
        yield return null;
    }

    /// <summary>
    /// 
    /// Checks if unit level is saved and loaded correctly
    /// 
    /// </summary>
    [UnityTest]
    public IEnumerator SaveLoad_Unit_Level() {

        Setup();

        try {

            game.Initialize(false);
            game.InitializeMap();
            game.sectors[0].GetUnit().SetLevel(5);
            SavedGame.Save("savetest",game);

            Game game2 = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GameManager 1")).GetComponent<Game>();
            GameData gameData = SavedGame.Load("savetest");
            game2.dialog.texture = game.dialog.texture;
            game2.Initialize(gameData);

            Assert.AreEqual(game.sectors[0].GetUnit().GetLevel(), game2.sectors[0].GetUnit().GetLevel());

        } finally {
            Cleanup();
        }
        yield return null;
    }

    /// <summary>
    /// 
    /// Checks if unit names are saved and loaded correctly
    /// 
    /// </summary>
    [UnityTest]
    public IEnumerator SaveLoad_Unit_Name() {

        Setup();

        try {

            game.Initialize(false);
            game.InitializeMap();
            SavedGame.Save("savetest",game);

            Game game2 = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GameManager 1")).GetComponent<Game>();
            GameData gameData = SavedGame.Load("savetest");
            game2.dialog.texture = game.dialog.texture;
            game2.Initialize(gameData);

            Assert.AreSame(game.sectors[0].GetUnit().unitName, game2.sectors[0].GetUnit().unitName);

        } finally {
            Cleanup();
        }
        yield return null;
    }

    /// <summary>
    /// 
    /// Checks if the Freeze Unit effect is saved and loaded
    /// correctly
    /// 
    /// </summary>
    [UnityTest]
    public IEnumerator SaveLoad_Unit_Frozen() {

        Setup();

        try {

            game.Initialize(false);
            game.InitializeMap();
            game.sectors[0].GetUnit().FreezeUnit();
            game.sectors[0].GetUnit().DecrementFrozenCounter();
            SavedGame.Save("savetest",game);

            Game game2 = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GameManager 1")).GetComponent<Game>();
            GameData gameData = SavedGame.Load("savetest");
            game2.dialog.texture = game.dialog.texture;
            game2.Initialize(gameData);

            Assert.IsTrue(game2.sectors[0].GetUnit().IsFrozen());
            Assert.AreEqual(game.sectors[0].GetUnit().GetFrozenCounter(), game2.sectors[0].GetUnit().GetFrozenCounter());

        } finally {
            Cleanup();
        }
        yield return null;
    }
}