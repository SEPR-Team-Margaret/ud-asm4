using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    private bool isSaveQuitMenuOpen = false;

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

    public void openSaveQuitMenu()
    {
        if (isSaveQuitMenuOpen)
        {
            dialog.Close();
            isSaveQuitMenuOpen = false;
            return;
        }
        isSaveQuitMenuOpen = true;
        dialog.setDialogType(Dialog.DialogType.SaveQuit);
        dialog.Show();
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
                return Array.IndexOf(sectors, sector);
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
            players[NUMBER_OF_PLAYERS - 1] = GameObject.Find("Player4").GetComponent<Player>();
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
                int randomIndex = UnityEngine.Random.Range (0, landmarkedSectors.Length);
				
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
        int rand = UnityEngine.Random.Range(0, sectors.Length);
        while (sectors[rand].GetLandmark() != null)
            rand = UnityEngine.Random.Range(0, sectors.Length);
        sectors[rand].setVC(true);
        Debug.Log(sectors[rand].name);
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

    public void SaveGame(string fileName)
    {
        SavedGame.Save(fileName, this);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1);
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

    /// <summary>
    /// 
    /// Sets the active player to the next player
    /// If it is a neutral player's turn then carries out their actions
    /// 
    /// </summary>
    public void NextPlayer() {
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
    /// <summary>
    /// 
    /// Carries out the neutral player turn
    /// The neutral player units cannot move to a sector with a unit on
    /// 
    /// </summary>
    public void NeutralPlayerTurn()
    {
        NextTurnState();
        List<Unit> units = currentPlayer.units;
        Unit selectedUnit = units[UnityEngine.Random.Range(0, units.Count)];
        Sector[] adjacentSectors = selectedUnit.GetSector().GetAdjacentSectors();
        for (int i = 0; i < adjacentSectors.Length; i++)
        {
            if (adjacentSectors[i].GetUnit() != null || adjacentSectors[i].isVC())
                adjacentSectors = adjacentSectors.Where(w => w != adjacentSectors[i]).ToArray();
        }
        selectedUnit.MoveTo(adjacentSectors[UnityEngine.Random.Range(0, adjacentSectors.Length)]);
    }

    /// <summary>
    /// 
    /// Advances the turn state to the next state
    /// 
    /// </summary>
    public void NextTurnState() {

        // change the turn state to the next in the order,
        // or to initial turn state if turn is completed

        switch (turnState)
        {
            case TurnState.Move1:
                if (!currentPlayer.hasUnits())
                {
                    turnState = TurnState.EndOfTurn;
                    break;
                }
                turnState = TurnState.Move2;
                break;

            case TurnState.Move2:
                turnState = TurnState.EndOfTurn;
                break;

            case TurnState.EndOfTurn:
                turnState = TurnState.Move1;
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
    //Added by Dom (15/02/2018)
    /// <summary>
    /// 
    /// Updates the text of the Actions Remaining label
    /// 
    /// </summary>
    private void UpdateActionsRemainingLabel()
    {
        switch (turnState)
        {
            case TurnState.Move1:
                actionsRemaining.text = "2";
                break;

            case TurnState.Move2:
                actionsRemaining.text = "1";
                break;

            case TurnState.EndOfTurn:
                actionsRemaining.text = "0";
                break;

            default:
                break;
        }
    }

    #region Function to check for defeated players and notify the others (Added by Jack 01/02/2018)

    /// <summary>
    /// 
    /// Checks if any players were defeated that turn and removes them 
    /// Displays a dialog box showing which players have been defeated this turn
    /// 
    /// </summary>
    public void CheckForDefeatedPlayers()
    {
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

    /// <summary>
    /// 
    /// sets the turn phase to the EndOfTurn state
    /// 
    /// </summary>
    public void EndTurn() {

        // end the current turn

        turnState = TurnState.EndOfTurn;
    }

    /// <summary>
    /// 
    /// checks if there is a winner in the game
    /// a winner is found when there is only one player with territory remaining, (unclaimed territories are ignored)
    /// 
    /// </summary>
    /// <returns>The winning player object or null if no winner has been found</returns>
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

    /// <summary>
    /// 
    /// Called when the game is over
    /// Displays a Dialog saying which player has won and allows the player to quit the game or restart the game
    /// 
    /// </summary>
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

    /// <summary>
    /// 
    /// Updates the Player Information GUI components and the Actions remaining label
    /// 
    /// </summary>
	public void UpdateGUI() {

		// update all players' GUIs
		for (int i = 0; i < 4; i++) {
			players [i].GetGui ().UpdateDisplay();
		}
        UpdateActionsRemainingLabel();
	}

    //modified by Peter
    /// <summary>
    /// 
    /// Initializes a new game
    /// 
    /// </summary>
    public void Initialize (bool neutralPlayer) {

        #region Setup GUI components (Added by Dom 06/02/2018)
        // initialize the game
        actionsRemaining = GameObject.Find("Remaining_Actions_Value").GetComponent<UnityEngine.UI.Text>();

        UnityEngine.UI.Button endTurnButton = GameObject.Find("End_Turn_Button").GetComponent<UnityEngine.UI.Button>();
        endTurnButton.onClick.AddListener(EndTurn);
        
        #endregion

        // create a specified number of human players
        // *** currently hard-wired to 2 for testing ***
        CreatePlayers(neutralPlayer);

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
    
    //Added by Dom
    /// <summary>
    /// 
    /// sets up a game with the state stored by the passed GameData
    /// 
    /// </summary>
    /// <param name="savedGame">The saved game state</param>
    public void Initialize(GameData savedGame)
    {

        gameMap = GameObject.Find("Map");

        // initialize the game
        actionsRemaining = GameObject.Find("Remaining_Actions_Value").GetComponent<UnityEngine.UI.Text>();

        UnityEngine.UI.Button endTurnButton = GameObject.Find("End_Turn_Button").GetComponent<UnityEngine.UI.Button>();
        endTurnButton.onClick.AddListener(EndTurn);

        if (savedGame.player4Controller.Equals("human"))
        {
            CreatePlayers(false);
        } else
        {
            CreatePlayers(true);
        }

        // set global game settings
        this.turnState = savedGame.turnState;
        this.gameFinished = savedGame.gameFinished;
        this.testMode = savedGame.testMode;
        this.currentPlayer = players[savedGame.currentPlayerID];
        currentPlayer.GetGui().Activate();
        players[savedGame.currentPlayerID].SetActive(true);

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

        // get an array of all sectors
        sectors = gameMap.GetComponentsInChildren<Sector>();

        // initialize each sector
        foreach (Sector sector in sectors)
        {
            sector.Initialize();
        }

        // set sector owners
        setupSectorOwner(0, savedGame.sector01Owner);
        setupSectorOwner(1, savedGame.sector02Owner);
        setupSectorOwner(2, savedGame.sector03Owner);
        setupSectorOwner(3, savedGame.sector04Owner);
        setupSectorOwner(4, savedGame.sector05Owner);
        setupSectorOwner(5, savedGame.sector06Owner);
        setupSectorOwner(6, savedGame.sector07Owner);
        setupSectorOwner(7, savedGame.sector08Owner);
        setupSectorOwner(8, savedGame.sector09Owner);
        setupSectorOwner(9, savedGame.sector10Owner);
        setupSectorOwner(10, savedGame.sector11Owner);
        setupSectorOwner(11, savedGame.sector12Owner);
        setupSectorOwner(12, savedGame.sector13Owner);
        setupSectorOwner(13, savedGame.sector14Owner);
        setupSectorOwner(14, savedGame.sector15Owner);
        setupSectorOwner(15, savedGame.sector16Owner);
        setupSectorOwner(16, savedGame.sector17Owner);
        setupSectorOwner(17, savedGame.sector18Owner);
        setupSectorOwner(18, savedGame.sector19Owner);
        setupSectorOwner(19, savedGame.sector20Owner);
        setupSectorOwner(20, savedGame.sector21Owner);
        setupSectorOwner(21, savedGame.sector22Owner);
        setupSectorOwner(22, savedGame.sector23Owner);
        setupSectorOwner(23, savedGame.sector24Owner);
        setupSectorOwner(24, savedGame.sector25Owner);
        setupSectorOwner(25, savedGame.sector26Owner);
        setupSectorOwner(26, savedGame.sector27Owner);
        setupSectorOwner(27, savedGame.sector28Owner);
        setupSectorOwner(28, savedGame.sector29Owner);
        setupSectorOwner(29, savedGame.sector30Owner);
        setupSectorOwner(30, savedGame.sector31Owner);
        setupSectorOwner(31, savedGame.sector32Owner);

        // set unit level in sectors
        setupUnit(0, savedGame.sector01Level);
        setupUnit(1, savedGame.sector02Level);
        setupUnit(2, savedGame.sector03Level);
        setupUnit(3, savedGame.sector04Level);
        setupUnit(4, savedGame.sector05Level);
        setupUnit(5, savedGame.sector06Level);
        setupUnit(6, savedGame.sector07Level);
        setupUnit(7, savedGame.sector08Level);
        setupUnit(8, savedGame.sector09Level);
        setupUnit(9, savedGame.sector10Level);
        setupUnit(10, savedGame.sector11Level);
        setupUnit(11, savedGame.sector12Level);
        setupUnit(12, savedGame.sector13Level);
        setupUnit(13, savedGame.sector14Level);
        setupUnit(14, savedGame.sector15Level);
        setupUnit(15, savedGame.sector16Level);
        setupUnit(16, savedGame.sector17Level);
        setupUnit(17, savedGame.sector18Level);
        setupUnit(18, savedGame.sector19Level);
        setupUnit(19, savedGame.sector20Level);
        setupUnit(20, savedGame.sector21Level);
        setupUnit(21, savedGame.sector22Level);
        setupUnit(22, savedGame.sector23Level);
        setupUnit(23, savedGame.sector24Level);
        setupUnit(24, savedGame.sector25Level);
        setupUnit(25, savedGame.sector26Level);
        setupUnit(26, savedGame.sector27Level);
        setupUnit(27, savedGame.sector28Level);
        setupUnit(28, savedGame.sector29Level);
        setupUnit(29, savedGame.sector30Level);
        setupUnit(30, savedGame.sector31Level);
        setupUnit(31, savedGame.sector32Level);

        //set VC sector
        sectors[savedGame.VCSector].setVC(true);

        UpdateGUI();

    }

    //Added by Dom
    /// <summary>
    /// 
    /// sets the sector owner, if it has one
    /// 
    /// </summary>
    /// <param name="sectorId">id of sector being set</param>
    /// <param name="ownerId">id of player</param>
    private void setupSectorOwner(int sectorId, int ownerId)
    {
        if (ownerId == -1)
        {
            return;
        }
        Player p = players[ownerId];
        sectors[sectorId].SetOwner(p);
        p.ownedSectors.Add(sectors[sectorId]);
    }
    //Added by Dom
    /// <summary>
    /// 
    /// sets up the units on the passed sector
    /// 
    /// </summary>
    /// <param name="sectorIndex">Sector id of sector being setup</param>
    /// <param name="level">unit level on sector; -1 if no unit on this sector</param>
    private void setupUnit(int sectorIndex, int level)
    {
        if (level == -1)
        {
            return;
        }
        Unit unit = Instantiate(sectors[sectorIndex].GetOwner().GetUnitPrefab()).GetComponent<Unit>();
        unit.Initialize(sectors[sectorIndex].GetOwner(), sectors[sectorIndex]);
        unit.SetLevel(level);
        unit.updateUnitMaterial();
        unit.MoveTo(sectors[sectorIndex]);
        sectors[sectorIndex].GetOwner().units.Add(unit);
    }

    /// <summary>
    /// 
    /// calls UpdateAccessible
    /// 
    /// </summary>
    void Update () {
        UpdateAccessible();
	}

    /// <summary>
    /// 
    /// at the end of each turn, check for a winner and end the game if necessary; otherwise, start the next player's turn 
    /// exposed version of update method so accessible for testing
    /// 
    /// </summary>
    public void UpdateAccessible ()
    {
        if (triggerDialog)
        {
            triggerDialog = false;
            dialog.setDialogType(Dialog.DialogType.SaveQuit);
            dialog.setPlayerName("PLAYER 1");
            dialog.Show();
        }
        // if the current turn has ended and test mode is not enabled
        if (turnState == TurnState.EndOfTurn)
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

    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }


    internal void GiveReward()
    {

        int rewardLevel = PlayerPrefs.GetInt("_mgScore");
        // REWARD TO BE ADDED TO PLAYER
        Debug.Log("Player " + (Array.IndexOf(players, currentPlayer) + 1) + " has won " + rewardLevel + " points");

    }


}
