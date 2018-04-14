using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// stores the data for the game map
/// 
/// </summary>
public class Map : MonoBehaviour {

    /* Added Fields:
     * 
     *    - maxPunishmentCardsOnMap
     *    - numPunishmentCardsOnMap
     *    - adjacency
     */

    public Game game;
    public Sector[] sectors;
    private bool started = false;

    public int[][] adjacency = new int[32][];

    [SerializeField] private int maxPunishmentCardsOnMap = 3;
    [SerializeField] private int numPunishmentCardsOnMap = 0; // current number of punishment cards on map

    public void Start() {
        this.adjacency = new int[32][];
        SetAdjacency();
        this.started = true;
    }
        
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

    public void SetAdjacency() {
        this.adjacency[0] = new int[] { 2 };
        this.adjacency[1] = new int[] { 1, 3, 4, 5, 7 };
        this.adjacency[2] = new int[] { 2, 9, 4, 6 };
        this.adjacency[3] = new int[] { 2, 3, 5, 6 };
        this.adjacency[4] = new int[] { 2, 3, 5, 6 };
        this.adjacency[5] = new int[] { 3, 4, 5, 8 };
        this.adjacency[6] = new int[] { 2, 5, 8, 14, 15, 16 };
        this.adjacency[7] = new int[] { 5, 6, 7, 9, 10, 13, 15, 25 };
        this.adjacency[8] = new int[] { 3, 8, 10 };
        this.adjacency[9] = new int[] { 8, 9, 11, 13 };
        this.adjacency[10] = new int[] { 10, 12, 13 };
        this.adjacency[11] = new int[] { 11, 13, 26, 30 };
        this.adjacency[12] = new int[] { 8, 10, 11, 12, 25, 26 };
        this.adjacency[13] = new int[] { 7, 8, 15, 16, 25 };
        this.adjacency[14] = new int[] { 7, 8, 14 };
        this.adjacency[15] = new int[] { 7, 14, 17, 18, 19, 23, 24, 25 };
        this.adjacency[16] = new int[] { 16, 18 };
        this.adjacency[17] = new int[] { 16, 17, 19 };
        this.adjacency[18] = new int[] { 16, 18, 20 };
        this.adjacency[19] = new int[] { 19, 21, 22, 23 };
        this.adjacency[20] = new int[] { 20, 22, 23, 27, 32 };
        this.adjacency[21] = new int[] { 20, 21, 23 };
        this.adjacency[22] = new int[] { 16, 20, 21, 22, 24 };
        this.adjacency[23] = new int[] { 16, 23, 25, 26, 27 };
        this.adjacency[24] = new int[] { 13, 14, 16, 24, 26 };
        this.adjacency[25] = new int[] { 12, 13, 24, 25, 27, 29, 30 };
        this.adjacency[26] = new int[] { 21, 23, 24, 28, 32 };
        this.adjacency[27] = new int[] { 26, 27, 29, 31 };
        this.adjacency[28] = new int[] { 26, 28, 30, 31 };
        this.adjacency[29] = new int[] { 12, 26, 29, 31 };
        this.adjacency[30] = new int[] { 28, 29, 30, 32 };
        this.adjacency[31] = new int[] { 21, 27, 31 };
    }

    /// <summary>
    /// 
    /// Gets the array of sectors that are adjacent to
    /// the sector with the provided ID
    /// 
    /// </summary>
    /// <returns>The array of adjacent sectors.</returns>
    /// <param name="sectorID">Sector ID.</param>
    public Sector[] GetAdj(int sectorID) {
        if (!started) {
            SetAdjacency();
            started = true;
        }
        Sector[] sectors = new Sector[this.adjacency[sectorID].Length];
        for (int i = 0; i < sectors.Length; i++) {
            sectors[i] = ToSector(this.adjacency[sectorID][i]-1);
        }
        return sectors;
    }

    /// <summary>
    /// 
    /// Takes a sector ID and returns the corresponding sector.
    /// </summary>
    /// <returns>The sector.</returns>
    /// <param name="sectorID">Sector ID.</param>
    public Sector ToSector(int sectorID) {
        return game.sectors[sectorID];
    }



}
