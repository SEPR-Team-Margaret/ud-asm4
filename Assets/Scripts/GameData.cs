using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializable class to store all the properties of a game to initialise it
/// </summary>
[System.Serializable]
public class GameData
{
    /* Added Fields:
     * 
     *    - actionsRemaining
     *    - playerNullified
     *    - playerNullifiedCounter
     *    - playerPunishmentCards
     *    - playerSkip
     *    - sectorName
     *    - sectorFrozen
     *    - sectorFrozenCounter
     *    - sectorPunishmentCard
     * 
     * Modified Fields:
     *    - playerAttackBonus   (altered to store bonus from minigame)
     *    - playerDefenceBonus  (altered to store bonus from minigame)
     *    - sectorOwner         (changed from 32 individual int fields to one int[32] field)
     *    - sectorLevel         (changed from 32 individual int fields to one int[32] field)
     */

    // Define all properties that are needed to instantiate a Game
    // Must be public
    public Game.TurnState turnState;
    public bool gameFinished;
    public bool testMode;
    public int currentPlayerID; // Index from 0
    public int actionsRemaining;

    // Players
    // Attack Bonus from PVC minigame
    public int[] playerAttackBonus = new int[4];
    // Defence Bonus from PVC minigame
    public int[] playerDefenceBonus = new int[4];

    // Resources Nullified
    public bool[] playerNullified = new bool[4];

    // Resources Nullified Counter
    public int[] playerNullifiedCounter = new int[4];

    // Color
    public Color[] playerColor = new Color[4];

    // Controller (Human, Neutral)
    public string[] playerController = new string[4];

    // Punishment Cards [FreezeUnit, NullifyResource, SkipTurn]
    public string[] playerPunishmentCards = new string[4];

    // Skip
    public bool[] playerSkip = new bool[4];

    // Sectors
    // Owner
    // Player ID index from 0, -1 for none (player1 index = 0)
    public int[] sectorOwner = new int[32];

    // Level (-1 for none)
    public int[] sectorLevel = new int[32];

    // Active  (-1 for none)
    public int sectorActive = -1;

    // Name
    public string[] sectorName = new string[32];

    // Frozen
    public bool[] sectorFrozen = new bool[32];

    // Frozen Counter
    public int[] sectorFrozenCounter = new int[32];

    // Punishment Card 
    public bool[] sectorPunishmentCard = new bool[32];
    public PunishmentCard.Effect[] sectorPunishmentCardEffect = new PunishmentCard.Effect[32];

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
            this.playerAttackBonus[i] = players[i].GetAttackBonus();
        }

        // Defence
        for (int i = 0; i < 4; i++) {
            this.playerDefenceBonus[i] = players[i].GetDefenceBonus();
        }

        // Resources Nullified
        for (int i = 0; i < 4; i++) {
            this.playerNullified[i] = players[i].GetResourcesNullified();
        }

        // Resources Nullified Counter
        for (int i = 0; i < 4; i++) {
            this.playerNullifiedCounter[i] = players[i].GetResourcesNullifiedCounter();
        }

        // Color
        for (int i = 0; i < 4; i++) {
            this.playerColor[i] = players[i].GetColor();
        }

        // Controller (Human, Neutral or None)
        for (int i = 0; i < 4; i++) {
            this.playerController[i] = players[i].GetController();
        }

        // Punishment Cards [FreezeUnit, NullifyResource, SkipTurn]
        for (int i = 0; i < 4; i++) {

            // get a list of the player's punishment cards
            List<PunishmentCard> punishmentCards = players[i].GetPunishmentCards();

            // initialize counters for each type of card
            int freezeUnitCards = 0;
            int nullifyResourceCards = 0;
            int skipTurnCards = 0;

            // count the number of each type of card the player has
            for (int j = 0; j < punishmentCards.Count; j++)
            {
                if (punishmentCards[j].GetEffect() == PunishmentCard.Effect.FreezeUnit)
                {
                    freezeUnitCards += 1;
                }
                else if (punishmentCards[j].GetEffect() == PunishmentCard.Effect.NullifyResource)
                {
                    nullifyResourceCards += 1;
                }
                else if (punishmentCards[j].GetEffect() == PunishmentCard.Effect.SkipTurn)
                {
                    skipTurnCards += 1;
                }
            }

            // encode the number of cards of each type into a string
            this.playerPunishmentCards[i] = freezeUnitCards.ToString() + "_" + nullifyResourceCards.ToString() + "_" + skipTurnCards.ToString();

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

        // Level
        for (int i = 0; i < 32; i++)
        {
            this.sectorLevel[i] = sectors[i].GetLevel();
        }

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

        // Punishment Card
        for (int i = 0; i < 32; i++)
        {
            if (sectors[i].GetPunishmentCard() == null)
            {
                sectorPunishmentCard[i] = false;
            }
            else
            {
                sectorPunishmentCard[i] = true;
                sectorPunishmentCardEffect[i] = sectors[i].GetPunishmentCard().GetEffect();
            }
        }

        // Vice Chancelor
        this.VCSector = game.GetVCSectorID();
    }
}