using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmark : MonoBehaviour {

    public enum ResourceType {Attack, Defence};
	[SerializeField] private ResourceType resourceType;
    [SerializeField] private int amount = 2;
    public GameObject landmarkBuilding;

    /// <summary>
    /// 
    /// Returns the type of bonus this land mark gives
    /// 
    /// </summary>
    /// <returns>Type of bonus this landmark gives</returns>
    public ResourceType GetResourceType() {
        return resourceType;
    }

    /// <summary>
    /// 
    /// Sets the type of resource this landmark should give
    /// 
    /// </summary>
    /// <param name="resourceType"></param>
    public void SetResourceType(ResourceType resourceType) {
        this.resourceType = resourceType;
    }

    /// <summary>
    /// 
    /// Gets the amount of bonus this landmark gives
    /// 
    /// </summary>
    /// <returns>Amount of bonus given by this landmark</returns>
    public int GetAmount() {
        return amount;
    }

    /// <summary>
    /// 
    /// Sets the amount of bonus this landmark gives
    /// 
    /// </summary>
    /// <param name="amount"></param>
    public void SetAmount(int amount) {
        this.amount = amount;
    }

}
