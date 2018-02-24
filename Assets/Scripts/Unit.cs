﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    [SerializeField] private Player owner;
    [SerializeField] private Sector sector;
    [SerializeField] private int level;
    [SerializeField] private Color color;
    [SerializeField] private bool selected = false;
    [SerializeField] private string unitName;

    [SerializeField] private Material level1Material;
	[SerializeField] private Material level2Material;
	[SerializeField] private Material level3Material;
	[SerializeField] private Material level4Material;
	[SerializeField] private Material level5Material;
    
    /// <summary>
    /// 
    /// Initializes the unit on the passed sector and assigns it to the passed player
    /// The unit is set to level 1
    /// The unit's colour is set to the colour of the player that owns it
    /// 
    /// </summary>
    /// <param name="player">The player the unit belongs to</param>
    /// <param name="sector">The sector the unit is on</param>
    public void Initialize(Player player, Sector sector)
    {

        // set the owner, level, and color of the unit
        owner = player;
        level = 1;
        color = owner.GetColor();

        unitName = GenerateName();

        // set the material color to the player color
        GetComponent<Renderer>().material.color = color;

        // place the unit in the sector
        MoveTo(sector);

    }

    private string GenerateName() {
        return null;

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
    public Color GetColor() {
        return color;
    }

    /// <summary>
    /// 
    /// Sets this unit's colour
    /// 
    /// </summary>
    /// <param name="color">This units colour</param>
    public void SetColor(Color color) {
        this.color = color;
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
        
        // clear the unit's current sector
        if (this.sector != null)
        {
            this.sector.ClearUnit();
        }   

        // set the unit's sector to the target sector
        // and the target sector's unit to the unit
        this.sector = targetSector;
        targetSector.SetUnit(this);
		Transform targetTransform = targetSector.transform.Find ("Units").transform;

        // set the unit's transform to be a child of
        // the target sector's transform
        transform.SetParent(targetTransform);

        // align the transform to the sector
        transform.position = targetTransform.position;


        // if the target sector belonged to a different 
        // player than the unit, capture it and level up
        if (targetSector.GetOwner() != this.owner)
        {
            // level up
            LevelUp();

            // capture the target sector for the owner of this unit
            owner.Capture(targetSector);
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
        for (int i = 0; i < this.owner.GetGame().gameMap.GetComponent<Map>().sectors.Length; i++)
        {
            if (this.sector == this.owner.GetGame().gameMap.GetComponent<Map>().sectors[i])
                tempSectorIndex = i;
        }

        // swap the units' references to their sectors
        this.sector = otherUnit.sector;
        otherUnit.sector = this.owner.GetGame().gameMap.GetComponent<Map>().sectors[tempSectorIndex] ;
                
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
            UpdateUnitMaterial();
		}
		
	}

    public void UpdateUnitMaterial()
    {
        switch (level)
        {
            case 2:
                this.gameObject.GetComponent<MeshRenderer>().material = level2Material;
                break;
            case 3:
                this.gameObject.GetComponent<MeshRenderer>().material = level3Material;
                break;
            case 4:
                this.gameObject.GetComponent<MeshRenderer>().material = level4Material;
                break;
            case 5:
                this.gameObject.GetComponent<MeshRenderer>().material = level5Material;
                break;
            default:
                this.gameObject.GetComponent<MeshRenderer>().material = level1Material;
                break;
        }

        // set material color to match owner color
        GetComponent<Renderer>().material.color = color;
    }

    /// <summary>
    /// 
    /// select the unit and highlight the sectors adjacent to it
    /// 
    /// </summary>
    public void Select() {
        
        selected = true;
        sector.ApplyHighlightAdjacent();
    }

    /// <summary>
    /// 
    /// deselect the unit and unhighlight the sectors adjacent to it
    /// 
    /// </summary>
    public void Deselect() {
        
        selected = false;
        sector.RevertHighlightAdjacent();
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
        
}
