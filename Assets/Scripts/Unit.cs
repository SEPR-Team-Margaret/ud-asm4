using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Unit : MonoBehaviour {

    /* Added Fields:
     * 
     *     - unitName
     *     - unitFrozen
     *     - frozenCounter
     *     - sprite
     *     - popup
     *     - popupElevated
     *     - popupName
     *     - popupLevel
     *     - popupDefaultTextColor
     *     - popupFrozenTextColor
     */

    [System.NonSerialized] private Player owner;
    [System.NonSerialized] private Sector sector;
    [SerializeField] private int level;
    [SerializeField] private bool selected = false;
    [SerializeField] public string unitName;
    [SerializeField] private bool unitFrozen = false; 
    [SerializeField] private int frozenCounter = 0;

    [SerializeField] private UnitSprite sprite;
    [System.NonSerialized] private GameObject popup;
    [System.NonSerialized] private bool popupElevated = false;
    [System.NonSerialized] private UnityEngine.UI.Text popupName;
    [System.NonSerialized] private UnityEngine.UI.Text popupLevel;
    private Color popupDefaultTextColor = new Color(0.196f, 0.196f, 0.196f, 1.0f);
    private Color popupFrozenTextColor = new Color(0.196f, 0.33f, 1.0f, 1.0f);

    /// <summary>
    /// 
    /// Initializes the unit on the passed sector and assigns it to the passed player
    /// The unit is set to level 1
    /// The unit's colour is set to the colour of the player that owns it
    /// 
    /// </summary>
    /// <param name="player">The player the unit belongs to</param>
    /// <param name="sector">The sector the unit is on</param>
    public void Initialize(Player player, Sector sector) {

        // set the owner, level, and color of the unit
        owner = player;
        level = 1;
        

        // generate a name for the unit
        unitName = GenerateName();

        // get a sprite for the unit
        sprite = new UnitSprite(gameObject);
        sprite.GenerateRandomSprite();

        // get references to the UI elements of the unit popup
        popup = gameObject.transform.Find("Popup").gameObject;
        popupName = popup.transform.Find("Name").GetComponent<UnityEngine.UI.Text>();
        popupLevel = popup.transform.Find("Level").GetComponent<UnityEngine.UI.Text>();

        // initialize the unit popup
        popupName.text = unitName;
        popupLevel.text = "1st Year";

        // place the unit in the sector
        MoveTo(sector);

        Material backgroundMat = new Material(Shader.Find("Specular"));
        Color ownerColor = owner.GetColor();
        Color offset = new Color(0.4f, 0.4f, 0.4f);
        backgroundMat.color = ownerColor - offset;
        gameObject.transform.Find("Background").GetComponent<MeshRenderer>().material = backgroundMat;

    }

    /// <summary>
    /// 
    /// Generates a random name.
    /// 
    /// </summary>
    /// <returns>A random name.</returns>
    private string GenerateName() {

        string[] firstNames = new string[] 
            { "James", "John" , "Rob"  , "Mike" , "Pat"  , "Will" , "David", "Joe"  , "Tom"  , "Jack" , 
              "Dan"  , "Paul" , "Mark" , "Steve", "Ed"   , "Brian", "Al"   , "Harry", "Adam" , "Nick" ,
              "Ron"  , "Tony" , "Kevin", "Jason", "Matt" , "Gary" , "Tim"  , "Jose" , "Larry", "Roger",
              "Jeff" , "Frank", "Scott", "Eric" , "Andy" , "Ray"  , "Greg" , "Josh" , "Jerry", "Ralph",
              "Sam"  , "Ben"  , "Luke" , "Ollie", "Marty", "Bruce" , "Louis", "Peter", "Roy"  , "Todd" ,
              "Mary" , "Linda", "Barb" , "Liz"  , "Jenny", "Maria", "Susan", "Marge", "Lilly", "Rosa" ,
              "Lisa" , "Nancy", "Karen", "Betty", "Helen", "Sandy", "Donna", "Carol", "Tina" , "Cindy",
              "Ruth" , "Laura", "Sarah", "Kim"  , "Debby", "Jess" , "Amy"  , "Anna" , "Kathy", "Ellen",
              "Pam"  , "Jane" , "Anne" , "Chris", "Jean" , "Rose" , "Irene", "Judy" , "Emily", "Emma" ,
              "Ava"  , "Wendy", "Clara", "Jill" , "Erica", "Dana" , "Beth" , "Tara" , "Lucy" , "Elena"
            };
        string[] lastNames = new string[] 
            { "Smith" , "Jones" , "Brown" , "Miller", "Taylor", "Wilson", "Davis" , "White" , "Clark" ,
              "Hall"  , "Thomas", "Moore" , "Hill"  , "Walker", "Wrigt" , "Martin", "Wood"  , "Allen" ,
              "Lewis" , "Scott" , "Young" , "Adams" , "Green" , "Evans" , "King"  , "Baker" , "John"  ,
              "Harris", "James" , "Lee"   , "Turner", "Parker", "Cook"  , "Morris", "Bell"  , "Ward"  ,
              "Watson", "Morgan", "Davies", "Cooper", "Rogers", "Gray"  , "Hughes", "Carter", "Murphy",
              "Henry" , "Foster", "Shaw"  , "Reed"  , "Howard", "Fisher", "May"   , "Church", "Mills" ,
              "Kelly" , "Price" , "Murray", "Palmer", "Cox"   , "Bailey", "Nelson", "Mason" , "Butler",
              "Hunt"  , "Graham", "Ross"  , "Stone" , "Porter", "Gibson", "West"  , "Brooks", "Ellis" ,
              "Barnes", "Wells" , "Hart"  , "Ford"  , "Cole"  , "Fox"   , "Holmes", "Day"   , "Long"  ,
              "Grant" , "Hunter", "Webb"  , "Gordon", "Perry" , "Black" , "Lane"  , "Warren", "Burns"
            };

        string firstName = (string) firstNames.GetValue(Random.Range(0, firstNames.Length));
        string lastName = (string) lastNames.GetValue(Random.Range(0, lastNames.Length));

        return (firstName + " " + lastName);

    }

    /// <summary>
    /// 
    /// returns the player that owns this unit
    /// 
    /// </summary>
    /// <returns>The player that owns this unit</returns>
    public Player GetOwner() {
        return owner;
    }

    /// <summary>
    /// 
    /// Sets the player that owns this unit
    /// 
    /// </summary>
    /// <param name="owner">The player that owns this unit</param>
    public void SetOwner(Player owner) {
        this.owner = owner;
    }

    /// <summary>
    /// 
    /// returns the sector that this unit is in
    /// 
    /// </summary>
    /// <returns>The sector that this unit is in</returns>
    public Sector GetSector() {
        return sector;
    }

    /// <summary>
    /// 
    /// sets the sector that this unit is in
    /// 
    /// </summary>
    /// <param name="sector">The sector that this unit is on</param>
    public void SetSector(Sector sector) {
        this.sector = sector;
    }

    /// <summary>
    /// 
    /// Returns this unit's level
    /// 
    /// </summary>
    /// <returns>This unit's level</returns>
    public int GetLevel() {
        return level;
    }

    /// <summary>
    /// 
    /// Sets this units level
    /// 
    /// </summary>
    /// <param name="level">This units level</param>
    public void SetLevel(int level) {
        this.level = level;
    }

    /// <summary>
    /// 
    /// Returns this units colour
    /// 
    /// </summary>
    /// <returns>This units colour</returns>
    private Color GetColor() {
        return owner.GetColor();
    }

    /// <summary>
    /// 
    /// Gets the sprite.
    /// 
    /// </summary>
    /// <returns>The sprite.</returns>
    public UnitSprite GetSprite() {
        return sprite;
    }

    /// <summary>
    /// 
    /// Sets the sprite.
    /// 
    /// </summary>
    /// <param name="sprite">Sprite.</param>
    public void SetSprite(UnitSprite sprite) {
        this.sprite = sprite;
    }


    /// <summary>
    /// 
    /// Returns if this unit has been selected
    /// 
    /// </summary>
    /// <returns>True if this unit is selected and false otherwise</returns>
    public bool IsSelected() {
        return selected;
    }

    /// <summary>
    /// 
    /// sets if this unit is currently selected
    /// 
    /// </summary>
    /// <param name="selected">True if unit is seclected else false</param>
    public void SetSelected(bool selected) {
        this.selected = selected;
    }

    /// <summary>
    /// 
    /// Moves this unit to the passed sector
    /// If the unit moves to a sector they do not own then LevelUp is called on it
    /// 
    /// </summary>
    /// <param name="targetSector">The sector to move this unit to</param>
    public void MoveTo(Sector targetSector) {

        if (this.unitFrozen == false) {
            // clear the unit's current sector
            if (this.sector != null) {
                this.sector.ClearUnit();
            }

            // set the unit's sector to the target sector
            // and the target sector's unit to the unit
            this.sector = targetSector;
            targetSector.SetUnit(this);
            Transform targetTransform = targetSector.transform.Find("Units").transform;

            // set the unit's transform to be a child of
            // the target sector's transform
            transform.SetParent(targetTransform);

            // align the transform to the sector
            transform.position = targetTransform.position;

            // if the target sector belonged to a different 
            // player than the unit, level up
            if (targetSector.GetOwner() != this.owner) {
                LevelUp();
            }

            // capture the target sector for the owner of this unit
            owner.Capture(targetSector);

        }
        else {
            Debug.Log("This unit is frozen!");
        }

    }

    /// <summary>
    /// 
    /// switch the position of this unit and the passed unit
    /// 
    /// </summary>
    /// <param name="otherUnit">The unit to be swapped with this one</param>
    public void SwapPlacesWith(Unit otherUnit) {

        // swap the sectors' references to the units
        this.sector.SetUnit(otherUnit);
        otherUnit.sector.SetUnit(this);


        // get the index of this unit's sector in the map's list of sectors
        int tempSectorIndex = -1;
        for (int i = 0; i < this.owner.GetGame().gameMap.GetComponent<Map>().sectors.Length; i++) {
            if (this.sector == this.owner.GetGame().gameMap.GetComponent<Map>().sectors[i])
                tempSectorIndex = i;
        }

        // swap the units' references to their sectors
        this.sector = otherUnit.sector;
        otherUnit.sector = this.owner.GetGame().gameMap.GetComponent<Map>().sectors[tempSectorIndex];

        // realign transforms for each unit
        this.transform.SetParent(this.sector.transform.Find("Units").transform);
        this.transform.position = this.sector.transform.Find("Units").position;

        otherUnit.transform.SetParent(otherUnit.sector.transform.Find("Units").transform);
        otherUnit.transform.position = otherUnit.sector.transform.Find("Units").position;

    }

    /// <summary>
    /// 
    /// increase this units level and update the unit model to display the new level
    /// levelling up is capped at level 5
    /// 
    /// </summary>
	public void LevelUp() {

        // level up the unit, capping at Level 5

        if (level < 5) {

            // increase level
            level++;
        }

        // update unit level in popup

        switch (level)
        {
            case 1:
                popupLevel.text = "1st Year";
                break;

            case 2:
                popupLevel.text = "2nd Year";
                break;

            case 3:
                popupLevel.text = "3rd Year";
                break;

            case 4:
                popupLevel.text = "4th Year";
                break;

            case 5:
                popupLevel.text = "Postgrad";
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// 
    /// select the unit
    /// 
    /// </summary>
    public void Select() {

        selected = true;
    }

    /// <summary>
    /// 
    /// deselect the unit
    /// 
    /// </summary>
    public void Deselect() {

        selected = false;
    }

    /// <summary>
    /// 
    /// safely destroy the unit by removing it from its owner's list of units before destroying
    /// 
    /// </summary>
    public void DestroySelf() {
        sector.ClearUnit();
        owner.units.Remove(this);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 
    /// Freezes the unit.
    /// 
    /// </summary>
    public void FreezeUnit() {
        unitFrozen = true;
        frozenCounter = 3;
        popupName.color = popupFrozenTextColor;
        popupLevel.color = popupFrozenTextColor;
        SoundManager.PlaySound("goose");
    }

    /// <summary>
    /// 
    /// Unfreezes unit.
    /// 
    /// </summary>
    public void UnFreezeUnit() {
        unitFrozen = false;
        frozenCounter = 0;
        popupName.color = popupDefaultTextColor;
        popupLevel.color = popupDefaultTextColor;
        SoundManager.PlaySound("goose");
    }

    /// <summary>
    /// 
    /// Returns whether this unit is frozen.
    /// 
    /// </summary>
    /// <returns><c>true</c> if this unit is frozen; otherwise, <c>false</c>.</returns>
    public bool IsFrozen() {
        return unitFrozen;
    }

    /// <summary>
    /// 
    /// Gets the value of the unit's frozen counter.
    /// 
    /// </summary>
    /// <returns>The value of the unit's frozen counter.</returns>
    public int GetFrozenCounter() {
        return frozenCounter;
    }

    /// <summary>
    /// 
    /// Decrements the unit's frozen counter.
    /// 
    /// </summary>
    public void DecrementFrozenCounter() {
        frozenCounter--;
    }

    /// <summary>
    /// 
    /// Sets unit's parameters based on the information stored in the saved data
    /// and places the unit in the sector specified by the provided ID
    /// 
    /// </summary>
    /// <param name="savedData">Saved data.</param>
    /// <param name="sectorID">Sector ID.</param>
    public void OnLoad(GameData savedData, int sectorID) {
        
        Game game = GameObject.Find("GameManager").GetComponent<Game>();
        this.owner = game.players[savedData.sectorOwner[sectorID]];
        this.sector = game.sectors[sectorID];
        this.level = savedData.sectorLevel[sectorID];
        this.unitName = savedData.sectorName[sectorID];
        this.unitFrozen = savedData.sectorFrozen[sectorID];
        this.frozenCounter = savedData.sectorFrozenCounter[sectorID];
        this.sprite.SetHat(savedData.sectorSpriteHat[sectorID]);
        this.sprite.SetHead(savedData.sectorSpriteHead[sectorID]);
        this.sprite.SetBody(savedData.sectorSpriteBody[sectorID]);
        this.popupName.text = this.unitName;
        this.level -= 1;
        this.LevelUp();
        this.gameObject.SetActive(true);

		this.owner.units.Add (this);
    }

    /// <summary>
    /// 
    /// When the mouse first hovers over the unit, display its popup
    /// 
    /// </summary>
    void OnMouseOver() {

        // display the unit's popup
        popup.SetActive(true);

        // elevate the unit so that the popup appears on top of any nearby units
        if (!popupElevated)
        {
            transform.Translate(new Vector3(0.0f, 0.0f, 0.5f));
            popupElevated = true;
        }

        // carry the OnMouseOver event into the unit's sector
        sector.OnMouseEnterAccessible();
    }

    /// <summary>
    /// 
    /// When the mouse is no longer hovering over the unit, hide its popup
    /// 
    /// </summary>
    void OnMouseExit() {

        // hide the unit's popup
        popup.SetActive(false);

        // return the unit to its original position
        transform.Translate(new Vector3(0.0f, 0.0f, -0.5f));
        popupElevated = false;

        // carry the OnMouseExit event into the unit's sector
        sector.OnMouseExitAccessible();
    }

    /// <summary>
    /// 
    /// When the unit is clicked as a button, carry the event into the unit's sector
    /// 
    /// </summary>
    void OnMouseUpAsButton() {
        sector.OnMouseUpAsButtonAccessible();
    }

}