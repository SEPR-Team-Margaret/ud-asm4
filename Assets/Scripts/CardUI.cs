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

    /// <summary>
    /// 
    /// Initializes this CardUI to be owned by the specified player.
    /// 
    /// </summary>
    /// <param name="player">Player.</param>
    public void Initialize(Player player) {

        // set player reference
        this.player = player;

        // get necessary references to UI elements
        background = transform.Find("Background").GetComponent<UnityEngine.UI.Image>();
        title = transform.Find("Card_Title").GetComponent<UnityEngine.UI.Text>();
        description = transform.Find("Card_Description").GetComponent<UnityEngine.UI.Text>();
        number = transform.Find("Card_Number").GetComponentInChildren<UnityEngine.UI.Text>();
        menuButton = GameObject.Find("Menu_Button").GetComponent<UnityEngine.UI.Button>();
        endTurnButton = GameObject.Find("End_Turn_Button").GetComponent<UnityEngine.UI.Button>();

        // set default card index
        index = 0;

        // hide this CardUI
        gameObject.SetActive(false);

    }

    /// <summary>
    /// 
    /// Shows this CardUI.
    /// 
    /// </summary>
    public void ShowUI() {

        // reset the card index to the default
        index = 0;

        // if the player has no punishment cards, display a dialog that says so instead of the CardUI
        if (player.GetPunishmentCards().Count == 0)
        {
            player.GetGame().dialog.SetDialogType(Dialog.DialogType.ShowText);
            player.GetGame().dialog.SetDialogData("No Cards", "You have no\npunishment cards!");
            player.GetGame().dialog.Show();
			//SoundManager.PlaySound ("turnPage");
            return;
        }

        // otherwise, display the CardUI
        ShowCard(index);
        this.gameObject.SetActive(true);
		SoundManager.PlaySound ("card");

        // set game UI buttons to be uninteractable to avoid opening
        // the menu or ending the turn while selecting a card
        menuButton.interactable = false;
        endTurnButton.interactable = false;

    }

    /// <summary>
    /// 
    /// Hides this CardUI.
    /// 
    /// </summary>
    public void HideUI() {

        // hide the CardUI
        this.gameObject.SetActive(false);
		SoundManager.PlaySound ("cardReverse");

        // set game UI buttons to be interactable again
        menuButton.interactable = true;
        endTurnButton.interactable = true;

    }

    /// <summary>
    /// 
    /// Shows the card at index 'i' in the player's list of cards
    /// 
    /// </summary>
    /// <param name="i">The index of the card to display in the player's list of cards.</param>
    private void ShowCard(int i) {

        // get the card to display
        PunishmentCard card = player.GetPunishmentCards()[i];

        // display the relevant information depending on the card's effect
        switch (card.GetEffect())
        {
            case PunishmentCard.Effect.FreezeUnit:
                title.text = "Goose Attack!"/*"Freeze Unit"*/;
                title.fontSize = 18;
                description.text = "Make a unit\nunable to move\nfor one round.";
                break;

            case PunishmentCard.Effect.NullifyResource:
                title.text = "Industrial Action!"/*"Nullify Resource"*/;
                title.fontSize = 16;
                description.text = "Nullifies one\nplayer's bonus\nfrom resources\nfor one round.";
                break;

            case PunishmentCard.Effect.SkipTurn:
                title.text = "Hangover!"/*"Skip A Turn"*/;
                title.fontSize = 18;
                description.text = "Skip one player's\nnext turn.";
                break;
        }

        // display the index of the card as a fraction
        number.text = (index + 1) + " / " + player.GetPunishmentCards().Count;
    }

    /// <summary>
    /// 
    /// Shows the next card in the player's list of cards,
    /// or shows the first card if the last card in the list
    /// is already shown.
    /// 
    /// </summary>
    public void ShowNextCard() {

        // get the index of the next card 
        index += 1;
        if (index == player.GetPunishmentCards().Count)
        {
            index = 0;
        }

        // show the next card
        ShowCard(index);

    }

    /// <summary>
    /// 
    /// Shows the previous card in the player's list of cards,
    /// or shows the last card if the first card in the list
    /// is already shown.
    /// 
    /// </summary>
    public void ShowPreviousCard() {

        // get the index of the previous card
        index -= 1;
        if (index < 0)
        {
            index = (player.GetPunishmentCards().Count - 1);
        }

        // show the previous card
        ShowCard(index);

    }

    /// <summary>
    /// 
    /// Uses the currently-displayed card and closes the CardUI.
    /// 
    /// </summary>
    public void UseCard() {

        HideUI();
        player.UseCard(index);

    }
}
