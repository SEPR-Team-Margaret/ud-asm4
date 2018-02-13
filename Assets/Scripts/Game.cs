using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Game : MonoBehaviour {

    public Player[] players; 
	public GameObject gameMap;
    public Player currentPlayer;
    public Sector[] sectors;

    public const int NUMBER_OF_PLAYERS = 4;
    
    public enum TurnState { Move1, Move2, EndOfTurn, NULL };
    [SerializeField] private TurnState turnState;
    [SerializeField] private bool gameFinished = false;
    [SerializeField] private bool testMode = false;
    public string saveFilePath;

    private UnityEngine.UI.Text actionsRemaining;

    [SerializeField] Dialog dialog;

    public bool triggerDialog = false;

    public bool[] eliminatedPlayers;


    public TurnState GetTurnState() {
        return turnState;
    }

    public void SetTurnState(TurnState turnState) {
        this.turnState = turnState;
    }

    public bool IsFinished() {
        return gameFinished;
    }

    public void EnableTestMode() {
        testMode = true;
    }

    public void DisableTestMode() {
        testMode = false;
    }

    public Player[] GetPlayers()
    {
        return players;
    }

    public Sector[] GetSectors()
    {
        return sectors;
    }

    public Player GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool GetTestMode()
    {
        return testMode;
    }

    public int GetPlayerID(Player player)
    {
        return System.Array.IndexOf(players, player);
    }

    /// <summary>
    /// Finds the sector the VC is assigned to
    /// </summary>
    /// <returns>The id of the sector, or -1 if not set</returns>
    public int GetVCSectorID()
    {
        foreach (Sector sector in sectors)
        {
            if (sector.isVC())
            {
                return System.Array.IndexOf(sectors, sector);
            }
        }
        return -1;
    }

    //Re-done by Peter
    public void CreatePlayers(bool neutralPlayer)
    {
        // mark the specified number of players as human
        if (!neutralPlayer)
        {
            for (int i = 0; i < NUMBER_OF_PLAYERS; i++)
            {
                players[i].SetHuman(true);
            }
            GameObject.Find("PlayerNeutralUI").SetActive(false);
            players[NUMBER_OF_PLAYERS] = GameObject.Find("Player4").GetComponent<Player>();
        }
        else
        {
            for (int i = 0; i < (NUMBER_OF_PLAYERS - 1); i++)
            {
                players[i].SetHuman(true);
            }
            players[NUMBER_OF_PLAYERS - 1] = GameObject.Find("PlayerNeutral").GetComponent<Player>();
            GameObject.Find("Player4UI").SetActive(false);
            players[NUMBER_OF_PLAYERS - 1].SetNeutral(true);
        }


        // give all players a reference to this game
        // and initialize their GUIs
        for (int i = 0; i < NUMBER_OF_PLAYERS; i++)
        {
            players[i].SetGame(this);
            players[i].GetGui().Initialize(players[i], i + 1);
        }

        eliminatedPlayers = new bool[NUMBER_OF_PLAYERS]; // always 4 players in game
        for (int i = 0; i < eliminatedPlayers.Length; i++)
        {
            eliminatedPlayers[i] = false;
        }
     }

    //modified by Peter
    public void InitializeMap() {

        // initialize all sectors, allocate players to landmarks,
        // and spawn units


		// get an array of all sectors
        sectors = gameMap.GetComponentsInChildren<Sector>();

		// initialize each sector
        foreach (Sector sector in sectors)
		{
            sector.Initialize();
		}
            
		// get an array of all sectors containing landmarks
        Sector[] landmarkedSectors = GetLandmarkedSectors(sectors);
            
        // ensure there are at least as many landmarks as players
        if (landmarkedSectors.Length < players.Length)
        {
            throw new System.Exception("Must have at least as many landmarks as players; only " + landmarkedSectors.Length.ToString() + " landmarks found for " + players.Length.ToString() + " players.");
        }


		// randomly allocate sectors to players
        foreach (Player player in players) 
		{
			bool playerAllocated = false;
            while (!playerAllocated) {
                
				// choose a landmarked sector at random
                int randomIndex = Random.Range (0, landmarkedSectors.Length);
				
                // if the sector is not yet allocated, allocate the player
                if (((Sector) landmarkedSectors[randomIndex]).GetOwner() == null)
				{
                    player.Capture(landmarkedSectors[randomIndex]);
					playerAllocated = true;
				}

                // retry until player is allocated
			}
		}

		// spawn units for each player
        foreach (Player player in players)
        {
            player.SpawnUnits();
        }

        //set Vice Chancellor
        int rand = Random.Range(0, sectors.Length);
        while (sectors[rand].GetLandmark() != null)
        {
            if (sectors[rand].GetLandmark() == null)
            {
                sectors[rand].setVC(true);
            }
            else
            {
                rand = Random.Range(0, sectors.Length);
            }
        }

    }

    private Sector[] GetLandmarkedSectors(Sector[] sectors) {

        // return a list of all sectors that contain landmarks from the given array

        List<Sector> landmarkedSectors = new List<Sector>();
        foreach (Sector sector in sectors)
        {
            if (sector.GetLandmark() != null)
            {
                landmarkedSectors.Add(sector);
            }
        }

        return landmarkedSectors.ToArray();
    }

    public bool NoUnitSelected() {
        
        // return true if no unit is selected, false otherwise


        // scan through each player
        foreach (Player player in players)
        {
            // scan through each unit of each player
            foreach (Unit unit in player.units)
            {
                // if a selected unit is found, return false
                if (unit.IsSelected() == true)
                    return false;
            }
        }

        // otherwise, return true
        return true;
    }

    public void NextPlayer() {
        // For saving tests
        SavedGame.Save("test1", this);
        //SavedGame.Load("test7");

        // set the current player to the next player in the order


        // deactivate the current player
        currentPlayer.SetActive(false);
        currentPlayer.GetGui().Deactivate();

        // find the index of the current player
       
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] == currentPlayer)
            {
                // set the next player's index
                int nextPlayerIndex = (i + 1) % NUMBER_OF_PLAYERS; // set index to next player, loop if end reached
                
                currentPlayer = players[nextPlayerIndex];
                players[nextPlayerIndex].SetActive(true);
                players[nextPlayerIndex].GetGui().Activate();
                if (currentPlayer.IsNeutral())
                {
                    NeutralPlayerTurn();
                    NeutralPlayerTurn(); 
                }
                break;                
            }
        }
    }

    //Created by Peter
    public void NeutralPlayerTurn()
    {
        NextTurnState();
        List<Unit> units = currentPlayer.units;
        Unit selectedUnit = units[Random.Range(0, units.Count)];
        Sector[] adjacentSectors = selectedUnit.GetSector().GetAdjacentSectors();
        for (int i = 0; i < adjacentSectors.Length; i++)
        {
            if (adjacentSectors[i].GetUnit() != null)
                adjacentSectors = adjacentSectors.Where(w => w != adjacentSectors[i]).ToArray();
        }
        selectedUnit.MoveTo(adjacentSectors[Random.Range(0, adjacentSectors.Length)]);
    }

    public void NextTurnState() {

        // change the turn state to the next in the order,
        // or to initial turn state if turn is completed

        switch (turnState)
        {
            case TurnState.Move1:
                turnState = TurnState.Move2;
                actionsRemaining.text = "1";
                break;

            case TurnState.Move2:
                turnState = TurnState.EndOfTurn;
                actionsRemaining.text = "0";
                break;

            case TurnState.EndOfTurn:
                turnState = TurnState.Move1;
                actionsRemaining.text = "2";
                break;

            default:
                break;
        }

        #region Remove defeated players and check if the game was won (Added by Jack 01/02/2018)

        CheckForDefeatedPlayers();

        Player winner = GetWinner();
        if (winner != null)
        {
            EndGame();
        }

        #endregion

        UpdateGUI();
    }

    #region Function to check for defeated players and notify the others (Added by Jack 01/02/2018)

    public void CheckForDefeatedPlayers()
    {
        // Checks if any players were defeated that turn state and removes them whilst notifying the rest of the players
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].IsEliminated() && eliminatedPlayers[i] == false)
            {
                // Set up the dialog box and show it
                dialog.setDialogType(Dialog.DialogType.PlayerElimated);
                dialog.setPlayerName(players[i].name);
                dialog.Show();
                eliminatedPlayers[i] = true; // ensure that the dialog is only shown once
                players[i].Defeat(currentPlayer); // Releases all owned sectors
            }
        }
    }

    #endregion

    public void EndTurn() {

        // end the current turn

        turnState = TurnState.EndOfTurn;
    }

    public Player GetWinner() {

        // return the winning player, or null if no winner yet

        Player winner = null;

        // scan through each player
        foreach (Player player in players)
        {
            // if the player hasn't been eliminated
            if (!player.IsEliminated())
            {
                // if this is the first player found that hasn't been eliminated,
                // assume the player is the winner
                if (winner == null)
                    winner = player;

                // if another player that was not eliminated was already,
                // found, then return null
                else
                    return null;
            }
        }

        // if only one player hasn't been eliminated, then return it as the winner
        return winner;
    }

    public void EndGame() {
       
        gameFinished = true;

        #region Show the winner dialog (Added by Jack 01/02/2018)

        dialog.setDialogType(Dialog.DialogType.EndGame);
        dialog.setPlayerName(GetWinner().name);
        dialog.Show();

        #endregion

        currentPlayer.SetActive(false);
        currentPlayer = null;
        turnState = TurnState.NULL;
        Debug.Log("GAME FINISHED");
    }

	public void UpdateGUI() {

		// update all players' GUIs
		for (int i = 0; i < 4; i++) {
			players [i].GetGui ().UpdateDisplay ();
		}
	}

    private void MenuButtonPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
        
    //modified by Peter
    public void Initialize () {

        #region Setup GUI components (Added by Dom 06/02/2018)
        // initialize the game
        actionsRemaining = GameObject.Find("Remaining_Actions_Value").GetComponent<UnityEngine.UI.Text>();

        UnityEngine.UI.Button endTurnButton = GameObject.Find("End_Turn_Button").GetComponent<UnityEngine.UI.Button>();
        endTurnButton.onClick.AddListener(EndTurn);

        UnityEngine.UI.Button menuButton = GameObject.Find("Menu_Button").GetComponent<UnityEngine.UI.Button>();
        menuButton.onClick.AddListener(MenuButtonPressed);
        #endregion

        // create a specified number of human players
        // *** currently hard-wired to 2 for testing ***
        CreatePlayers(true);

        // initialize the map and allocate players to landmarks
        InitializeMap();

        // initialize the turn state
        turnState = TurnState.Move1;

        // set Player 1 as the current player
        currentPlayer = players[0];
		currentPlayer.GetGui().Activate();
        players[0].SetActive(true);

		// update GUIs
		UpdateGUI();

	}

    // Initialize the game from a saved game
    public void Initialize(GameData savedGame)
    {
        // set global game settings
        this.turnState = savedGame.turnState;
        this.gameFinished = savedGame.gameFinished;
        this.testMode = savedGame.testMode;
        this.currentPlayer = players[savedGame.currentPlayerID];

        // set player attack bonus
        players[0].SetAttack(savedGame.player1Attack);
        players[1].SetAttack(savedGame.player2Attack);
        players[2].SetAttack(savedGame.player3Attack);
        players[3].SetAttack(savedGame.player4Attack);

        // set player defence bonus
        players[0].SetDefence(savedGame.player1Defence);
        players[1].SetDefence(savedGame.player2Defence);
        players[2].SetDefence(savedGame.player3Defence);
        players[3].SetDefence(savedGame.player4Defence);

        // set player colour
        players[0].SetColor(savedGame.player1Color);
        players[1].SetColor(savedGame.player2Color);
        players[2].SetColor(savedGame.player3Color);
        players[3].SetColor(savedGame.player4Color);

        // set player colour
        players[0].SetController(savedGame.player1Controller);
        players[1].SetController(savedGame.player2Controller);
        players[2].SetController(savedGame.player3Controller);
        players[3].SetController(savedGame.player4Controller);

        // set sector owners
        sectors[0].SetOwner(players[savedGame.sector01Owner]);
        sectors[1].SetOwner(players[savedGame.sector02Owner]);
        sectors[2].SetOwner(players[savedGame.sector03Owner]);
        sectors[3].SetOwner(players[savedGame.sector04Owner]);
        sectors[4].SetOwner(players[savedGame.sector05Owner]);
        sectors[5].SetOwner(players[savedGame.sector06Owner]);
        sectors[6].SetOwner(players[savedGame.sector07Owner]);
        sectors[7].SetOwner(players[savedGame.sector08Owner]);
        sectors[8].SetOwner(players[savedGame.sector09Owner]);
        sectors[9].SetOwner(players[savedGame.sector10Owner]);
        sectors[10].SetOwner(players[savedGame.sector11Owner]);
        sectors[11].SetOwner(players[savedGame.sector12Owner]);
        sectors[12].SetOwner(players[savedGame.sector13Owner]);
        sectors[13].SetOwner(players[savedGame.sector14Owner]);
        sectors[14].SetOwner(players[savedGame.sector15Owner]);
        sectors[15].SetOwner(players[savedGame.sector16Owner]);
        sectors[16].SetOwner(players[savedGame.sector17Owner]);
        sectors[17].SetOwner(players[savedGame.sector18Owner]);
        sectors[18].SetOwner(players[savedGame.sector19Owner]);
        sectors[19].SetOwner(players[savedGame.sector20Owner]);
        sectors[20].SetOwner(players[savedGame.sector21Owner]);
        sectors[21].SetOwner(players[savedGame.sector22Owner]);
        sectors[22].SetOwner(players[savedGame.sector23Owner]);
        sectors[23].SetOwner(players[savedGame.sector24Owner]);
        sectors[24].SetOwner(players[savedGame.sector25Owner]);
        sectors[25].SetOwner(players[savedGame.sector26Owner]);
        sectors[26].SetOwner(players[savedGame.sector27Owner]);
        sectors[27].SetOwner(players[savedGame.sector28Owner]);
        sectors[28].SetOwner(players[savedGame.sector29Owner]);
        sectors[29].SetOwner(players[savedGame.sector30Owner]);
        sectors[30].SetOwner(players[savedGame.sector31Owner]);
        sectors[31].SetOwner(players[savedGame.sector32Owner]);

        // set unit level in sectors




    }

    void Update () {

        // at the end of each turn, check for a winner and end the game if
        // necessary; otherwise, start the next player's turn

        if (triggerDialog)
        {
            triggerDialog = false;
            dialog.setDialogType(Dialog.DialogType.EndGame);
            dialog.setPlayerName("PLAYER 1");
            dialog.Show();
        }
        // if the current turn has ended and test mode is not enabled
        if (turnState == TurnState.EndOfTurn && !testMode)
        {
            
            // if there is no winner yet
            if (GetWinner() == null)
            {
                // start the next player's turn
                // Swapped by Jack
                NextTurnState();
                NextPlayer();

                // skip eliminated players
                while (currentPlayer.IsEliminated())
                    NextPlayer();

                // spawn units for the next player
                currentPlayer.SpawnUnits();
            }
            else
                if (!gameFinished)
                    EndGame();
        }
	}

    public void UpdateAccessible () {

        // copy of Update that can be called by other objects (for testing)

        if (turnState == TurnState.EndOfTurn)
        {
            // if there is no winner yet
            if (GetWinner() == null)
            {
                // start the next player's turn
                NextPlayer();
                NextTurnState();

                // skip eliminated players
                while (currentPlayer.IsEliminated())
                    NextPlayer();

                // spawn units for the next player
                currentPlayer.SpawnUnits();
            }
            else
                if (!gameFinished)
                    EndGame();
        }
    }  
}
