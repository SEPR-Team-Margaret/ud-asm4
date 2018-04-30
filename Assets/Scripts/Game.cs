using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;


[System.Serializable]
public class Game : MonoBehaviour {

    /* Turn State Overhaul:
     * 
     *    - Merged the 'Move1' and 'Move2' states into a single 'Move' state
     *    - Created the 'actionsRemaining' integer to store the number of remaining moves
     *    - Added the 'prevState' field to track previous turn states
     */

    [SerializeField] public Player[] players; 
	[SerializeField] public GameObject gameMap;
    [SerializeField] public Player currentPlayer;
    [SerializeField] public Sector[] sectors;

    public const int NUMBER_OF_PLAYERS = 4;

    public enum TurnState { Move, EndOfTurn, SelectUnit, UseCard, NULL };
    [SerializeField] private TurnState turnState;
    [SerializeField] public TurnState prevState;
    
    [SerializeField] private bool gameFinished = false;
    [SerializeField] private bool testMode = false;
    public string saveFilePath;

    [SerializeField] public bool[] eliminatedPlayers;
    [SerializeField] public List<string> eliminatedUnits;

    [SerializeField] public bool triggerDialog = false;
    [SerializeField] public Dialog dialog;
    [SerializeField] public GameObject animationBlocker;
    [SerializeField] private GameObject punishmentCardPrefab;

    [SerializeField] private int actionsRemaining = 2;
    [SerializeField] private UnityEngine.UI.Text actionsRemainingLabel;
    [SerializeField] private UnityEngine.UI.Button menuButton;
    [SerializeField] private UnityEngine.UI.Button endTurnButton;
    [SerializeField] private UnityEngine.UI.Button cardButton;


    //modified by Peter
    /// <summary>
    /// 
    /// Initializes a new game
    /// 
    /// </summary>
    public void Initialize(bool neutralPlayer)
    {
        if (testMode) return;

        #region Setup GUI components
        // initialize the game
        actionsRemainingLabel = GameObject.Find("Remaining_Actions_Value").GetComponent<UnityEngine.UI.Text>();

        endTurnButton = GameObject.Find("End_Turn_Button").GetComponent<UnityEngine.UI.Button>();
        endTurnButton.onClick.AddListener(EndTurn);

        menuButton = GameObject.Find("Menu_Button").GetComponent<UnityEngine.UI.Button>();
        cardButton = GameObject.Find("Card_Button").GetComponent<UnityEngine.UI.Button>();

        dialog.SetGame(this);
        #endregion

        // create the players
        CreatePlayers(neutralPlayer);

        // initialize the map and allocate players to landmarks
        InitializeMap();

        // initialize the turn state
        turnState = TurnState.Move;

        // set Player 1 as the current player
        currentPlayer = players[0];
        currentPlayer.GetGui().Activate();
        players[0].SetActive(true);

        // update GUIs
        UpdateGUI();

    }

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
        actionsRemainingLabel = GameObject.Find("Remaining_Actions_Value").GetComponent<UnityEngine.UI.Text>();
        actionsRemaining = savedGame.actionsRemaining;

        endTurnButton = GameObject.Find("End_Turn_Button").GetComponent<UnityEngine.UI.Button>();
        endTurnButton.onClick.AddListener(EndTurn);

        menuButton = GameObject.Find("Menu_Button").GetComponent<UnityEngine.UI.Button>();
        cardButton = GameObject.Find("Card_Button").GetComponent<UnityEngine.UI.Button>();

        dialog.SetGame(this);

        if (savedGame.playerController[3] == "human")
        {
            CreatePlayers(false);
        } else
        {
            CreatePlayers(true);
        }

        this.turnState = TurnState.Move;

        this.gameFinished = savedGame.gameFinished;
        this.testMode = savedGame.testMode;
        this.currentPlayer = this.players[savedGame.currentPlayerID];
        currentPlayer.GetGui().Activate();
        this.currentPlayer.SetActive(true);

        for (int i = 0; i < 4; i++) {
            this.players[i].OnLoad(savedGame, i);
        }


        // get an array of all sectors
        sectors = gameMap.GetComponentsInChildren<Sector>();

        // initialize each sector
        for (int i = 0; i < sectors.Length; i++) {
            sectors[i].Initialize(i);
        }

        for (int i = 0; i < sectors.Length; i++) {
            this.sectors[i].OnLoad(savedGame);
        }

        int numPunishmentCards = GameObject.FindObjectsOfType<PunishmentCard>().Length;
        gameMap.GetComponent<Map>().NumPunishmentCardsOnMap = numPunishmentCards;

        UpdateGUI();

        CheckForDefeatedPlayers();

    }

    /// <summary>
    /// 
    /// Returns the current turn state
    /// 
    /// </summary>
    /// <returns>The current turn state</returns>
    public TurnState GetTurnState() {
        return turnState;
    }

    /// <summary>
    /// 
    /// Sets the current turn state
    /// 
    /// </summary>
    /// <param name="turnState">The state the current turn state should be set to</param>
    public void SetTurnState(TurnState turnState) {
        this.turnState = turnState;
    }

    /// <summary>
    /// 
    /// Intended for use with reverting from SelectUnit and UseCard states to previous move states.
    /// 
    /// </summary>
    /// <param name="turnState">The state the current turn state should be set to</param>
    public void RevertTurnState() {
        TurnState temp = this.prevState;
        this.prevState = turnState;
        this.turnState = temp;
    }

    /// <summary>
    /// 
    /// Returns if the game is finished
    /// 
    /// </summary>
    /// <returns>True if game is finished else false</returns>
    public bool IsFinished() {
        return gameFinished;
    }

    /// <summary>
    /// 
    /// Sets testmode to on
    /// 
    /// </summary>
    public void EnableTestMode() {
        testMode = true;
    }

    /// <summary>
    /// 
    /// Turns off test mode
    /// 
    /// </summary>
    public void DisableTestMode() {
        testMode = false;
    }

    /// <summary>
    /// 
    /// Sets the reference to the menu button.
    /// 
    /// </summary>
    /// <param name="menuButton">Menu button.</param>
    public void SetMenuButton(UnityEngine.UI.Button menuButton) {
        this.menuButton = menuButton;
    }

    /// <summary>
    /// 
    /// Sets the reference to the end turn button.
    /// 
    /// </summary>
    /// <param name="endTurnButton">End turn button.</param>
    public void SetEndTurnButton(UnityEngine.UI.Button endTurnButton) {
        this.endTurnButton = endTurnButton;
    }

    /// <summary>
    /// 
    /// Sets the reference to the card button.
    /// 
    /// </summary>
    /// <param name="cardButton">Card button.</param>
    public void SetCardButton(UnityEngine.UI.Button cardButton) {
        this.cardButton = cardButton;
    }

    /// <summary>
    /// 
    /// Enables the user interface buttons.
    /// 
    /// </summary>
    public void EnableUIButtons() {

        menuButton.interactable = true;
        endTurnButton.interactable = true;
        cardButton.interactable = true;
    }

    /// <summary>
    /// 
    /// Disables the user interface buttons.
    /// 
    /// </summary>
    public void DisableUIButtons() {

        menuButton.interactable = false;
        endTurnButton.interactable = false;
        cardButton.interactable = false;
    }

    /// <summary>
    /// 
    /// Returns an array of the players in this game
    /// 
    /// </summary>
    /// <returns>Array of players in this game</returns>
    public Player[] GetPlayers()
    {
        return players;
    }

    /// <summary>
    /// 
    /// Returns the array of sectors used in this game
    /// 
    /// </summary>
    /// <returns>Array of sectors used in this game</returns>
    public Sector[] GetSectors()
    {
        return sectors;
    }

    /// <summary>
    /// 
    /// Gets the object of the current player
    /// 
    /// </summary>
    /// <returns></returns>
    public Player GetCurrentPlayer()
    {
        return currentPlayer;
    }

    /// <summary>
    /// 
    /// Gets if this game is in testmode or not
    /// 
    /// </summary>
    /// <returns>True if in testmode else false</returns>
    public bool GetTestMode()
    {
        return testMode;
    }

    /// <summary>
    /// 
    /// Gets the index of the passed player object in the players array
    /// 
    /// </summary>
    /// <param name="player">The player to find the index of</param>
    /// <returns>The player objects index in players</returns>
    public int GetPlayerID(Player player)
    {
        return System.Array.IndexOf(players, player);
    }

    public void SetPlayers(Player[] players)
    {
        this.players = players;
    }

    /// <summary>
    /// 
    /// Triggers the Save and quit dialog
    /// 
    /// </summary>
    public void OpenSaveQuitMenu()
    {
        dialog.SetDialogType(Dialog.DialogType.SaveQuit);
        dialog.Show();
    }

    /// <summary>
    /// 
    /// If there is no VC on the map, sets it in a random sector.
    /// 
    /// </summary>
    public void SpawnVC() {

        if (GetVCSectorID() == -1)
        {
            // choose a sector for the Vice Chancellor to be in
            int rand = UnityEngine.Random.Range(0, sectors.Length);

            // ensure that the chosen sector does not contain a landmark
            while (sectors[rand].GetLandmark() != null)
                rand = UnityEngine.Random.Range(0, sectors.Length);

            // set the Vice Chancellor in the chosen sector
            sectors[rand].SetVC(true);
            Debug.Log("PVC is at " + sectors[rand].name);
        }
    }

    /// <summary>
    /// Finds the sector the VC is assigned to
    /// </summary>
    /// <returns>The id of the sector, or -1 if not set</returns>
    public int GetVCSectorID()
    {
        foreach (Sector sector in sectors)
        {
            if (sector.IsVC())
            {
                return Array.IndexOf(sectors, sector);
            }
        }
        return -1;
    }

    /// <summary>
    /// Gets the number of actions remaining.
    /// </summary>
    /// <returns>The number of actions remaining.</returns>
    public int GetActionsRemaining() {
        return actionsRemaining;
    }

    /// <summary>
    /// Sets the number of actions remaining.
    /// </summary>
    /// <param name="actionsRemaining">Actions remaining.</param>
    public void SetActionsRemaining(int actionsRemaining) {
        this.actionsRemaining = actionsRemaining;
    }

    /// <summary>
    /// Gets the punishment card prefab.
    /// </summary>
    /// <returns>The punishment card prefab.</returns>
    public GameObject GetPunishmentCardPrefab() {
        return punishmentCardPrefab;
    }

    //Re-done by Peter
    /// <summary>
    /// 
    /// Sets up the players in the game
    /// 3 human + 1 neutral if neutral player enabled
    /// 4 human if no neutal player
    /// 
    /// </summary>
    /// <param name="neutralPlayer">True if neutral player enabled else false</param>
    public void CreatePlayers(bool neutralPlayer)
    {
        // set player ids
        for (int i = 0; i < NUMBER_OF_PLAYERS; i++) {
            players[i].SetID(i);
        }

        // mark the specified number of players as human
        if (!neutralPlayer)
        {
            // set all 4 players as human
            for (int i = 0; i < NUMBER_OF_PLAYERS; i++)
            {
                players[i].SetHuman(true);
            }
            GameObject.Find("PlayerNeutralUI").SetActive(false);
            players[NUMBER_OF_PLAYERS - 1] = GameObject.Find("Player4").GetComponent<Player>();
        }
        else
        {
            // set players 1-3 as human
            for (int i = 0; i < (NUMBER_OF_PLAYERS - 1); i++)
            {
                players[i].SetHuman(true);
            }

            // set player 4 to neutral
            players[NUMBER_OF_PLAYERS - 1] = GameObject.Find("PlayerNeutral").GetComponent<Player>();
            GameObject.Find("Player4UI").SetActive(false);
            players[NUMBER_OF_PLAYERS - 1].SetNeutral(true);
            players[NUMBER_OF_PLAYERS - 1].playerID = 3;
        }

        // give each player a reference to a PlayerUI
        players[0].SetGui(GameObject.Find("Player1UI").GetComponent<PlayerUI>());
        players[1].SetGui(GameObject.Find("Player2UI").GetComponent<PlayerUI>());
        players[2].SetGui(GameObject.Find("Player3UI").GetComponent<PlayerUI>());
        if (!neutralPlayer)
        {
            players[3].SetGui(GameObject.Find("Player4UI").GetComponent<PlayerUI>());
        }
        else
        {
            players[3].SetGui(GameObject.Find("PlayerNeutralUI").GetComponent<PlayerUI>());
        }

        // give each player a reference to a CardUI
        players[0].SetCardUI(GameObject.Find("Player1CardUI").GetComponent<CardUI>());
        players[1].SetCardUI(GameObject.Find("Player2CardUI").GetComponent<CardUI>());
        players[2].SetCardUI(GameObject.Find("Player3CardUI").GetComponent<CardUI>());
        players[3].SetCardUI(GameObject.Find("Player4CardUI").GetComponent<CardUI>());


        // give all players a reference to this game
        // and initialize their GUIs and CardUIs
        for (int i = 0; i < NUMBER_OF_PLAYERS; i++)
        {
            players[i].SetGame(this);
            
            players[i].GetGui().Initialize(players[i], i + 1);
            players[i].GetCardUI().Initialize(players[i]);
        }

        // initialize the array of eliminated players
        eliminatedPlayers = new bool[NUMBER_OF_PLAYERS]; // always 4 players in game
        for (int i = 0; i < eliminatedPlayers.Length; i++)
        {
            eliminatedPlayers[i] = false;
        }
     }

    //modified by Peter
    /// <summary>
    /// 
    /// initialize all sectors, allocate players to landmarks, and spawn units
    /// 
    /// </summary>
    public void InitializeMap() {

		// get an array of all sectors
        sectors = gameMap.GetComponentsInChildren<Sector>();

		// initialize each sector
        for (int i = 0; i < sectors.Length; i ++) {
            sectors[i].Initialize(i);
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

        // spawn the VC
        SpawnVC();
    }

    /// <summary>
    /// 
    /// Returns an array of sectors that contain landmarks from the passed array of sectors
    /// 
    /// </summary>
    /// <param name="sectors">Sectors to search though</param>
    /// <returns>Subset of sectors from sectors that contain landmarks</returns>
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

    /// <summary>
    /// 
    /// Saves this game to file
    /// 
    /// </summary>
    /// <param name="fileName">Name of save game file</param>
    public void SaveGame(string fileName)
    {
        SavedGame.Save(fileName, this);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1);
    }

    /// <summary>
    /// 
    /// Returns if there is a unit currently selected
    /// 
    /// </summary>
    /// <returns>True if no unit is seleced else false</returns>
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

				// current player is to be skipped, change to not skip for next turn, move to player after skipped player (skipping this turn)

                while (currentPlayer.skipTurn == true) {
					Debug.Log ("Skip Me! " + currentPlayer.playerID);
					currentPlayer.SkipTurnOff ();
                    currentPlayer.GetGui().UpdateDisplay();
                    nextPlayerIndex = (nextPlayerIndex + 1) % NUMBER_OF_PLAYERS;
					currentPlayer = players [nextPlayerIndex];
					Debug.Log ("New current player: " + currentPlayer.playerID);
				}

                players[nextPlayerIndex].SetActive(true);
                players[nextPlayerIndex].GetGui().Activate();

                // decrement the player's resource bonus from the PVC minigame, if any
                if (players[nextPlayerIndex].GetAttackBonus() > 0)
                {
                    players[nextPlayerIndex].SetAttackBonus(players[nextPlayerIndex].GetAttackBonus() - 1);
                }
                if (players[nextPlayerIndex].GetDefenceBonus() > 0)
                {
                    players[nextPlayerIndex].SetDefenceBonus(players[nextPlayerIndex].GetDefenceBonus() - 1);
                }

                if (currentPlayer.IsNeutral() && !currentPlayer.IsEliminated())
                {
                    players[nextPlayerIndex].SpawnUnits();
					StartCoroutine(DelayedNeutralPlayerTurn(nextPlayerIndex));
                }
                break;                
            }
        }

        /* 
         * Decrement counters associated with the
         * temporary effects from Punishment Cards
         * 
         */

        // decrement the frozen counter of any frozen units
		for (int i = 0; i < players.Length; i++) {
			for (int j = 0; j < players[i].units.Count(); j++) {
                if (players[i].units[j].IsFrozen()) {
                    if (players[i].units[j].GetFrozenCounter() > 0)
                    {
                        players[i].units[j].DecrementFrozenCounter();
                    }
                    else
                    {
                        players[i].units[j].UnFreezeUnit();
                    }
				}
			}
		}

        // decrement the resources nullified counter of any players with nullified resources
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetResourcesNullified())
            {
                if (players[i].GetResourcesNullifiedCounter() > 0)
                {
                    players[i].DecrementResourcesNullifiedCounter();
                }
                else
                {
                    players[i].RestoreResources();
                    players[i].GetGui().UpdateDisplay();
                }
            }
        }

        // 35% chance to spawn a punishment card
        float chance = UnityEngine.Random.value;
        if (chance < 0.35f)
        {
            MonoBehaviour.Instantiate(punishmentCardPrefab).GetComponent<PunishmentCard>().Initialize();
        }
    }

    /// <summary>
    /// 
    /// Coroutine to handle the neutral player's moves,
    /// adding delays in between the moves.
    /// 
    /// </summary>
    /// <param name="playerIndex">Player index.</param>
	IEnumerator DelayedNeutralPlayerTurn(int playerIndex){
		
        DisableUIButtons ();
		yield return new WaitForSecondsRealtime(1.0f);
		NeutralPlayerTurn (playerIndex);
		yield return new WaitForSecondsRealtime(1.0f);
		NeutralPlayerTurn (playerIndex);
		EnableUIButtons ();
	}

    /// <summary>
    /// 
    /// Carries out the neutral player turn
    /// The neutral player units cannot move to a sector with a unit on
    /// 
    /// </summary>
    public void NeutralPlayerTurn(int playerIndex)
    {
        NextTurnState();

        // select one of the AI's movable units at random
        List<Unit> units = players[playerIndex].units;
		List<Unit> unfrozenUnits = new List<Unit>();
		foreach (Unit unit in units) {
			if (unit.IsFrozen() == false) {
				unfrozenUnits.Add (unit);
			}
		}

        // if no units are able to be moved, do nothing
		if (unfrozenUnits.Count == 0) {
			return;
		}

        Unit selectedUnit = unfrozenUnits[UnityEngine.Random.Range(0, unfrozenUnits.Count)];

        // select one of the sectors adjacent to the selected unit at random
        Sector[] adjacentSectors = selectedUnit.GetSector().GetAdjacentSectors();
        List<Sector> possibleSectors = new List<Sector>();
        for (int i = 0; i < adjacentSectors.Length; i++)
        {
            // do not move into any sector that is owned by another player
            // or that contains a unit, punishment card, or the PVC
            bool neutralOrEmpty = adjacentSectors[i].GetOwner() == null || adjacentSectors[i].GetOwner().IsNeutral();
            if (neutralOrEmpty && !adjacentSectors[i].IsVC() && adjacentSectors[i].GetPunishmentCard() == null)
            {
                if (adjacentSectors[i].GetUnit() == null)
                {
                    possibleSectors.Add(adjacentSectors[i]);
                }
            }
        }

        // if there is at least one eligible sector, select one and move into it
        if (possibleSectors.Count > 0)
        {
            selectedUnit.MoveTo(possibleSectors[UnityEngine.Random.Range(0, possibleSectors.Count -1)]);
        }
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
            case TurnState.Move:
                Debug.Log("Move Initiated");
                if (!currentPlayer.hasUnits())
                {
                    this.prevState = turnState;
                    turnState = TurnState.EndOfTurn;
                    Debug.LogWarning(currentPlayer + " has no units.");
                    break;
                }
                this.prevState = turnState;
                actionsRemaining -= 1;
                if (actionsRemaining != 0)
                {
                    turnState = TurnState.Move;
                }
                else
                {
                    turnState = TurnState.EndOfTurn;
                    EndTurn();
                }
                break;

			case TurnState.EndOfTurn:
				Debug.Log ("EndOfTurn Initiated");
				this.prevState = turnState;
				turnState = TurnState.Move;
				actionsRemaining = 2;
	            break;

            case TurnState.SelectUnit:
                Debug.Log("SelectUnit Initiated");
                if (this.prevState == TurnState.Move) {
                    this.prevState = turnState;
                    actionsRemaining -= 1;
                    if (actionsRemaining != 0)
                    {
                        turnState = TurnState.Move;
                    }
                    else
                    {
                        turnState = TurnState.EndOfTurn;
                        EndTurn();
                    }
                } else if (this.prevState == TurnState.UseCard) {
                    this.prevState = turnState;
                    this.turnState = TurnState.Move;
                } else {
                    Debug.LogWarning("Previous State not updated correctly: currentState: " + this.turnState + ", prevState: " + this.prevState+".");
                }
                break;

            case TurnState.UseCard:
                Debug.Log("UseCardd Initiated");
                this.prevState = turnState;
                this.turnState = TurnState.Move;
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

    /// <summary>
    /// 
    /// Updates the text of the Actions Remaining label
    /// 
    /// </summary>
    private void UpdateActionsRemainingLabel()
    {
        switch (turnState)
        {
            case TurnState.Move:
                actionsRemainingLabel.text = actionsRemaining.ToString();
                break;

            case TurnState.EndOfTurn:
                actionsRemainingLabel.text = "0";
                break;

            case TurnState.SelectUnit:
            case TurnState.UseCard:
                actionsRemainingLabel.text = actionsRemaining.ToString();
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// 
    /// Sets the actions remaining label.
    /// 
    /// </summary>
    /// <param name="actionsRemaining">Text to set as the actions remaining label</param>
    public void SetActionsRemainingLabel(UnityEngine.UI.Text actionsRemaining)
    {
        this.actionsRemainingLabel = actionsRemaining;
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
                eliminatedPlayers[i] = true; // only disable players once
                players[i].Defeat(currentPlayer); // Releases all owned sectors
                players[i].GetGui().gameObject.SetActive(false);
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

        // if the current turn has ended and test mode is not enabled
        if (turnState == TurnState.EndOfTurn) {

            // if there is no winner yet
            if (GetWinner() == null) {
                
                // start the next player's turn
                NextTurnState();
                NextPlayer();

                // skip eliminated players
                while (currentPlayer.IsEliminated())
                    NextPlayer();

                // spawn units for the next player
                currentPlayer.SpawnUnits();
            } else
                if (!gameFinished)
                EndGame();
        } else {
            Debug.LogWarning("State was somehow changed from EOT, currentState: " + this.turnState + ", prevState: " + this.prevState + ".");
        }
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

        dialog.SetDialogType(Dialog.DialogType.EndGame);
        dialog.SetDialogData(GetWinner().name);
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

    /// <summary>
    /// 
    /// sets the sector owner, if it has one
    /// 
    /// </summary>
    /// <param name="sectorId">id of sector being set</param>
    /// <param name="ownerId">id of player</param>
    private void SetupSectorOwner(int sectorId, int ownerId)
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
    private void SetupUnit(int sectorIndex, int level)
    {
        if (level == -1)
        {
            return;
        }
        Unit unit = Instantiate(sectors[sectorIndex].GetOwner().GetUnitPrefab()).GetComponent<Unit>();
        unit.Initialize(sectors[sectorIndex].GetOwner(), sectors[sectorIndex]);
        unit.SetLevel(level);
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
            dialog.SetDialogType(Dialog.DialogType.SaveQuit);
            dialog.SetDialogData("PLAYER 1");
            dialog.Show();
        }
        
    }
    
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteAll();
    }

    /// <summary>
    /// 
    /// Allocates a reward to the player when they complete the mini game
    /// 
    /// </summary>
    internal void GiveReward()
    {
        int rewardLevel = PlayerPrefs.GetInt("_mgScore");
        // REWARD TO BE ADDED TO PLAYER
        int bonus = 1; 
        if (rewardLevel <= 3)
        {
            bonus = 1;
        }
        else if (rewardLevel <= 6)
        {
            bonus = 2;
        }
        else if (rewardLevel <= 9)
        {
            bonus = 3;
        }
        else
        {
            bonus = 4;
        }

        Player rewardedPlayer;

        // if the turn has already passed to the next player, reward the previous player
        if (actionsRemaining == 2)
        {
            rewardedPlayer = players[currentPlayer.playerID - 1];
        }
        // otherwise, reward the current player
        else
        {
            rewardedPlayer = currentPlayer;
        }

        rewardedPlayer.SetAttackBonus(rewardedPlayer.GetAttackBonus() + bonus);
        rewardedPlayer.SetDefenceBonus(rewardedPlayer.GetDefenceBonus() + bonus);

        dialog.SetDialogType(Dialog.DialogType.ShowText);

        dialog.SetDialogData("REWARD!", string.Format("Well done, you have gained:\n+{0} Attack\n+{0} Defence", bonus));

        dialog.Show();

        UpdateGUI(); // update GUI with new bonuses

    }

	/// <summary>
	/// 
	/// Triggers the skip turn dialog
	/// 
	/// </summary>
	public void OpenSkipTurnMenu()
	{
		dialog.SetDialogType(Dialog.DialogType.SelectTurnSkip);
		dialog.Show();
	}

	/// <summary>
	/// 
	/// Picks the player to apply the skip turn to by taking the value of item selected in drop down (only 3 options)
	/// Then choses the correct player, based on the options the player would have had in their dropdown box
	/// 
	/// </summary>
	public void SkipSelectedPlayer(){
		int selectedPlayerInt = dialog.GetSelectedPlayer (); // gives the value of the option selected (0-2)

		if (currentPlayer == players [0]) {
			int[] player1options = { 1, 2, 3 };
			players [player1options [selectedPlayerInt]].SkipTurnOn ();
		} else if (currentPlayer == players [1]) {
			int[] player2options = { 0, 2, 3 };
			players [player2options [selectedPlayerInt]].SkipTurnOn ();
		} else if (currentPlayer == players [2]) {
			int[] player3options = { 0, 1, 3 };
			players [player3options [selectedPlayerInt]].SkipTurnOn ();
		} else if (currentPlayer == players [3]) {
			int[] player4options = { 0, 1, 2 };
			players [player4options [selectedPlayerInt]].SkipTurnOn ();
		} else {
			Debug.Log ("Something went wrong chosing the player to skip in game.cs");
		}
		dialog.Close();
		NextTurnState ();
	}

    /// <summary>
    /// 
    /// Triggers the Nullify Resource dialog
    /// 
    /// </summary>
    public void OpenNullifyResourceMenu() {
        dialog.SetDialogType(Dialog.DialogType.SelectNullifyResource);
        dialog.Show();
    }

    /// <summary>
    /// 
    /// Picks the player to apply the Nullify Resource to by taking 
    /// the value of item selected in drop down (only 3 options),
    /// Then choses the correct player, based on the options the 
    /// player would have had in their dropdown box
    /// 
    /// </summary>
    public void NullifyResourceForSelectedPlayer() {
        int selectedPlayerInt = dialog.GetSelectedPlayer (); // gives the value of the option selected (0-2)

        if (currentPlayer == players [0]) {
            int[] player1options = { 1, 2, 3 };
            players [player1options [selectedPlayerInt]].NullifyResources();
        } else if (currentPlayer == players [1]) {
            int[] player2options = { 0, 2, 3 };
            players [player2options [selectedPlayerInt]].NullifyResources();
        } else if (currentPlayer == players [2]) {
            int[] player3options = { 0, 1, 3 };
            players [player3options [selectedPlayerInt]].NullifyResources();
        } else if (currentPlayer == players [3]) {
            int[] player4options = { 0, 1, 2 };
            players [player4options [selectedPlayerInt]].NullifyResources();
        } else {
            Debug.Log ("Something went wrong chosing the player to skip in game.cs");
        }
        dialog.Close();
        NextTurnState();
    }
}