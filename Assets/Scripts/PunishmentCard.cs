using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunishmentCard : MonoBehaviour {

    [SerializeField] private Player owner;
    [SerializeField] private Sector sector;
    [SerializeField] private enum Effect {Type1, Type2, Funny};

    #region Getters and Setters



    #endregion

    public void Initialize(Player player, Sector sector) {

        owner = player;
        SetSector(sector);

    }

    public void SetSector(Sector sector) {

    }
}
