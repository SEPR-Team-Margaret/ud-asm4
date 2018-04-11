using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardUI : MonoBehaviour {

    [SerializeField] private Player player;
    [SerializeField] private UnityEngine.UI.Image background; 
    [SerializeField] private UnityEngine.UI.Text title;
    [SerializeField] private UnityEngine.UI.Text description;
    [SerializeField] private UnityEngine.UI.Text number;
    [SerializeField] private UnityEngine.UI.Button menuButton;
    [SerializeField] private UnityEngine.UI.Button endTurnButton;
    [SerializeField] private int index = 0;

    public void Initialize(Player player) {
        
        this.player = player;

        background = transform.Find("Background").GetComponent<UnityEngine.UI.Image>();
        title = transform.Find("Card_Title").GetComponent<UnityEngine.UI.Text>();
        description = transform.Find("Card_Description").GetComponent<UnityEngine.UI.Text>();
        number = transform.Find("Card_Number").GetComponentInChildren<UnityEngine.UI.Text>();
        menuButton = GameObject.Find("Menu_Button").GetComponent<UnityEngine.UI.Button>();
        endTurnButton = GameObject.Find("End_Turn_Button").GetComponent<UnityEngine.UI.Button>();

        index = 0;

        HideUI();

    }

    public void ShowUI() {

        index = 0;

        if (player.GetPunishmentCards().Count == 0)
        {
            player.GetGame().dialog.SetDialogType(Dialog.DialogType.ShowText);
            player.GetGame().dialog.SetDialogData("No Cards", "You have no punishment cards!");
            player.GetGame().dialog.Show();
			SoundManager.PlaySound ("turnPage");
            return;
        }

        ShowCard(index);
        this.gameObject.SetActive(true);
		SoundManager.PlaySound ("card");

        // set UI buttons to IgnoreRaycast layer
        menuButton.interactable = false;
        endTurnButton.interactable = false;

    }

    public void HideUI() {

        this.gameObject.SetActive(false);
		SoundManager.PlaySound ("cardReverse");

        // set UI buttons to UI layer
        menuButton.interactable = true;
        endTurnButton.interactable = true;

    }

    private void ShowCard(int i) {

        PunishmentCard card = player.GetPunishmentCards()[i];
        switch (card.GetEffect())
        {
            case PunishmentCard.Effect.FreezeUnit:
                title.text = "Freeze Unit";
                description.text = "Render one unit unable to move for 1 round.";
                break;

            case PunishmentCard.Effect.NullifyResource:
                title.text = "Nullify Resource";
                description.text = "Prevent one player from benefitting from resources for 1 round";
                break;

            case PunishmentCard.Effect.SkipTurn:
                title.text = "Skip A Turn";
                description.text = "Skip one player's next turn";
                break;
        }

        number.text = (index + 1) + " / " + player.GetPunishmentCards().Count;
    }

    public void ShowNextCard() {

        index += 1;
        if (index == player.GetPunishmentCards().Count)
        {
            index = 0;
        }

        ShowCard(index);

    }

    public void ShowPreviousCard() {

        index -= 1;
        if (index < 0)
        {
            index = (player.GetPunishmentCards().Count - 1);
        }

        ShowCard(index);

    }

    public void UseCard() {

        player.UseCard(index);
        HideUI();

    }
}
