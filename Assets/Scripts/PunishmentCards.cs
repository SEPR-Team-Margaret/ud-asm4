using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunishmentCards : MonoBehaviour {

    [SerializeField] private Player owner;
    [SerializeField] private int numPunishmentCards;

    #region Getters and Setters

    /// <summary>
    /// 
    /// Gets the number of Punishment Cards this player has
    /// 
    /// </summary>
    public int GetPunishmentCards() {
        return numPunishmentCards;
    }

    /// <summary>
    /// 
    /// Sets the number of Punishment Cards this player has
    /// 
    /// </summary>
    /// <param name="val">An integer value that numPunishmentCards will be set to, if val is greater than or equal to 0.</param>
    public void SetPunishmentCards(int val) {
        if (val >= 0) {
            numPunishmentCards = val;
        }
        else {
            Debug.LogWarning("numPunishmentCards is being set to " + val + ", which is less than 0.");
        }

    }

    /// <summary>
    /// 
    /// Sets the number of Punishment Cards this player has.
    /// Defaults to 0 if numPunishmentCards becomes less than 0.
    /// 
    /// </summary>
    /// <param name="val">An integer value that numPunishmentCards will have added to it.</param>
    public void AddPunishmentCards(int val) {
        numPunishmentCards += val;

        if (numPunishmentCards < 0) {
            numPunishmentCards = 0;
            Debug.LogWarning("Punishment Cards is being set to a value less than 0");
        }
    }

    #endregion

}
