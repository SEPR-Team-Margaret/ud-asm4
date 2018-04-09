using UnityEngine;
using System.Collections;
using UnityEngine.TestTools;

public class TestSetup 
{
    private Game game;
    private Map map;
    private Player[] players;
    private PlayerUI[] gui;
    private GameObject unitPrefab;
    private GameObject book;
    private Dialog dialog;


    public TestSetup()
    {
        Setup();
    }
    
    private void Setup()
    {
        // initialize the game, map, and players with any references needed
        // the "GameManager" asset contains a copy of the GameManager object
        // in the 4x4 Test, but its script lacks references to players & the map
        game = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GameManager 1")).GetComponent<Game>();

        // the "Map" asset is a copy of the 4x4 Test map, complete with
        // adjacent sectors and landmarks at (0,1), (1,3), (2,0), and (3,2),
        // but its script lacks references to the game & sectors
        map = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Map")).GetComponent<Map>();
            
        // the "GUI" asset contains the PlayerUI object for each Player
        gui = MonoBehaviour.Instantiate(Resources.Load<GameObject>("GUI")).GetComponentsInChildren<PlayerUI>();

        unitPrefab = Resources.Load<GameObject>("Unit");

        book = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Book"));

        dialog = game.gameObject.GetComponentInChildren<Dialog>();


        // the "Scenery" asset contains the camera and light source of the 4x4 Test
        // can uncomment to view scene as tests run, but significantly reduces speed
        //MonoBehaviour.Instantiate(Resources.Load<GameObject>("Scenery"));


        // set up references between objects
        game.gameMap = map.gameObject;
        game.eliminatedPlayers = new bool[4];
        game.SetActionsRemainingLabel(book.gameObject.GetComponentInChildren<UnityEngine.UI.Text>());
        game.gameObject.name = "GameManager";

        map.game = game;
        map.sectors = map.GetComponentsInChildren<Sector>();
        map.gameObject.name = "Map";

        game.players[0].SetColor(Color.red);
        game.players[1].SetColor(Color.magenta);
        game.players[2].SetColor(Color.yellow);
        game.players[3].SetColor(Color.green);

        for (int i = 0; i < game.players.Length; i++) {
            game.players[i].SetGui(gui[i]);
            game.players[i].SetGame(game);
            game.players[i].GetGui().Initialize(game.players[i], i + 1);
            game.players[i].SetUnitPrefab((GameObject)Resources.Load("Unit"));
        }
        GameObject.Find("PlayerNeutral").GetComponent<Player>().SetGui(GameObject.Find("PlayerNeutralUI").GetComponent<PlayerUI>());

        players = game.GetPlayers();

        dialog.texture = Resources.Load<GameObject>("DialogTexture");

        // enable test mode
        game.EnableTestMode();

    }

    private IEnumerator Hold()
    {
        yield return null;
    }

    public Game GetGame()
    {
        return this.game;
    }

    public Map GetMap()
    {
        return this.map;
    }

    public Player[] GetPlayers()
    {
        return this.players;
    }

    public PlayerUI[] GetPlayerUIs()
    {
        return this.gui;
    }

    public GameObject GetUnitPrefab()
    {
        return this.unitPrefab;
    }

    public GameObject GetBook() 
    {
        return book;
    }
    
}