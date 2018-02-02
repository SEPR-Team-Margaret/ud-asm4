using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Class created by Jack (01/02/2018)
public class Dialog : MonoBehaviour {

    public GameObject texture;

    private DialogType type;

    public enum DialogType
    {
        EndGame, PlayerElimated
    }
    
    public void setDialogType(DialogType type)
    {
        // Updates the dialog with the different buttons needed for each mode
        this.type = type;
        switch (type)
        {
            case DialogType.EndGame: 
                texture.transform.GetChild(0).GetComponent<Text>().text = "GAME OVER!"; // Hides the unnecessary buttons and shows the others
                texture.transform.GetChild(2).gameObject.SetActive(false);
                texture.transform.GetChild(3).gameObject.SetActive(true);
                texture.transform.GetChild(4).gameObject.SetActive(true);
                break;
            case DialogType.PlayerElimated:
                texture.transform.GetChild(0).GetComponent<Text>().text = "ELIMINATED!";
                texture.transform.GetChild(2).gameObject.SetActive(true);
                texture.transform.GetChild(3).gameObject.SetActive(false);
                texture.transform.GetChild(4).gameObject.SetActive(false);
                break;
        }
    }

    public void setPlayerName(string name)
    {

        switch (type)
        {
            case DialogType.EndGame:
                texture.transform.GetChild(1).GetComponent<Text>().text = name + " WON!";
                break;
            case DialogType.PlayerElimated:
                texture.transform.GetChild(1).GetComponent<Text>().text = name + " was eliminated";
                break;
        }
    }

    public void Show()
    {
        texture.SetActive(true);
    }

    public void Close()
    {
        Debug.Log("Closing");
        texture.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

}
