using UnityEngine;

/// <summary>
/// Serializable class to store all the properties of a game to initialise it
/// </summary>
[System.Serializable]
public class GameData
{
    // Define all properties that are needed to instantiate a Game
    // Must be public
    public Game.TurnState turnState;
    public bool gameFinished;
    public bool testMode;
    public int currentPlayerID; // Index from 0
    public int actionsRemaining;

    // Players
    // Attack
    public int[] playerAttack = new int[4];
    // Defence
    public int[] playerDefence = new int[4];

    // Color
    public Color[] playerColor = new Color[4];

    // Controller (Human, Neutral)
    public string[] playerController = new string[4];

    // Skip
    public bool[] playerSkip = new bool[4];

    // Sectors
    // Owner
    // Player ID index from 0, -1 for none (player1 index = 0)
    public int[] sectorOwner = new int[32];
    /*
    public int sector01Owner;
    public int sector02Owner;
    public int sector03Owner;
    public int sector04Owner;
    public int sector05Owner;
    public int sector06Owner;
    public int sector07Owner;
    public int sector08Owner;
    public int sector09Owner;
    public int sector10Owner;
    public int sector11Owner;
    public int sector12Owner;
    public int sector13Owner;
    public int sector14Owner;
    public int sector15Owner;
    public int sector16Owner;
    public int sector17Owner;
    public int sector18Owner;
    public int sector19Owner;
    public int sector20Owner;
    public int sector21Owner;
    public int sector22Owner;
    public int sector23Owner;
    public int sector24Owner;
    public int sector25Owner;
    public int sector26Owner;
    public int sector27Owner;
    public int sector28Owner;
    public int sector29Owner;
    public int sector30Owner;
    public int sector31Owner;
    public int sector32Owner;
    */

    // Level (-1 for none)
    public int[] sectorLevel = new int[32];
    /*
    public int sector01Level;
    public int sector02Level;
    public int sector03Level;
    public int sector04Level;
    public int sector05Level;
    public int sector06Level;
    public int sector07Level;
    public int sector08Level;
    public int sector09Level;
    public int sector10Level;
    public int sector11Level;
    public int sector12Level;
    public int sector13Level;
    public int sector14Level;
    public int sector15Level;
    public int sector16Level;
    public int sector17Level;
    public int sector18Level;
    public int sector19Level;
    public int sector20Level;
    public int sector21Level;
    public int sector22Level;
    public int sector23Level;
    public int sector24Level;
    public int sector25Level;
    public int sector26Level;
    public int sector27Level;
    public int sector28Level;
    public int sector29Level;
    public int sector30Level;
    public int sector31Level;
    public int sector32Level;
    */

    // Active  (-1 for none)
    public int sectorActive = -1;

    // Name
    public string[] sectorName = new string[32];

    // Frozen
    public bool[] sectorFrozen = new bool[32];

    // Frozen Counter
    public int[] sectorFrozenCounter = new int[32];

    // Vice Chancelor
    // Sector number
    public int VCSector;

    /// <summary>
    /// Fetches all data when called and assigns to properties
    /// </summary>
    /// <param name="game">The game to save</param>
    public void SetupGameData(Game game)
    {
        // Game properties
        this.turnState = game.GetTurnState();
        this.gameFinished = game.IsFinished();
        this.testMode = game.GetTestMode();
        this.currentPlayerID = game.GetPlayerID(game.GetCurrentPlayer());
        this.actionsRemaining = game.GetActionsRemaining();
        
        // Player properties
        Player[] players = game.GetPlayers();

        // Attack
        for (int i = 0; i < 4; i++) {
            this.playerAttack[i] = players[i].GetAttack();
        }

        // Defence
        for (int i = 0; i < 4; i++) {
            this.playerDefence[i] = players[i].GetDefence();
        }

        // Color
        for (int i = 0; i < 4; i++) {
            this.playerColor[i] = players[i].GetColor();
        }

        // Controller (Human, Neutral or None)
        for (int i = 0; i < 4; i++) {
            this.playerController[i] = players[i].GetController();
        }

        // Skip
        for (int i = 0; i < 4; i++) {
            this.playerSkip[i] = players[i].skipTurn;
        }

        // Sectors
        Sector[] sectors = game.GetSectors();

        // Owner
        for (int i = 0; i < 32; i++)
        {
            this.sectorOwner[i] = game.GetPlayerID(sectors[i].GetOwner());
        }
        /*
        this.sector01Owner = game.GetPlayerID(sectors[0].GetOwner());
        this.sector02Owner = game.GetPlayerID(sectors[1].GetOwner());
        this.sector03Owner = game.GetPlayerID(sectors[2].GetOwner());
        this.sector04Owner = game.GetPlayerID(sectors[3].GetOwner());
        this.sector05Owner = game.GetPlayerID(sectors[4].GetOwner());
        this.sector06Owner = game.GetPlayerID(sectors[5].GetOwner());
        this.sector07Owner = game.GetPlayerID(sectors[6].GetOwner());
        this.sector08Owner = game.GetPlayerID(sectors[7].GetOwner());
        this.sector09Owner = game.GetPlayerID(sectors[8].GetOwner());
        this.sector10Owner = game.GetPlayerID(sectors[9].GetOwner());
        this.sector11Owner = game.GetPlayerID(sectors[10].GetOwner());
        this.sector12Owner = game.GetPlayerID(sectors[11].GetOwner());
        this.sector13Owner = game.GetPlayerID(sectors[12].GetOwner());
        this.sector14Owner = game.GetPlayerID(sectors[13].GetOwner());
        this.sector15Owner = game.GetPlayerID(sectors[14].GetOwner());
        this.sector16Owner = game.GetPlayerID(sectors[15].GetOwner());
        this.sector17Owner = game.GetPlayerID(sectors[16].GetOwner());
        this.sector18Owner = game.GetPlayerID(sectors[17].GetOwner());
        this.sector19Owner = game.GetPlayerID(sectors[18].GetOwner());
        this.sector20Owner = game.GetPlayerID(sectors[19].GetOwner());
        this.sector21Owner = game.GetPlayerID(sectors[20].GetOwner());
        this.sector22Owner = game.GetPlayerID(sectors[21].GetOwner());
        this.sector23Owner = game.GetPlayerID(sectors[22].GetOwner());
        this.sector24Owner = game.GetPlayerID(sectors[23].GetOwner());
        this.sector25Owner = game.GetPlayerID(sectors[24].GetOwner());
        this.sector26Owner = game.GetPlayerID(sectors[25].GetOwner());
        this.sector27Owner = game.GetPlayerID(sectors[26].GetOwner());
        this.sector28Owner = game.GetPlayerID(sectors[27].GetOwner());
        this.sector29Owner = game.GetPlayerID(sectors[28].GetOwner());
        this.sector30Owner = game.GetPlayerID(sectors[29].GetOwner());
        this.sector31Owner = game.GetPlayerID(sectors[30].GetOwner());
        this.sector32Owner = game.GetPlayerID(sectors[31].GetOwner());
        */

        // Level
        for (int i = 0; i < 32; i++)
        {
            this.sectorLevel[i] = sectors[i].GetLevel();
        }
        /*
        this.sector01Level = sectors[0].GetLevel();
        this.sector02Level = sectors[1].GetLevel();
        this.sector03Level = sectors[2].GetLevel();
        this.sector04Level = sectors[3].GetLevel();
        this.sector05Level = sectors[4].GetLevel();
        this.sector06Level = sectors[5].GetLevel();
        this.sector07Level = sectors[6].GetLevel();
        this.sector08Level = sectors[7].GetLevel();
        this.sector09Level = sectors[8].GetLevel();
        this.sector10Level = sectors[9].GetLevel();
        this.sector11Level = sectors[10].GetLevel();
        this.sector12Level = sectors[11].GetLevel();
        this.sector13Level = sectors[12].GetLevel();
        this.sector14Level = sectors[13].GetLevel();
        this.sector15Level = sectors[14].GetLevel();
        this.sector16Level = sectors[15].GetLevel();
        this.sector17Level = sectors[16].GetLevel();
        this.sector18Level = sectors[17].GetLevel();
        this.sector19Level = sectors[18].GetLevel();
        this.sector20Level = sectors[19].GetLevel();
        this.sector21Level = sectors[20].GetLevel();
        this.sector22Level = sectors[21].GetLevel();
        this.sector23Level = sectors[22].GetLevel();
        this.sector24Level = sectors[23].GetLevel();
        this.sector25Level = sectors[24].GetLevel();
        this.sector26Level = sectors[25].GetLevel();
        this.sector27Level = sectors[26].GetLevel();
        this.sector28Level = sectors[27].GetLevel();
        this.sector29Level = sectors[28].GetLevel();
        this.sector30Level = sectors[29].GetLevel();
        this.sector31Level = sectors[30].GetLevel();
        this.sector32Level = sectors[31].GetLevel();
        */

        // Active
        for (int i = 0; i < 32; i++)
        {
            if (sectors[i].GetUnit() != null && sectors[i].GetUnit().IsSelected())
            {
                sectorActive = i;
                break;
            }
        }

        // Name
        for (int i = 0; i < 32; i++)
        {
            if (sectors[i].GetUnit() != null)
            {
                sectorName[i] = sectors[i].GetUnit().unitName;
            }
            else
            {
                sectorName[i] = null;
            }
        }

        // Frozen
        for (int i = 0; i < 32; i++)
        {
            if (sectors[i].GetUnit() != null)
            {
                sectorFrozen[i] = sectors[i].GetUnit().IsFrozen();
            }
            else
            {
                sectorFrozen[i] = false;
            }
        }

        // Frozen Counter
        for (int i = 0; i < 32; i++)
        {
            if (sectors[i].GetUnit() != null)
            {
                sectorFrozenCounter[i] = sectors[i].GetUnit().GetFrozenCounter();
            }
            else
            {
                sectorFrozenCounter[i] = 0;
            }
        }
        // Vice Chancelor
        this.VCSector = game.GetVCSectorID();
    }
}