using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardButton : MonoBehaviour {

    /* This class simply contains the behavior of the
     * button to show the Punishment Card selection UI
     */

    private Game game;

    void Start() {
        this.game = GameObject.Find("GameManager").GetComponent<Game>();
    }

    public void OpenCardMenu() {
        game.GetCurrentPlayer().GetCardUI().ShowUI();
    }

}
