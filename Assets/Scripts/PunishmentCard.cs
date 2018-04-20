using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PunishmentCard : MonoBehaviour {

    [System.NonSerialized] private Player owner;
    [SerializeField] private int playerID;
    [System.NonSerialized] private Sector sector;
    [SerializeField] private int sectorID;
    public enum Effect {FreezeUnit, SkipTurn, NullifyResource};
    [System.NonSerialized] private Map map;
    [SerializeField] private Effect effect;
	private Game game;

    #region Getters and Setters

    private void Start() {
		this.game = GameObject.Find("GameManager").GetComponent<Game>();
	}

    /// <summary>
    /// 
    ///  Set the punishment card's sector
    /// 
    /// </summary>
    /// <param name="sect">Sector.</param>
    public void SetSector(Sector sect) {
        sector = sect;
    }

    /// <summary>
    /// 
    /// Gets the punishment card's effect.
    /// 
    /// </summary>
    /// <returns>The card's effect.</returns>
    public Effect GetEffect() {
        return effect;
    }

    /// <summary>
    /// 
    /// Sets the punishment card's effect.
    /// 
    /// </summary>
    /// <param name="effect">Effect.</param>
    public void SetEffect(Effect effect) {
        this.effect = effect;
    }

    /// <summary>
    /// 
    /// Sets the punishment card's reference to the game.
    /// 
    /// </summary>
    /// <param name="game">Game.</param>
    public void SetGame (Game game) {
        this.game = game;
    }

    #endregion

    /// <summary>
    /// 
    /// Initialize this punishment card using default parameters
    /// and place it in a random sector.
    /// 
    /// </summary>
    public void Initialize() {

        // get a reference to the map
        map = GameObject.Find("Map").GetComponent<Map>();

        if (map == null) {
            Debug.LogWarning("Map object or Map Script not found!");
        }

        // randomly select an effect for the card
        Effect[] arrayOfEffects = { Effect.FreezeUnit, Effect.SkipTurn, Effect.NullifyResource };
        effect = arrayOfEffects[(Mathf.RoundToInt(Random.Range(0, arrayOfEffects.Length)))];

        // if the map has already met its maximum number of punisment cards, destroy this card
        if (map.NumPunishmentCardsOnMap >= map.MaxPunishmentCardsOnMap) {
            Destroy(this.gameObject);
        } 
        // otherwise, increment the map's number of punishment cards
        else 
        {
            map.NumPunishmentCardsOnMap += 1;
        }

        // set the card's owner to null and place it in a random sector
        owner = null;
        sector = RandomizeSector();
        if (sector != null)
        {
            Transform targetTransform = sector.transform.Find("Units").transform;
            this.transform.position = targetTransform.position;
        }
        // if no valid sector was found, decrement the map's number of punishment cards and destroy this card
        else
        {
            Debug.Log("failed to find valid sector; destroying instance of card...");
            map.NumPunishmentCardsOnMap -= 1;
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// 
    /// Initialize this punishment card to have the specified effect
    /// and place it in the specified sector
    /// 
    /// </summary>
    /// <param name="sector">Sector.</param>
    /// <param name="effect">Effect.</param>
    public void Initialize(Sector sector, Effect effect) {

        // get a reference to the map
        map = GameObject.Find("Map").GetComponent<Map>();

        if (map == null) {
            Debug.LogWarning("Map object or Map Script not found!");
        }

        // set the card's effect
        this.effect = effect;

        // if the map has already met its maximum number of punisment cards, destroy this card
        if (map.NumPunishmentCardsOnMap >= map.MaxPunishmentCardsOnMap) {
            Destroy(this.gameObject);
        } 
        // otherwise, increment the map's number of punishment cards
        else 
        {
            map.NumPunishmentCardsOnMap += 1;
        }

        // set the card's owner and sector
        owner = null;
        this.sector = sector;
        if (sector != null)
        {
            Transform targetTransform = sector.transform.Find("Units").transform;
            this.transform.position = targetTransform.position;
        }
    }

    /// <summary>
    /// 
    /// Randomly selects a valid sector, gives the sector 
    /// a reference to this card, and returns the sector.
    /// 
    /// If there are no valid sectors, returns null.
    /// 
    /// </summary>
    /// <returns>A random valid sector.</returns>
    private Sector RandomizeSector() {

        // make a local instance of the array of sectors and a list of sector IDs
        Sector[] sectors = map.GetSectors();
        List<int> numbers = new List<int>();
        for (int i = 0; i <= sectors.GetLength(0); i++) {
            numbers.Add(i+1);
        }

        // find a valid sector
        Sector sect = findValidSector(sectors, numbers);

        // if a valid sector is found, give it a reference to this card
        if (sect != null)
        {
            sect.SetPunishmentCard(this);
        }

        // return the sector
        return sect;

    }

    /// <summary>
    /// 
    /// Finds and returns valid sector for a punishment card to spawn into.
    /// 
    /// </summary>
    /// <returns>A valid sector.</returns>
    /// <param name="sectors">The array sectors.</param>
    /// <param name="nums">A list the IDs of potentially valid sectors.</param>
    private Sector findValidSector(Sector[] sectors, List<int> nums) {

        // if the list of potentially valid sector IDs is empty, return null
        int size = nums.Count;
        if (size == 0) {
            return null;
        }
        // otherwise
        else {
            // select a random sector
            int rand = Mathf.RoundToInt(Random.Range(1, size));
            Sector sect = sectors[rand-1];

            // if the selected sector does not contain a punishment card, 
            // unit, landmark, or the VC, return the sector
            if (sect.GetPunishmentCard() == null && sect.GetUnit() == null && sect.GetLandmark() == null && sect.IsVC() == false) {
                return sect;
            } 
            // otherwise, remove the selected sector's ID from the list
            // of potentially valid sector IDs and recurse using the new list
            else 
            {
                nums.RemoveAt(rand-1);
                return findValidSector(sectors, nums);
            }
        }
        
    }

    /// <summary>
    /// 
    /// Activate this card's effect, then destroys this card
    /// 
    /// </summary>
	public void Use() {

        switch (effect) {
            
            //Freeze unit card
            case Effect.FreezeUnit:

                // show instruction window
                map.game.dialog.SetDialogType(Dialog.DialogType.ShowText);
                map.game.dialog.SetDialogData("Goose a Unit", "Select a unit to make\nthem unable to move\nfor one round.");
                map.game.dialog.Show();

                // set game turn state to SelectUnit with UseCard as the previous state
                map.game.prevState = Game.TurnState.UseCard;
                map.game.SetTurnState(Game.TurnState.SelectUnit);
                break;

            //Skip a turn
			case Effect.SkipTurn:
				game.OpenSkipTurnMenu();
				break;

            //Nullify resource
			case Effect.NullifyResource:
                game.OpenNullifyResourceMenu();
				break;
		}

        Destroy(this.gameObject);
	}

    /// <summary>
    /// 
    /// Assigns this card's parameters based on the provided saved data
    /// 
    /// </summary>
    /// <param name="savedData">Saved data.</param>
    public void OnLoad(PunishmentCard savedData) {
        Game game = GameObject.Find("GameManager").GetComponent<Game>();
        this.playerID = savedData.playerID;
        this.owner = game.players[savedData.playerID];
        this.sectorID = savedData.sectorID;
        this.sector = game.sectors[savedData.sectorID];
        this.effect = savedData.effect;
	}

}
