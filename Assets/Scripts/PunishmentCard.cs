using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PunishmentCard : MonoBehaviour {

    [System.NonSerialized] private Player owner;
    [SerializeField] private int playerID;
    [System.NonSerialized] private Sector sector;
    [SerializeField] private int sectorID;
    private enum Effect {FreezeUnit, SkipTurn, NullifyResource};
    [System.NonSerialized] private Map map;
    [SerializeField] private Effect effect;

    #region Getters and Setters

    private void Start() {
        map = GameObject.Find("Map").GetComponent<Map>();

        if (map == null) {
            Debug.LogWarning("Map object or Map Script not found!");
        }

		Effect[] arrayOfEffects = { Effect.FreezeUnit, Effect.SkipTurn, Effect.NullifyResource };
		effect = arrayOfEffects [(Mathf.RoundToInt (Random.Range (0, 2)))];
    }

    public void SetSector(Sector sect) {
        sector = sect;
    }

    #endregion

    public void Initialize(Player player, Sector sector) {
        if (map.NumPunishmentCardsOnMap >= map.MaxPunishmentCardsOnMap) {
            Destroy(this);
        } else {
            map.NumPunishmentCardsOnMap += 1;
        }
        owner = player;
        sector = RandomizeSector();

    }

    private Sector RandomizeSector() {

        Sector[] sectors = map.GetSectors();
        List<int> numbers = new List<int>();
        for (int i = 0; i <= sectors.GetLength(0); i++) {
            numbers.Add(i+1);
        }
        Sector sect = findValidSector(sectors, numbers);
        sect.SetPunishmentCard(this);
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
            if (sect.GetPunishmentCard() == null && sect.GetUnit() == null) {
                return sect;
            } else {
                nums.RemoveAt(rand-1);
                findValidSector(sectors, nums);
            }
        }

        Debug.Log("You shouldn't be here lol");
        return null;
        
    }

	public void usePunishmentCard() {
		switch (effect) {
			case Effect.FreezeUnit:
				//Freeze unit card
				//pass in a selected unit
			//selectedUnit.FreezeUnit();
				break;
			case Effect.SkipTurn:
				//Skip a turn

				break;
			case Effect.NullifyResource:
				//Nullify resource
				break;
		}
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
