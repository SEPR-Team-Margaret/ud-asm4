using UnityEngine;

public class Sector : MonoBehaviour {

    /* Added Field:
     * 
     *    - punishmentCard
     */

    [SerializeField] public int sectorID;
    [System.NonSerialized] private Color DEFAULT_COLOR = new Color(0.8f,0.8f,0.8f/*0.83f,0.87f,0.87f*/);

    [System.NonSerialized] private Map map;
    [SerializeField] private Unit unit;
    [System.NonSerialized] private Player owner;
    [SerializeField] private int ownerID;
    [System.NonSerialized] private Sector[] adjacentSectors;
    [SerializeField] private Landmark landmark;
    [SerializeField] private bool VC = false;
    [SerializeField] private PunishmentCard punishmentCard;

    [SerializeField] private bool isHighlighted;


    #region Getters and Setters

    /// <summary>
    /// 
    /// Gets the default color for the sector.
    /// 
    /// </summary>
    /// <returns>The default color for the sector.</returns>
    public Color GetDefaultColor() {
        return this.DEFAULT_COLOR;
    }

    /// <summary>
    /// 
    /// Returns if this sector contains the vice chancelor
    /// 
    /// </summary>
    /// <returns>True if sector contains vice chancellor else false</returns>
    public bool IsVC()
    {
        return VC;
    }

    /// <summary>
    /// 
    /// Sets if this sector contains the vice chancellor
    /// 
    /// </summary>
    /// <param name="VC">True if this sector should contain the VC else false</param>
    public void SetVC(bool VC)
    {
        this.VC = VC;
    }
    
    /// <summary>
    /// 
    /// fetches the unit object on this sector
    /// </summary>
    /// 
    /// <returns>The unit on this sector</returns>
    public Unit GetUnit() {
        return unit;
    }

    /// <summary>
    /// 
    /// Sets the unit on this sector
    /// 
    /// </summary>
    /// <param name="unit">The unit that is to be put on this sector</param>
    public void SetUnit(Unit unit) {
        this.unit = unit;
    }

    /// <summary>
    /// 
    /// fetches the Player object for the player that owns this sector
    /// 
    /// </summary>
    /// <returns>The Player object of the player that owns this sector</returns>
    public Player GetOwner() {
        return owner;
    }

    /// <summary>
    /// 
    /// sets the owner of this sector to the passed player
    /// updates the colour of the sector to that of the player
    /// 
    /// if passed player is null resets the sector colour back to grey
    /// 
    /// </summary>
    /// <param name="owner">Player object of the new owner of this sector or null if there is no owner</param
    public void SetOwner (Player owner) {

        // set sector owner to the given player
        this.owner = owner;
        if (owner == null) {
            this.ownerID = -1;
        } else { 
            this.ownerID = owner.playerID;
        }

        // set sector color to the color of the given player
        // or gray if null
        Renderer renderer = GetComponent<Renderer>();
        if (owner == null) {
            Debug.Log(DEFAULT_COLOR);
            //SEE HERE
            renderer.material.color = DEFAULT_COLOR;
        } else {
            renderer.material.color = owner.GetColor();
        }
    }

    /// <summary>
    /// 
    /// sets the owner of this sector to the passed player without updating the colour
    /// for testing only
    /// 
    /// </summary>
    /// <param name="owner">Player object of the new owner of this sector or null if there is no owner</param
    public void SetOwnerNoColour(Player owner)
    {
        this.owner = owner;
    }
    
    /// <summary>
    /// Get the level of the unit on the sector
    /// </summary>
    /// <returns>Int value for the id of the sector</returns>
    public int GetLevel()
    {
        if (this.unit == null)
        {
            return -1;
        }
        else
        {
            return this.unit.GetLevel();
        } 
    }

    /// <summary>
    /// 
    /// returns an array of sectors that are adjacent to this one
    /// 
    /// </summary>
    /// <returns>Array of sectors that are adjacent to this sector</returns>
    public Sector[] GetAdjacentSectors() {
        return adjacentSectors;
    }

    /// <summary>
    /// 
    /// returns the landmark of this sector or null if it does not have one
    /// 
    /// </summary>
    /// <returns>The landmark object for this sector; May return null if sector does not have a landmark</returns>
	public Landmark GetLandmark() {
        return landmark;
    }

    /// <summary>
    /// 
    /// Sets the landmark object of this sector to the passed landmark
    /// 
    /// </summary>
    /// <param name="landmark">Landmark object to be set on this sector</param>
	public void SetLandmark(Landmark landmark) {
        this.landmark = landmark;
    }

    public PunishmentCard GetPunishmentCard() {
        return punishmentCard;
    }
    public void SetPunishmentCard(PunishmentCard pc) {
        this.punishmentCard = pc;
    }

    public bool IsHighlighted() {
        return isHighlighted;
    }
    public void SetIsHighlighted(bool flag) {
        this.isHighlighted = flag;
    }

    #endregion

    /// <summary>
    /// 
    /// initializes a sector
    /// determines if the sector contains a landmark
    /// sets owner and unit to null
    /// 
    /// </summary>
    public void Initialize(int id) {

        this.sectorID = id;

        this.map = GameObject.Find("Map").GetComponent<Map>();

        this.adjacentSectors = map.GetAdj(id);

		// set no owner
		SetOwner(null);

		// clear unit
		unit = null;

		// get landmark (if any)
		landmark = gameObject.GetComponentInChildren<Landmark>();

	}

    /// <summary>
    /// 
    /// highlight a sector by increasing its RGB values by a specified amount
    /// 
    /// </summary>
    /// <param name="amount"></param>
    public void ApplyHighlight(float amount) {
        if (!this.IsHighlighted()) {
            Renderer renderer = GetComponent<Renderer>();
            Color currentColor = renderer.material.color;
            Color offset = new Vector4(amount, amount, amount, 1);
            Color newColor = currentColor + offset;

            renderer.material.color = newColor;

            SetIsHighlighted(true);
        }
    }

    /// <summary>
    /// 
    /// unhighlight a sector by decreasing its RGB values by a specified amount
    /// 
    /// </summary>
    /// <param name="amount"></param>
    public void RevertHighlight() {
        if (this.IsHighlighted()) {
            Renderer renderer = GetComponent<Renderer>();
            Color currentColor = renderer.material.color;
            
            if(this.owner != null) {
                renderer.material.color = this.owner.GetColor();
            } else {
                //SEE HERE
                renderer.material.color = DEFAULT_COLOR;
            }

            SetIsHighlighted(false);
        }
    }

    /// <summary>
    /// 
    /// highlight each sector adjacent to this one
    /// 
    /// </summary>
    public void ApplyHighlightAdjacent() {
        foreach (Sector adjacentSector in adjacentSectors)
        {
            if (!adjacentSector.IsHighlighted()) {
                adjacentSector.ApplyHighlight(0.2f);
                adjacentSector.SetIsHighlighted(true);
            }
        }
    }

    /// <summary>
    /// 
    /// unhighlight each sector adjacent to this one
    /// 
    /// </summary>
    public void RevertHighlightAdjacent() {
        foreach (Sector adjacentSector in adjacentSectors)
        {
            if (adjacentSector.IsHighlighted()) {
                adjacentSector.RevertHighlight();
                adjacentSector.SetIsHighlighted(false);
            }
        }
    }

    /// <summary>
    /// 
    /// clear this sector of any unit
    /// 
    /// </summary>
    public void ClearUnit() {


        unit = null;
    }

    void OnMouseUpAsButton () {

        // when this sector is clicked, determine the context
        // and act accordingly

		OnMouseUpAsButtonAccessible();

    }
    
    public void OnMouseUpAsButtonAccessible() {

        Debug.Log("CurrentState: "+map.game.GetTurnState() + " | PrevState: "+ map.game.prevState.ToString());
        // a method of OnMouseUpAsButton that is 
        // accessible to other objects for testing


        // if this sector contains a unit and belongs to the
        // current active player, and if no unit is selected
        if (map.game.GetTurnState() == Game.TurnState.Move/*1 || map.game.GetTurnState() == Game.TurnState.Move2*/)
        {
            if (unit != null && owner.IsActive() && map.game.NoUnitSelected())
            {
                if (unit.IsFrozen())
                {
                    // show info dialog
                    map.game.dialog.SetDialogType(Dialog.DialogType.ShowText);
                    map.game.dialog.SetDialogData("Frozen!", (unit.unitName + " can't move"));
                    map.game.dialog.Show();
                    return;
                }
                // select this sector's unit
                unit.Select();
                this.ApplyHighlightAdjacent();
                this.map.game.prevState = map.game.GetTurnState();
                this.map.game.SetTurnState(Game.TurnState.SelectUnit);
            }
        }
        else if (map.game.GetTurnState() == Game.TurnState.SelectUnit && map.game.prevState != Game.TurnState.UseCard)
        {
            // if this sector's unit is already selected
            if (unit != null && unit.IsSelected())
            {
                // deselect this sector's unit           
                unit.Deselect();
                this.RevertHighlightAdjacent();
                this.map.game.RevertTurnState();
            }
            // if this sector is adjacent to the sector containing the selected unit
            else if (AdjacentSelectedUnit() != null)
            {
                // get the selected unit
                Unit selectedUnit = AdjacentSelectedUnit();

                // deselect the selected unit
                selectedUnit.Deselect();
                Sector OriginSector = selectedUnit.GetSector();

                // if this sector is unoccupied
                if (unit == null)
                    MoveIntoUnoccupiedSector(selectedUnit);

                // if the sector is occupied by a friendly unit
                else if (unit.GetOwner() == selectedUnit.GetOwner())
                    MoveIntoFriendlyUnit(selectedUnit);

                // if the sector is occupied by a hostile unit
                else if (unit.GetOwner() != selectedUnit.GetOwner())
                    MoveIntoHostileUnit(selectedUnit, this.unit);

                OriginSector.RevertHighlight();
                OriginSector.RevertHighlightAdjacent();
                /*
                if (map.game.prevState == Game.TurnState.Move)
                {
                    Debug.Log("Apply Highlighting");
                    this.ApplyHighlight(0.4f);
                }
                */
                map.game.NextTurnState(); // advance to next turn phase when action take (Modified by Dom 13/02/2018)
                
            }

        }
        else if (map.game.GetTurnState() == Game.TurnState.SelectUnit && map.game.prevState == Game.TurnState.UseCard)
        {
            // freeze the selected unit
            if (unit != null && unit.GetOwner() != map.game.currentPlayer)
            {
                // show info dialog
                map.game.dialog.SetDialogType(Dialog.DialogType.ShowText);
                map.game.dialog.SetDialogData("Frozen!", (unit.unitName + " can't move"));
                map.game.dialog.Show();

                unit.FreezeUnit();
                map.game.prevState = map.game.GetTurnState();
                map.game.SetTurnState(Game.TurnState.Move);
            }
        }
    }

    /// <summary>
    /// 
    /// Moves the passed unit onto this sector
    /// should only be used when this sector is unoccupied
    /// 
    /// </summary>
    /// <param name="unit">The unit to be moved onto this sector</param>
    public void MoveIntoUnoccupiedSector(Unit unit) {
        
        // move the selected unit into this sector
        unit.MoveTo(this);
    }

    /// <summary>
    /// 
    /// switches the unit on this sector with the passed one
    /// 
    /// </summary>
    /// <param name="otherUnit">Unit object of the unit on the adjacent sector to be switched onto this sector</param>
    public void MoveIntoFriendlyUnit(Unit otherUnit) {

        if (unit.IsFrozen())
        {
            // show info dialog
            map.game.dialog.SetDialogType(Dialog.DialogType.ShowText);
            map.game.dialog.SetDialogData("Frozen!", (unit.unitName + " can't move"));
            map.game.dialog.Show();

            // restore wasted action
            map.game.SetActionsRemaining(map.game.GetActionsRemaining() + 1);
            return;
        }

        // swap the two units
        this.unit.SwapPlacesWith(otherUnit);
    }

    /// <summary>
    /// 
    /// initates a combat encounter between a pair of units
    /// the loosing is destroyed
    /// if the attacker wins then they move onto the defending units territory
    /// 
    /// </summary>
    /// <param name="attackingUnit"></param>
    /// <param name="defendingUnit"></param>
    public void MoveIntoHostileUnit(Unit attackingUnit, Unit defendingUnit) {
        
        // if the attacking unit wins
        if (Conflict(attackingUnit, defendingUnit))
        {
            // destroy defending unit
            defendingUnit.DestroySelf();
            GameObject gameManagerObject = GameObject.Find("GameManager");
            Game game = gameManagerObject.GetComponent<Game>();

            game.eliminatedUnits.Add(defendingUnit.unitName);

            // move the attacking unit into this sector
            attackingUnit.MoveTo(this);
        }

        // if the defending unit wins
        else
        {
            // destroy attacking unit
            attackingUnit.DestroySelf();
            GameObject gameManagerObject = GameObject.Find("GameManager");
            Game game = gameManagerObject.GetComponent<Game>();

            game.eliminatedUnits.Add(attackingUnit.unitName);
        }      
        
        // removed automatically end turn after attacking (Modified by Dom 13/02/18)
    }

    /// <summary>
    /// 
    /// Returns the selected unit if it is adjacent to this sector
    /// otherwise, returns null
    /// 
    /// </summary>
    public Unit AdjacentSelectedUnit() {

        // scan through each adjacent sector
        foreach (Sector adjacentSector in adjacentSectors)
        {
            // if the adjacent sector contains the selected unit,
            // return the selected unit
            if (adjacentSector.unit != null && adjacentSector.unit.IsSelected())
                return adjacentSector.unit;
        }

        // otherwise, return null
        return null;
    }

    /// <summary>
    /// 
    /// returns the outcome of a combat encounter between two units
    /// takes into consideration the units levels and the attack/defence bonus of the player
    /// 
    /// close match leads to uncertain outcome (i.e. could go either way)
    /// if one unit + bonuses is significantly more powerful than another then they are very likely to win
    /// 
    /// </summary>
    /// <param name="attackingUnit">Unit object of the attacking unit</param>
    /// <param name="defendingUnit">Unit object of the defending unit</param>
    /// <returns>'true' if attacking unit wins or 'false' if defending unit wins</returns>
    private bool Conflict(Unit attackingUnit, Unit defendingUnit) {

        // return 'true' if attacking unit wins;
        // return 'false' if defending unit wins
        
        int attackingUnitRoll;
        if (attackingUnit.GetOwner().GetResourcesNullified()) {
            attackingUnitRoll = Random.Range(1, (5 + attackingUnit.GetLevel()));
        } else {
            attackingUnitRoll = Random.Range(1, (5 + attackingUnit.GetLevel())) + attackingUnit.GetOwner().GetAttack() + attackingUnit.GetOwner().GetAttackBonus();
        }

        int defendingUnitRoll;
        if (defendingUnit.GetOwner().GetResourcesNullified()) {
            defendingUnitRoll = Random.Range(1, (5 + defendingUnit.GetLevel()));
        } else {
            defendingUnitRoll = Random.Range(1, (5 + defendingUnit.GetLevel())) + defendingUnit.GetOwner().GetDefence() + defendingUnit.GetOwner().GetDefenceBonus();
        }


        // display conflict resolution dialog (in place of the planned combat animation)

        // generate the title for the dialog
        string title;
        if (attackingUnitRoll > defendingUnitRoll)
        {
            title = "Victory!";
        }
        else
        {
            title = "Defeat!";
        }

        // generate the body for the dialog
        string body = "";

        // generate the attacker portion of the dialog body
        if (attackingUnit.GetOwner().GetResourcesNullified()) {
            body += (attackingUnit.unitName + " rolled a " + (attackingUnitRoll).ToString() + "\n");
            body += "- attack bonus nullified -\n\n";
        } else {
            body += (attackingUnit.unitName + " rolled a " + (attackingUnitRoll - (attackingUnit.GetOwner().GetAttack() + attackingUnit.GetOwner().GetAttackBonus())).ToString() + "\n");
            body += (" plus " + (attackingUnit.GetOwner().GetAttack() + attackingUnit.GetOwner().GetAttackBonus()).ToString() + " attack bonus \n\n");
        }

        // generate the defender portion of the dialog body
        if (defendingUnit.GetOwner().GetResourcesNullified()) {
            body += (defendingUnit.unitName + " rolled a " + (defendingUnitRoll).ToString() + "\n");
            body += "- defence bonus nullified -\n\n";
        } else {
            body += (defendingUnit.unitName + " rolled a " + (defendingUnitRoll - (defendingUnit.GetOwner().GetDefence() + defendingUnit.GetOwner().GetDefenceBonus())).ToString() + "\n");
            body += (" plus " + (defendingUnit.GetOwner().GetDefence() + defendingUnit.GetOwner().GetDefenceBonus()).ToString() + " defence bonus \n\n");
        }

        // Show the dialog
        map.game.dialog.SetDialogType(Dialog.DialogType.ShowText);
        map.game.dialog.SetDialogData(title, body);
        map.game.dialog.Show();
        try {
            GameObject info = GameObject.Find("Info");
            UnityEngine.UI.Text text = info.GetComponent<UnityEngine.UI.Text>();
            text.lineSpacing = 0.5f;
        } catch (System.NullReferenceException e) {
            Debug.Log("could not change line spacing in Conflict dialog");
        }

        return attackingUnitRoll > defendingUnitRoll;


        #region conflict resolution algorithm updated to make more fair (Modified by Dom 13/02/2018)

        /*

        // diff = +ve attacker advantage 
        // diff = -ve defender advantage
        int diff = (attackingUnit.GetLevel() + attackingUnit.GetOwner().GetAttack() + 1) - (defendingUnit.GetLevel() + defendingUnit.GetOwner().GetDefence());

        // determine uncertaincy in combat
        // small diff in troops small uncertaincy level
        float uncertaincy = -0.4f * (Mathf.Abs(diff)) + 0.5f;
        if (uncertaincy < 0.1f)
        {
            uncertaincy = 0.1f; // always at least 10% uncertaincy
        }

        if (Random.Range(0, 1) < uncertaincy)
        {
            if (diff < 0)
            {
                return false;
            } else
            {
                return true;
            }
        } else
        {
            if (diff < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        */

        #endregion
    }

    /// <summary>
    /// 
    /// Raises the mouse enter event.
    /// 
    /// </summary>
    void OnMouseEnter() {
        OnMouseEnterAccessible();
    }

    /// <summary>
    /// 
    /// When the mouse first hovers over the sector,
    /// if the game is in the 'Move' state and this sector 
    /// contains a unit owned by the current player, 
    /// highlight the sector
    /// 
    /// </summary>
    public void OnMouseEnterAccessible() {

        if(map.game.GetTurnState() == Game.TurnState.Move) {
            if(this.unit != null) {
                if(this.unit.GetOwner() == map.game.currentPlayer) {
                    ApplyHighlight(0.4f);
                }
            }
        } else if (map.game.GetTurnState() == Game.TurnState.SelectUnit) {

        }
    }

    /// <summary>
    /// 
    /// Raises the mouse exit event.
    /// 
    /// </summary>
    void OnMouseExit() {
        OnMouseExitAccessible();
    }

    /// <summary>
    /// 
    /// When the mouse first leaves the sector,
    /// if the game is in the 'Move' state and this sector 
    /// contains a unit owned by the current player, 
    /// unhighlight the sector
    /// 
    /// </summary>
    public void OnMouseExitAccessible() {

        //The mouse is no longer hovering over the GameObject so output this message each frame
        if (map.game.GetTurnState() == Game.TurnState.Move) {
            if (this.unit != null) {
                if (this.unit.GetOwner() == map.game.currentPlayer) {
                    RevertHighlight();
                }
            }
        } else if (map.game.GetTurnState() == Game.TurnState.SelectUnit) {

        }
    }

    /// <summary>
    /// 
    /// Assigns the sector's parameters based on the provided saved data.
    /// 
    /// </summary>
    /// <param name="savedData">Saved data.</param>
    public void OnLoad(GameData savedData) {

        // assign the sector's owner, if any
        if (savedData.sectorOwner[sectorID] == -1) {
            this.owner = null;
            this.ownerID = -1;
        } else {
            map.game.players[savedData.sectorOwner[sectorID]].Capture(this);
        }

        // instantiate and load the sector's unit, if any
        if (savedData.sectorLevel[sectorID] != -1)
        {
            this.unit = MonoBehaviour.Instantiate(map.game.players[0].GetUnitPrefab()).GetComponent<Unit>();
            this.unit.Initialize(this.owner, this);
            this.unit.OnLoad(savedData,sectorID);
        }

        // instantiate the sector's punishment card, if any
        if (savedData.sectorPunishmentCard[sectorID] == true)
        {
            PunishmentCard card = MonoBehaviour.Instantiate(map.game.GetPunishmentCardPrefab()).GetComponent<PunishmentCard>();
            card.Initialize(this, savedData.sectorPunishmentCardEffect[sectorID]);
            this.punishmentCard = card;
        }

        // set this sector to contain the VC, if it should
        this.VC = savedData.VCSector == sectorID;
    }
    
}