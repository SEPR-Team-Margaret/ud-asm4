using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Player : MonoBehaviour {
    [SerializeField] public int playerID;

    [System.NonSerialized] public List <Sector> ownedSectors = new List<Sector>();
    [System.NonSerialized] public List <Unit> units = new List<Unit>();

    [System.NonSerialized] private Game game;
    [System.NonSerialized] private GameObject unitPrefab;
    [System.NonSerialized] private PlayerUI gui;
    [System.NonSerialized] private CardUI cardGUI;
    [SerializeField] private int attack = 0;
    [SerializeField] private int attackBonus = 0;
    [SerializeField] private int defence = 0;
    [SerializeField] private int defenceBonus = 0;
    [SerializeField] private bool resourcesNullified = false;
    [SerializeField] private int resourcesNullifiedCounter = 0;
    [SerializeField] private Color color;
    [SerializeField] private bool human;
    [SerializeField] private bool neutral;
    [SerializeField] private bool active = false;
	[SerializeField] public bool skipTurn = false;
    [SerializeField] private List<PunishmentCard> punishmentCards = new List<PunishmentCard>();

    #region Getters and Setters

    public Game GetGame() {
        return game;
    }

    public void SetGame(Game game) {
        this.game = game;
    }

    public GameObject GetUnitPrefab() {
        return unitPrefab;
    }

    public void SetUnitPrefab(GameObject prefab) {
        unitPrefab = prefab;
    }

	public PlayerUI GetGui() {
		return gui;
	}

	public void SetGui(PlayerUI gui) {
		this.gui = gui;
	}

    public CardUI GetCardUI() {
        return cardGUI;
    }

    public void SetCardUI(CardUI cardGUI) {
        this.cardGUI = cardGUI;
    }

    /// <summary>
    /// 
    /// gets this player's attack bonus
    /// 
    /// </summary>
    /// <returns>This player's attack bonus</returns>
    public int GetAttack() {
        return attack;
    }

    /// <summary>
    /// 
    /// sets this players attack bonus
    /// 
    /// </summary>
    /// <param name="attack">The player's attack bonus</param>
    public void SetAttack(int attack) {
        this.attack = attack;
    }

    /// <summary>
    /// Gets the attack bonus from the PVC minigame.
    /// </summary>
    /// <returns>The attack bonus from the PVC minigame.</returns>
    public int GetAttackBonus() {
        return attackBonus;
    }

    /// <summary>
    /// Sets the attack bonus from the PVC minigame.
    /// </summary>
    /// <param name="attackBonus">Attack bonus.</param>
    public void SetAttackBonus(int attackBonus) {
        this.attackBonus = attackBonus;
    }

    /// <summary>
    /// 
    /// gets this player's defence
    /// 
    /// </summary>
    /// <returns>The player's defence bonus</returns>
    public int GetDefence() {
        return defence;
    }

    /// <summary>
    /// 
    /// sets this players defence bonus
    /// 
    /// </summary>
    /// <param name="defence">The player's defence bonus</param>
    public void SetDefence(int defence) {
        this.defence = defence;
    }

    /// <summary>
    /// Gets the defence bonus from the PVC minigame.
    /// </summary>
    /// <returns>The defence bonus from the PVC minigame.</returns>
    public int GetDefenceBonus() {
        return defenceBonus;
    }

    /// <summary>
    /// Sets the defence bonus from the PVC minigame.
    /// </summary>
    /// <param name="defenceBonus">Defence bonus.</param>
    public void SetDefenceBonus(int defenceBonus) {
        this.defenceBonus = defenceBonus;
    }

    /// <summary>
    /// 
    /// returns if the player's resource bonus has been nullified or not
    /// 
    /// </summary>
    /// <returns><c>true</c>, if resource bonus is nullified, <c>false</c> otherwise.</returns>
    public bool GetResourcesNullified() {
        return resourcesNullified;
    }

    /// <summary>
    /// 
    /// Gets the resources nullified counter.
    /// 
    /// </summary>
    /// <returns>The resources nullified counter.</returns>
    public int GetResourcesNullifiedCounter() {
        return resourcesNullifiedCounter;
    }

    /// <summary>
    /// 
    /// Decrements the resources nullified counter.
    /// 
    /// </summary>
    public void DecrementResourcesNullifiedCounter() {
        resourcesNullifiedCounter--;
    }
    
    /// <summary>
    /// 
    /// gets the colour associated with this player
    /// 
    /// </summary>
    /// <returns>The colour associated with this player</returns>
    public Color GetColor() {
        return color;
    }

    /// <summary>
    /// 
    /// sets the players colour to be used for sector colouring and setting the player's name colour
    /// 
    /// </summary>
    /// <param name="color">The colour to be used for this player</param>
    public void SetColor(Color color) {
        this.color = color;
    }

    /// <summary>
    /// 
    /// Store who controlls the player in the save game
    /// 
    /// </summary>
    /// <returns>String "human"/"neutral"/"none" depending on the player properties</returns>
    public string GetController()
    {
        if (this.IsHuman())
        {
            return "human";
        }
        else if (this.IsNeutral())
        {
            return "neutral";
        }
        else
        {
            return "none";
        }
    }

    /// <summary>
    /// 
    /// sets how this player is controlled
    /// 
    /// </summary>
    /// <param name="controller">'human' if player controlled by human; 'neutral' if player controlled by neutral AI; all other values have no contoller</param>
    public void SetController(String controller)
    {
        if (controller.Equals("human"))
        {
            human = true;
            neutral = false;
        }
        else if (controller.Equals("neutral"))
        {
            human = false;
            neutral = true;
        }
        else
        {
            human = false;
            neutral = false;
        }
    }

    /// <summary>
    /// 
    /// Gets the list of Punishment Cards this player has
    /// 
    /// </summary>
	public List<PunishmentCard> GetPunishmentCards() {
		return punishmentCards;
    }

    /// <summary>
    /// 
    /// Adds punishment card to the list of punishment cards for the player
    /// 
    /// </summary>
    /// <param name="val">An instance of a punishment card to be added to the list</param>
	public void AddPunishmentCards(PunishmentCard card) {
		punishmentCards.Add (card);

    }

	/// <summary>
	/// 
	/// Removes punishment card from the list of punishment cards for the player
	/// 
	/// </summary>
	/// <param name="val">An instance of a punishment card to be removed from the list</param>
	public void RemovePunishmentCards(PunishmentCard card) {
		punishmentCards.Remove (card);

	}

    #endregion

    /// <summary>
    /// 
    /// returns if this player is controlled by a human or not
    /// 
    /// </summary>
    /// <returns>True if the player is controlled by a human else false</returns>
    public bool IsHuman() {
        return human;
    }

    /// <summary>
    /// 
    /// sets if this player is to be controlled by a human
    /// 
    /// </summary>
    /// <param name="human">True if the player is to be contolled by a human else false</param>
    public void SetHuman(bool human) {
        this.human = human;
    }

    /// <summary>
    /// 
    /// returns true if this player is neutral else false
    /// 
    /// </summary>
    /// <returns>True if player is neutral else false</returns>
    public bool IsNeutral()
    {
        return neutral;
    }

    /// <summary>
    /// 
    /// Sets if this player should be controlled by the neutral AI
    /// 
    /// </summary>
    /// <param name="neutral">True if the player is neutral else false</param>
    public void SetNeutral(bool neutral)
    {
        this.neutral = neutral;
    }

    /// <summary>
    /// 
    /// returns if the player is active or not
    /// 
    /// </summary>
    /// <returns>True if player is active else false</returns>
    public bool IsActive() {
        return active;
    }

    /// <summary>
    /// 
    /// sets if this player is active
    /// if it is a player's turn they are active else they are not
    /// 
    /// </summary>
    /// <param name="active">True if player is active else false</param>
    public void SetActive(bool active) {
        this.active = active;
    }

    public void SetID(int id) {
        this.playerID = id;
    }

    /// <summary>
    /// 
    /// called when this player is eliminated in order to pass all sectors owned by this player to the player that eliminated this player
    /// 
    /// </summary>
    /// <param name="player">The player to recieve all of this player's sectors</param>
    public void Defeat(Player player)
    {
        if (!IsEliminated())
            return; // Incase the player hasn't lost
        foreach (Sector sector in ownedSectors)
        {
            sector.SetOwner(player); // Reset all the sectors
        }
    }

    /// <summary>
    /// 
    /// called when this player captures a sector 
    /// updates this players attack and defence bonuses
    /// sets the sectors owner and updates its colour
    /// 
    /// </summary>
    /// <param name="sector">The sector that is being captured by this player</param>
    public void Capture(Sector sector) {

        // capture the given sector


        // store a copy of the sector's previous owner
        Player previousOwner = sector.GetOwner();

        // if the sector previously belonged to a different player
        if (previousOwner != this)
        {

            // add the sector to the list of owned sectors
            ownedSectors.Add(sector);

            // remove the sector from the previous owner's
            // list of sectors
            if (previousOwner != null)
                previousOwner.ownedSectors.Remove(sector);

            // set the sector's owner to this player
            if (game.GetTestMode())
            {
                // if in test mode, do not color sector
                sector.SetOwnerNoColour(this);
            }
            else
            {
                // otherwise, set owner normally
                sector.SetOwner(this);
            }

            // if the sector contains a landmark
            if (sector.GetLandmark() != null)
            {
                Landmark landmark = sector.GetLandmark();

                // remove the landmark's resource bonus from the previous
                // owner and add it to this player
                if (landmark.GetResourceType() == Landmark.ResourceType.Attack)
                {
                    this.attack += landmark.GetAmount();
                    if (previousOwner != null)
                        previousOwner.attack -= landmark.GetAmount();
                }
                else if (landmark.GetResourceType() == Landmark.ResourceType.Defence)
                {
                    this.defence += landmark.GetAmount();
                    if (previousOwner != null)
                        previousOwner.defence -= landmark.GetAmount();
                }
            }

        }

        if (sector.IsVC())
        {
            game.NextTurnState(); // update turn mode before game is saved
            sector.SetVC(false); // set VC to false so game can only be triggered once
            SavedGame.Save("_tmp", game);
            SceneManager.LoadScene(2); 

        }

		if (sector.GetPunishmentCard() != null) {
			AddPunishmentCards (sector.GetPunishmentCard ());
            sector.GetPunishmentCard().gameObject.SetActive(false);
            sector.SetPunishmentCard(null);
            game.gameMap.GetComponent<Map>().NumPunishmentCardsOnMap -= 1;
		}

    }

    /// <summary>
    /// 
    /// spawns a unit at each unoccupied sector containing a landmark owned by this player
    /// 
    /// </summary>
    public void SpawnUnits() {
        // scan through each owned sector
		foreach (Sector sector in ownedSectors) 
		{
            // if the sector contains a landmark and is unoccupied
            if (sector.GetLandmark() != null && sector.GetUnit() == null)
            {
                // instantiate a new unit at the sector
                if (unitPrefab == null) {
                    unitPrefab = (GameObject)Resources.Load("Unit");
                }
                
                Unit newUnit = Instantiate(unitPrefab).GetComponent<Unit>();

                // initialize the new unit
                newUnit.Initialize(this, sector);

                // add the new unit to the player's list of units and 
                // the sector's unit parameters
                units.Add(newUnit);
                sector.SetUnit(newUnit);

                newUnit.gameObject.SetActive(true);
            }
		}
	}

    /// <summary>
    /// 
    /// checks if the player has any units or if they own any landmarks
    /// if they have neither then they have been eliminated
    /// 
    /// </summary>
    /// <returns>true if the player has no units and landmarks else false</returns>
    public bool IsEliminated() {
        if (units.Count == 0 && !OwnsLandmark())
            return true;
        else
            return false;
    }
    
    public bool hasUnits()
    {
        return units.Count > 0;
    }

    /// <summary>
    /// 
    /// returns true if any of the sectors the player owns contains a landmark
    /// 
    /// </summary>
    /// <returns>true if the player owns 1 or more landmarks else false</returns>
    private bool OwnsLandmark() {

        // scan through each owned sector
        foreach (Sector sector in ownedSectors)
        {
            // if a landmarked sector is found, return true
            if (sector.GetLandmark() != null)
                return true;
        }

        // otherwise, return false
        return false;
    }

    /// <summary>
    /// 
    /// Nullifies the player's resource bonuses.
    /// 
    /// </summary>
    public void NullifyResources() {
        resourcesNullified = true;
        resourcesNullifiedCounter = 3;
    }

    /// <summary>
    /// 
    /// Restores the player's resource bonuses.
    /// 
    /// </summary>
    public void RestoreResources() {
        resourcesNullified = false;
        resourcesNullifiedCounter = 0;
    }

	/// <summary>
	/// 
	/// readies the player to have their next turn skipped (changes a boolean)
	/// 
	/// </summary>
	public void SkipTurnOn() {
		skipTurn = true;
		Debug.Log ("Skipturn: " + skipTurn + " for " + playerID);
	}

	/// <summary>
	/// 
	/// readies the player to have their next turn as normal (changes a boolean)
	/// 
	/// </summary>
	public void SkipTurnOff() {
		skipTurn = false;
	}

    public void OnLoad(GameData savedData, int playerID) {

        GameObject unitPrefab = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Unit"));
        this.SetUnitPrefab(unitPrefab);
        unitPrefab.gameObject.SetActive(false);

        this.attackBonus = savedData.playerAttackBonus[playerID];
        this.defenceBonus = savedData.playerDefenceBonus[playerID];
        this.resourcesNullified = savedData.playerNullified[playerID];
        this.resourcesNullifiedCounter = savedData.playerNullifiedCounter[playerID];
        this.color = savedData.playerColor[playerID];

        if (savedData.playerController[playerID] == "human")
        {
            this.human = true;
            this.neutral = false;
        }
        else
        {
            this.human = false;
            this.neutral = true;
        }
            
        this.active = savedData.currentPlayerID == playerID;
        this.skipTurn = savedData.playerSkip[playerID];

 //       this.punishmentCards = savedData.punishmentCards;
        string punishmentCardString = savedData.playerPunishmentCards[playerID];
        char[] separator = new char[] { '_' };
        string[] punishmentCardStringArray = punishmentCardString.Split(separator, 3);

        int freezeUnitCards = Convert.ToInt32(punishmentCardStringArray[0]);
        int nullifyResourceCards = Convert.ToInt32(punishmentCardStringArray[1]);
        int skipTurnCards = Convert.ToInt32(punishmentCardStringArray[2]);

        for (int i = 0; i < freezeUnitCards; i++)
        {
            PunishmentCard card = MonoBehaviour.Instantiate(game.GetPunishmentCardPrefab()).GetComponent<PunishmentCard>();
            card.Initialize(null, PunishmentCard.Effect.FreezeUnit);
            this.AddPunishmentCards(card);
            card.SetGame(game);
            card.gameObject.SetActive(false);
        }

        for (int i = 0; i < nullifyResourceCards; i++)
        {
            PunishmentCard card = MonoBehaviour.Instantiate(game.GetPunishmentCardPrefab()).GetComponent<PunishmentCard>();
            card.Initialize(null, PunishmentCard.Effect.NullifyResource);
            this.AddPunishmentCards(card);
            card.SetGame(game);
            card.gameObject.SetActive(false);
        }

        for (int i = 0; i < skipTurnCards; i++)
        {
            PunishmentCard card = MonoBehaviour.Instantiate(game.GetPunishmentCardPrefab()).GetComponent<PunishmentCard>();
            card.Initialize(null, PunishmentCard.Effect.SkipTurn);
            this.AddPunishmentCards(card);
            card.SetGame(game);
            card.gameObject.SetActive(false);
        }
    }

    public void UseCard(int index) {
        game.SetTurnState(Game.TurnState.UseCard);
        PunishmentCard selectedCard = punishmentCards[index];
        if (selectedCard != null) {
            selectedCard.Use();
            this.punishmentCards.RemoveAt(index);
        } else {
            Debug.LogWarning("Somehow passed a null card index, or a null card was found: "+index.ToString());
        }
        
    }
}