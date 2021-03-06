﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour {
    
	[SerializeField] private Player player;
	[SerializeField] private UnityEngine.UI.Text header;
	[SerializeField] private UnityEngine.UI.Text headerHighlight;
	[SerializeField] private UnityEngine.UI.Text percentOwned;
	[SerializeField] private UnityEngine.UI.Text attack;
	[SerializeField] private UnityEngine.UI.Text defence;
	[SerializeField] private int numberOfSectors;
    [SerializeField] private GameObject nullified;
    [SerializeField] private GameObject skip;
	private Color defaultHeaderColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);

    /// <summary>
    /// 
    /// Loads all the components for a PlayerUI
    /// 
    /// </summary>
    /// <param name="player">The player object for whom this UI is for</param>
    /// <param name="player_id">ID of the player</param>
	public void Initialize(Player player, int player_id) {

		this.player = player;

		header = transform.Find("Header").GetComponent<UnityEngine.UI.Text>();
		headerHighlight = transform.Find("HeaderHighlight").GetComponent<UnityEngine.UI.Text>();
		percentOwned = transform.Find("PercentOwned_Value").GetComponent<UnityEngine.UI.Text>();
		attack = transform.Find("ATK_Value").GetComponent<UnityEngine.UI.Text>();
		defence = transform.Find("DEF_Value").GetComponent<UnityEngine.UI.Text>();
		numberOfSectors = player.GetGame().gameMap.GetComponent<Map>().sectors.Length;
        nullified = transform.Find("Nullified").gameObject;
        skip = transform.Find("Skip").gameObject;

		header.text = "Player " + player_id.ToString();
		headerHighlight.text = header.text;
		headerHighlight.color = player.GetColor();

        if (player.IsNeutral())
        {
            header.text = "Neutral";
            headerHighlight.text = header.text;
        }

        nullified.SetActive(false);
        skip.SetActive(false);
    }

    /// <summary>
    /// 
    /// Update the text labels in the UI
    /// 
    /// </summary>
	public void UpdateDisplay() {

		percentOwned.text = Mathf.Round(100 * player.ownedSectors.Count / numberOfSectors).ToString() + "%";
        attack.text = (player.GetAttack() + player.GetAttackBonus()).ToString();
        defence.text = (player.GetDefence() + player.GetDefenceBonus()).ToString();
        if (player.GetResourcesNullified())
        {
            nullified.SetActive(true);
        }
        else
        {
            nullified.SetActive(false);
        }
        if (player.skipTurn)
        {
            skip.SetActive(true);
        }
        else
        {
            skip.SetActive(false);
        }
	}

    /// <summary>
    /// 
    /// Highlight the player's name in the UI
    /// 
    /// </summary>
	public void Activate() {
		header.color = player.GetColor();
	}

    /// <summary>
    /// 
    /// Un-highlight the player's name in the UI
    /// 
    /// </summary>
	public void Deactivate() {
        header.color = defaultHeaderColor;
	}
}
