using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardButton : MonoBehaviour {

    private Game game;

    void Start() {
        this.game = GameObject.Find("GameManager").GetComponent<Game>();
    }

    public void OpenCardMenu() {
        game.GetCurrentPlayer().GetCardUI().ShowUI();
    }

}
