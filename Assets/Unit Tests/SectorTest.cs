using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class SectorTest 
{
	private Game game;
	private Map map;
    private Player[] players;
    private PlayerUI[] gui;
    private GameObject book;

    private void Setup()
    {
        TestSetup t = new TestSetup();
        this.game = t.GetGame();
        this.map = t.GetMap();
        this.players = t.GetPlayers();
        this.gui = t.GetPlayerUIs();
        this.book = t.GetBook();

        game.InitializeMap();
    }

    private void Cleanup() {

        GameObject.Destroy(game.gameObject);
        GameObject.Destroy(map.gameObject);
        GameObject.Destroy(gui[0].GetComponentInParent<Canvas>().gameObject);
        GameObject.Destroy(book);

    }

    [UnityTest]
    public IEnumerator SetOwner_SectorOwnerAndColorCorrect() {
        
        Setup();

        Sector sector = map.sectors[0];
        sector.SetOwner(null);
        Player player = players[0];

        sector.SetOwner(player);
        Assert.AreSame(sector.GetOwner(), player);
        Assert.IsTrue(sector.gameObject.GetComponent<Renderer>().material.color.Equals(player.GetColor()));

        sector.SetOwner(null);
        Assert.IsNull(sector.GetOwner());
        Assert.IsTrue(sector.gameObject.GetComponent<Renderer>().material.color.Equals(sector.GetDefaultColor()));

        Cleanup();

        yield return null;
    }

    [UnityTest]
    public IEnumerator Initialize_OwnedAndNotOwnedSectorsOwnerAndColor() {
        
        Setup();

        Sector sectorWithoutLandmark = map.sectors[1];
        Sector sectorWithLandmark = map.sectors[0];

        sectorWithoutLandmark.Initialize(0);
        Assert.IsNull(sectorWithoutLandmark.GetOwner());
        Assert.IsTrue(sectorWithoutLandmark.gameObject.GetComponent<Renderer>().material.color.Equals(sectorWithoutLandmark.GetDefaultColor()));
        Assert.IsNull(sectorWithoutLandmark.GetUnit());
        Assert.IsNull(sectorWithoutLandmark.GetLandmark());

        sectorWithLandmark.Initialize(0);
        Assert.IsNull(sectorWithLandmark.GetOwner());
        Assert.IsTrue(sectorWithLandmark.gameObject.GetComponent<Renderer>().material.color.Equals(sectorWithoutLandmark.GetDefaultColor()));
        Assert.IsNull(sectorWithLandmark.GetUnit());
        Assert.IsNotNull(sectorWithLandmark.GetLandmark());

        Cleanup();

        yield return null;
    }

    /* This test needs updating to new RevertHighlight Standards
     * [UnityTest]
    public IEnumerator Highlight_SectorColourCorrect() {
        
        Setup();

        Sector sector = map.sectors[0];
        sector.gameObject.GetComponent<Renderer>().material.color = Color.gray;
        float amount = 0.2f;
        Color highlightedGray = Color.gray + (Color) (new Vector4(amount, amount, amount, 1));

        sector.ApplyHighlight(amount);
        Assert.IsTrue(sector.gameObject.GetComponent<Renderer>().material.color.Equals(highlightedGray));

        sector.RevertHighlight(amount);
        Assert.IsTrue(sector.gameObject.GetComponent<Renderer>().material.color.Equals(Color.gray));

        yield return null;
    }*/


    [UnityTest]
    public IEnumerator ClearUnit_UnitRemovedFromSector() {
        
        Setup();

//        game.InitializeMap();

        Sector sector = map.sectors[0];
        Assert.NotNull(sector.GetUnit());

        sector.ClearUnit();
        Assert.IsNull(sector.GetUnit());

        Cleanup();

        yield return null;
    }

    [UnityTest]
    public IEnumerator OnMouseAsButton_CorrectUnitIsSelected() {
        
        Setup();

//        game.InitializeMap();

        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];
        Sector sectorC = map.sectors[2];
        Player playerA = players[0];
        Player playerB = players[1];
        Unit unitA = MonoBehaviour.Instantiate(players[0].GetUnitPrefab()).GetComponent<Unit>();
        Unit unitB = MonoBehaviour.Instantiate(players[0].GetUnitPrefab()).GetComponent<Unit>(); // *should this be players[1]?* ###########################################################################################

        // ensure sectors A & B are adjacent to each other
        Assert.Contains(sectorA, sectorB.GetAdjacentSectors());
        Assert.Contains(sectorB, sectorA.GetAdjacentSectors());

        // ensure sectors A & C are not adjacent to each other
        foreach (Sector sector in sectorA.GetAdjacentSectors())
        {
            Assert.IsFalse(sector == sectorC);
        }
        foreach (Sector sector in sectorC.GetAdjacentSectors())
        {
            Assert.IsFalse(sector == sectorA);
        }

        sectorA.SetOwner(playerA);
        sectorA.ClearUnit();
        unitA.Initialize(playerA, sectorA);

        unitB.SetOwner(playerB);

        playerA.units.Add(unitA);
        playerB.units.Add(unitB);


        // test clicking a sector with a unit while the unit's owner
        // is active AND there are no units selected
        playerA.SetActive(true);
        unitA.SetSelected(false);
        unitB.SetSelected(false);

        sectorA.OnMouseUpAsButtonAccessible();
        Assert.IsTrue(unitA.IsSelected());

        // test clicking on the sector containing the selected unit
        sectorA.OnMouseUpAsButtonAccessible();
        Assert.IsFalse(unitA.IsSelected());


        // test clicking a sector with a unit while there are no
        // units selected, but the unit's owner is NOT active
        playerA.SetActive(false);
        unitA.SetSelected(false);
        unitB.SetSelected(false);

        sectorA.OnMouseUpAsButtonAccessible();
        Assert.IsFalse(unitA.IsSelected());


        // test clicking a sector with a unit while the unit's owner
        // is active, but there IS another unit selected
        playerA.SetActive(true);
        unitA.SetSelected(false);
        unitB.SetSelected(true);

        sectorA.OnMouseUpAsButtonAccessible();
        Assert.IsFalse(unitA.IsSelected());


        // test clicking on a sector adjacent to a selected unit
        unitA.SetSelected(true);
        unitB.SetSelected(false);
        game.SetTurnState(Game.TurnState.SelectUnit);

        sectorB.OnMouseUpAsButtonAccessible();
        Assert.IsFalse(unitA.IsSelected());

        // only need to test deselection;
        // other interactions covered in smaller tests below

        Cleanup();

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoveIntoUnoccupiedSector_NewSectorHasUnitAndOldDoesNotAndTurnStateProgressed() {
        
        Setup();

        game.SetTurnState(Game.TurnState.Move);
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];

        MonoBehaviour.Instantiate(players[0].GetUnitPrefab()).GetComponent<Unit>().Initialize(players[0],sectorA);
//        sectorA.SetUnit(MonoBehaviour.Instantiate(players[0].GetUnitPrefab()).GetComponent<Unit>());
//        sectorA.GetUnit().SetSector(sectorA);
        sectorB.SetUnit(null);

        sectorB.MoveIntoUnoccupiedSector(sectorA.GetUnit());
        Assert.IsNotNull(sectorB.GetUnit());
        Assert.IsNull(sectorA.GetUnit());
 //       Assert.IsTrue(game.GetTurnState() == Game.TurnState.Move2);

        Cleanup();

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoveIntoFriendlyUnit_UnitsSwapSectorsAndTurnStateProgressed() {
        
        Setup();

        game.SetTurnState(Game.TurnState.Move);
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];

        sectorA.SetUnit(MonoBehaviour.Instantiate(players[0].GetUnitPrefab()).GetComponent<Unit>());
        sectorA.GetUnit().SetSector(sectorA);
        sectorA.GetUnit().SetLevel(5);
        sectorA.GetUnit().SetOwner(players[0]);
        sectorA.SetOwner(players[0]);

        sectorB.SetUnit(MonoBehaviour.Instantiate(players[0].GetUnitPrefab()).GetComponent<Unit>());
        sectorB.GetUnit().SetSector(sectorB);
        sectorB.GetUnit().SetLevel(1);
        sectorB.GetUnit().SetOwner(players[0]);
        sectorB.SetOwner(players[0]);

        sectorB.MoveIntoFriendlyUnit(sectorA.GetUnit());
        Assert.IsTrue(sectorA.GetUnit().GetLevel() == 1); // level 1 unit now in sectorA
        Assert.IsTrue(sectorB.GetUnit().GetLevel() == 5); // level 2 unit now in sectorB => units have swapped locations
  //      Assert.IsTrue(game.GetTurnState() == Game.TurnState.Move2);

        Cleanup();

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoveIntoHostileUnit_AttackingUnitTakesSectorAndLevelUpAndTurnEnd() {
        
        Setup();

        game.SetTurnState(Game.TurnState.Move);
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];

        // for all tests, sectorA's unit will be the attacking unit
        // and sectorB's unit will be the defending unit

        // setup units such that the attacking unit wins
        ResetSectors(sectorA, sectorB);
        sectorA.GetOwner().SetAttack(99); // to ensure the sectorA unit will win any conflict (attacking)
        sectorB.GetOwner().SetDefence(0);

        Unit unit = sectorA.GetUnit();
        Debug.Log(unit.GetLevel());
        sectorB.MoveIntoHostileUnit(sectorA.GetUnit(), sectorB.GetUnit());
        Debug.Log(unit.GetLevel());
        Assert.IsNull(sectorA.GetUnit()); // attacking unit moved out of sectorA
        Assert.IsTrue(sectorB.GetUnit().GetLevel() == 2); // attacking unit that moved to sectorB gained a level (the unit won the conflict)
 //       Assert.IsTrue(game.GetTurnState() == Game.TurnState.EndOfTurn);

        Cleanup();

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoveIntoHostileUnit_DefendingUnitDefendsSectorAndTurnEnd() {
        
        Setup();

        game.SetTurnState(Game.TurnState.Move);
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];

        // for all tests, sectorA's unit will be the attacking unit
        // and sectorB's unit will be the defending unit

        // setup units such that the defending unit wins
        game.SetTurnState(Game.TurnState.Move);
        ResetSectors(sectorA, sectorB);
        sectorA.GetOwner().SetAttack(0);
        sectorB.GetOwner().SetDefence(99); //to ensure the sectorB unit will win any conflict (defending)

        sectorB.MoveIntoHostileUnit(sectorA.GetUnit(), sectorB.GetUnit());
        Assert.IsNull(sectorA.GetUnit()); // attacking unit destroyed
        Assert.IsTrue(sectorB.GetUnit().GetLevel() == 1); // defending unit did not gain a level following defence
 //       Assert.IsTrue(game.GetTurnState() == Game.TurnState.EndOfTurn);

        Cleanup();

        yield return null;
    }

    [UnityTest]
    public IEnumerator MoveIntoHostileUnit_TieConflict_DefendingUnitDefendsSectorAndTurnEnd() {
        
        Setup();

        game.SetTurnState(Game.TurnState.Move);
        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];

        // for all tests, sectorA's unit will be the attacking unit
        // and sectorB's unit will be the defending unit

        // setup units such that there is a tie (defending unit wins)

        // *** UNITCONTROLLER DESTROYSELF METHOD NEEDS TO CLEAR UNIT ***

        game.SetTurnState(Game.TurnState.Move);
        ResetSectors(sectorA, sectorB);
        sectorA.GetUnit().SetLevel(-4);
        sectorA.GetOwner().SetAttack(0);
        sectorB.GetUnit().SetLevel(-4);
        sectorB.GetOwner().SetDefence(0); // making both units equal

        sectorB.MoveIntoHostileUnit(sectorA.GetUnit(), sectorB.GetUnit());
        Assert.IsNull(sectorA.GetUnit()); // attacking unit destroyed
        Assert.IsTrue(sectorB.GetUnit().GetLevel() == -4); // defending unit did not gain a level following defence
 //       Assert.IsTrue(game.GetTurnState() == Game.TurnState.EndOfTurn);

        Cleanup();

        yield return null;
    }

    [UnityTest]
    public IEnumerator AdjacentSelectedUnit_SectorsAreAdjacent() {
        
        Setup();

//        game.InitializeMap();

        Sector sectorA = map.sectors[0];
        Sector sectorB = map.sectors[1];

        // ensure sectors A and B are adjacent to each other
        Assert.Contains(sectorA, sectorB.GetAdjacentSectors());
        Assert.Contains(sectorB, sectorA.GetAdjacentSectors());

        // test with no unit in adjacent sector
        Assert.IsNull(sectorB.AdjacentSelectedUnit());

        // test with unselected unit in adjacent sector
        sectorA.SetUnit(MonoBehaviour.Instantiate(players[0].GetUnitPrefab()).GetComponent<Unit>());
        sectorA.GetUnit().SetSelected(false);
        Assert.IsNull(sectorB.AdjacentSelectedUnit());

        // test with selected unit in adjacent sectors
        sectorA.GetUnit().SetSelected(true);
        Assert.IsNotNull(sectorB.AdjacentSelectedUnit());

        Cleanup();

        yield return null;
    }
    
    private void ResetSectors(Sector sectorA, Sector sectorB) {
        
        // re-initialize sectors for in between test cases in MoveIntoHostileUnitTest

        sectorA.SetOwner(players[0]);
        sectorA.SetUnit(MonoBehaviour.Instantiate(players[0].GetUnitPrefab()).GetComponent<Unit>());
        sectorA.GetUnit().Initialize(players[0], sectorA);

        sectorB.SetOwner(players[1]);
        sectorB.SetUnit(MonoBehaviour.Instantiate(players[1].GetUnitPrefab()).GetComponent<Unit>());
        sectorB.GetUnit().Initialize(players[1], sectorB);
    }
}