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

    public void SetSector(Sector sect) {
        sector = sect;
    }

    public Effect GetEffect() {
        return effect;
    }

    public void SetEffect(Effect effect) {
        this.effect = effect;
    }

    #endregion

    public void Initialize(/*Player player, Sector sector*/) {
        
        map = GameObject.Find("Map").GetComponent<Map>();

        if (map == null) {
            Debug.LogWarning("Map object or Map Script not found!");
        }

        Effect[] arrayOfEffects = { Effect.FreezeUnit, Effect.SkipTurn, Effect.NullifyResource };
        effect = arrayOfEffects [(Mathf.RoundToInt (Random.Range (0, 2)))];

        if (map.NumPunishmentCardsOnMap >= map.MaxPunishmentCardsOnMap) {
            Destroy(this.gameObject);
        } else {
            map.NumPunishmentCardsOnMap += 1;
        }
        owner = null/*player*/;
        sector = RandomizeSector();
        if (sector != null)
        {
            Transform targetTransform = sector.transform.Find("Units").transform;
            this.transform.position = targetTransform.position;
        }
        else
        {
            Debug.Log("failed to find valid sector");
        }
    }

    public void Initialize(Sector sector, Effect effect) {

        map = GameObject.Find("Map").GetComponent<Map>();

        if (map == null) {
            Debug.LogWarning("Map object or Map Script not found!");
        }

        this.effect = effect;

        if (map.NumPunishmentCardsOnMap >= map.MaxPunishmentCardsOnMap) {
            Destroy(this.gameObject);
        } else {
            map.NumPunishmentCardsOnMap += 1;
        }
        owner = null;
        this.sector = sector;
        if (sector != null)
        {
            Transform targetTransform = sector.transform.Find("Units").transform;
            this.transform.position = targetTransform.position;
        }
    }

    private Sector RandomizeSector() {

        Sector[] sectors = map.GetSectors();
        List<int> numbers = new List<int>();
        for (int i = 0; i <= sectors.GetLength(0); i++) {
            numbers.Add(i+1);
        }
        Sector sect = findValidSector(sectors, numbers);
        if (sect != null)
        {
            sect.SetPunishmentCard(this);
        }
        return sect;

    }

    private Sector findValidSector(Sector[] sectors, List<int> nums) {
        int size = nums.Count;
        if (size == 0) {
            return null;
        }
        else {
            int rand = Mathf.RoundToInt(Random.Range(1, size));
            Sector sect = sectors[rand-1];
            if (sect.GetPunishmentCard() == null && sect.GetUnit() == null && sect.GetLandmark() == null && sect.IsVC() == false) {
                return sect;
            } else {
                nums.RemoveAt(rand-1);
                return findValidSector(sectors, nums);
            }
        }

        Debug.Log("You shouldn't be here lol");
        return null;
        
    }

	public void Use() {

		switch (effect) {
            case Effect.FreezeUnit:
                //Freeze unit card

                // show instruction window
                map.game.dialog.SetDialogType(Dialog.DialogType.ShowText);
                map.game.dialog.SetDialogData("Freeze a Unit", "Select a unit to freeze");
                map.game.dialog.Show();

                // set game turn state to SelectUnit with UseCard as the previous state
                map.game.prevState = Game.TurnState.UseCard;
                map.game.SetTurnState(Game.TurnState.SelectUnit);
				
                break;
			case Effect.SkipTurn:
                //Skip a turn
			//selectedPlayer.skipTurnOn();		-- need to come up with a way of selecting player, skip turn on and off ready
				game.OpenSkipTurnMenu();
				break;
			case Effect.NullifyResource:
                //Nullify resource
				break;
		}

        //this.map.game.RevertTurnState();

        Destroy(this.gameObject);
	}

    private void End() {
    }

    public void OnLoad(PunishmentCard savedData) {
        Game game = GameObject.Find("GameManager").GetComponent<Game>();
        this.playerID = savedData.playerID;
        this.owner = game.players[savedData.playerID];
        this.sectorID = savedData.sectorID;
        this.sector = game.sectors[savedData.sectorID];
        this.effect = savedData.effect;
}

}
