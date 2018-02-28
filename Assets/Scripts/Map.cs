using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// stores the data for the game map
/// 
/// </summary>
public class Map : MonoBehaviour {

    public Game game;
    public Sector[] sectors;

    [SerializeField] private int maxPunishmentCardsOnMap = 3;
    [SerializeField] private int numPunishmentCardsOnMap = 0; // current number of punishment cards on map

    public int NumPunishmentCardsOnMap
    {
        get { return numPunishmentCardsOnMap; }
        set { numPunishmentCardsOnMap = value; }
    }

    public int MaxPunishmentCardsOnMap
    {
        get { return maxPunishmentCardsOnMap; }
    }
    /// <summary>
    /// 
    /// Returns the array of sectors belonging to this map
    /// 
    /// </summary>
    /// <returns>Array of sectors used in this game</returns>

    public Sector[] GetSectors() {
        return sectors;
    }



}
