using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Class created by Jack (01/02/2018)
public class Dialog : MonoBehaviour
{

	private Game game;

    public GameObject texture;

    private DialogType type;

	public Dropdown skipPlayer;
	List<string> listElements = new List<string>();

    public enum DialogType
    {
        EndGame, PlayerElimated, SaveQuit, ShowText, SelectTurnSkip, SelectNullifyResource
    }

    /// <summary>
    /// 
    /// Sets the game.
    /// 
    /// </summary>
    /// <param name="game">Game.</param>
    public void SetGame(Game game) {
        this.game = game;
    }

    /// <summary>
    /// 
    /// Sets up the dialog in the format of the passed dialog type
    /// 
    /// </summary>
    /// <param name="type">The type that this dialog should be set up in the form of</param>
    public void SetDialogType(DialogType type)
    {
		// Update the dialog with the different buttons needed for each mode
        this.type = type;
        switch (type)
        {
            case DialogType.EndGame:
                texture.transform.GetChild(0).GetComponent<Text>().text = "GAME OVER!"; // Hides the unnecessary buttons and shows the others
                texture.transform.GetChild(1).gameObject.SetActive(false);
                texture.transform.GetChild(2).gameObject.SetActive(false);
                texture.transform.GetChild(3).gameObject.SetActive(true);
                texture.transform.GetChild(4).gameObject.SetActive(true);
                texture.transform.GetChild(5).gameObject.SetActive(false);
                texture.transform.GetChild(6).gameObject.SetActive(false);
                texture.transform.GetChild(7).gameObject.SetActive(true);
                texture.transform.GetChild(8).gameObject.SetActive(false);
                texture.transform.GetChild(9).gameObject.SetActive(false);
                texture.transform.GetChild(10).gameObject.SetActive(false);
                break;
            case DialogType.PlayerElimated:
                texture.transform.GetChild(0).GetComponent<Text>().text = "ELIMINATED!";
                texture.transform.GetChild(1).gameObject.SetActive(true);
                texture.transform.GetChild(2).gameObject.SetActive(true);
                texture.transform.GetChild(3).gameObject.SetActive(false);
                texture.transform.GetChild(4).gameObject.SetActive(false);
                texture.transform.GetChild(5).gameObject.SetActive(false);
                texture.transform.GetChild(6).gameObject.SetActive(false);
                texture.transform.GetChild(7).gameObject.SetActive(true);
                texture.transform.GetChild(8).gameObject.SetActive(false);
                texture.transform.GetChild(9).gameObject.SetActive(false);
                texture.transform.GetChild(10).gameObject.SetActive(false);
                break;
            case DialogType.SaveQuit:
                texture.transform.GetChild(0).GetComponent<Text>().text = "PAUSED";
                texture.transform.GetChild(1).GetComponent<Text>().text = "";
                texture.transform.GetChild(2).gameObject.SetActive(false);
                texture.transform.GetChild(3).gameObject.SetActive(false);
                texture.transform.GetChild(4).gameObject.SetActive(true);
                texture.transform.GetChild(5).gameObject.SetActive(true);
                texture.transform.GetChild(6).gameObject.SetActive(true);
                texture.transform.GetChild(7).gameObject.SetActive(true);
                texture.transform.GetChild(8).gameObject.SetActive(false);
                texture.transform.GetChild(9).gameObject.SetActive(false);
                texture.transform.GetChild(10).gameObject.SetActive(false);
                break;
            case DialogType.ShowText:
                texture.transform.GetChild(0).GetComponent<Text>().text = "";
                texture.transform.GetChild(1).gameObject.SetActive(true);
                texture.transform.GetChild(2).gameObject.SetActive(true);
                texture.transform.GetChild(3).gameObject.SetActive(false);
                texture.transform.GetChild(4).gameObject.SetActive(false);
                texture.transform.GetChild(5).gameObject.SetActive(false);
                texture.transform.GetChild(6).gameObject.SetActive(true);
                texture.transform.GetChild(7).gameObject.SetActive(true);
                texture.transform.GetChild(8).gameObject.SetActive(false);
                texture.transform.GetChild(9).gameObject.SetActive(false);
                texture.transform.GetChild(10).gameObject.SetActive(false);
                break;
			case DialogType.SelectTurnSkip:
				CreateSkipPlayerList ();
				texture.transform.GetChild(0).GetComponent<Text>().text = "SKIP TURN";
				texture.transform.GetChild(1).GetComponent<Text>().text = "Select a player to\nmiss a turn";
				texture.transform.GetChild(2).gameObject.SetActive(false);
				texture.transform.GetChild(3).gameObject.SetActive(false);
				texture.transform.GetChild(4).gameObject.SetActive(false);
				texture.transform.GetChild(5).gameObject.SetActive(false);
				texture.transform.GetChild(6).gameObject.SetActive(false);
				texture.transform.GetChild(7).gameObject.SetActive(true);
                texture.transform.GetChild(8).gameObject.SetActive(true);
                texture.transform.GetChild(9).gameObject.SetActive(true);
                texture.transform.GetChild(10).gameObject.SetActive(false);
				break;
            case DialogType.SelectNullifyResource:
                CreateSkipPlayerList();
                texture.transform.GetChild(0).GetComponent<Text>().text = "NULLIFY RESOURCE";
                texture.transform.GetChild(1).GetComponent<Text>().text = "Select a player to nullify\ntheir resource bonus";
                texture.transform.GetChild(2).gameObject.SetActive(false);
                texture.transform.GetChild(3).gameObject.SetActive(false);
                texture.transform.GetChild(4).gameObject.SetActive(false);
                texture.transform.GetChild(5).gameObject.SetActive(false);
                texture.transform.GetChild(6).gameObject.SetActive(false);
                texture.transform.GetChild(7).gameObject.SetActive(true);
                texture.transform.GetChild(8).gameObject.SetActive(true);
                texture.transform.GetChild(9).gameObject.SetActive(false);
                texture.transform.GetChild(10).gameObject.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 
    /// Sets the player's name in this dialog
    /// 
    /// </summary>
    /// <param name="name">The player's name who this dialog refers to</param>
    public void SetDialogData(string data)
    {
        switch (type)
        {
            case DialogType.EndGame:
                texture.transform.GetChild(1).GetComponent<Text>().text = data + " WON!";
                break;
            case DialogType.PlayerElimated:
                texture.transform.GetChild(1).GetComponent<Text>().text = data + "\nwas eliminated";
                break;
        }
    }

    /// <summary>
    /// 
    /// Creates a dialog with specific header and body text
    /// 
    /// </summary>
    /// <param name="header">Header text</param>
    /// <param name="body">Body text</param>
    public void SetDialogData(string header, string body)
    {
        switch (type)
        {
            case DialogType.ShowText:
                texture.transform.GetChild(0).GetComponent<Text>().text = header;
                texture.transform.GetChild(1).GetComponent<Text>().text = body;
                break;
        }
    }

    /// <summary>
    /// 
    /// Displays this dialog
    /// 
    /// </summary>
    public void Show()
    {
        texture.SetActive(true);
		SoundManager.PlaySound ("turnPage");

        try {
        
            // set UI buttons to be uninteractable
            game.DisableUIButtons();

        } catch (System.NullReferenceException) {
            Debug.Log("could not disable gui buttons");
        }
    }

    /// <summary>
    /// 
    /// Closes this dialog
    /// 
    /// </summary>
    public void Close()
    {
        Debug.Log("Closing");
        GameObject info = GameObject.Find("Info");
        UnityEngine.UI.Text text = info.GetComponent<UnityEngine.UI.Text>();
        text.lineSpacing = 1.0f;

        try {
            
            // set UI buttons to be interactable
            game.EnableUIButtons();

        } catch (System.NullReferenceException) {
            Debug.Log("could not re-enable gui buttons");
        }

        texture.SetActive(false);
    }

    /// <summary>
    /// 
    /// Changes to the previous sce3ne
    /// 
    /// </summary>
    public void Exit()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    /// <summary>
    /// 
    /// Loads the main menu scene
    /// 
    /// </summary>
    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

	/// <summary>
	/// 
	/// Creates the list of players, not the current player, and adds them as options to the skip player dropdown
	/// 
	/// </summary>
	public void CreateSkipPlayerList(){
		listElements.Clear ();
		foreach (Player player in game.players){
			if (game.currentPlayer != player) {
                if (player.playerID == 3 && !player.IsHuman())
                {
                    listElements.Add("Neutral");
                }
                else
                {
                    listElements.Add("Player " + (player.playerID + 1));
                }
			}
		}
		Debug.Log (listElements);
		skipPlayer = texture.transform.GetChild (8).GetComponent<Dropdown> ();
		skipPlayer.ClearOptions();
		skipPlayer.AddOptions(listElements);
	}

	public int GetSelectedPlayer(){
		return skipPlayer.value;
	}
}