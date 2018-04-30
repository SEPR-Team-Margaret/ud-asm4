using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class UnitSprite {
    
    [System.NonSerialized] private GameObject unitGO;

    [System.NonSerialized] private GameObject headGO;
    [System.NonSerialized] private GameObject bodyGO;
    [System.NonSerialized] private GameObject hatGO;

    [SerializeField] public int currentHead;
    [SerializeField] public int currentBody;
    [SerializeField] public int currentHat;

    /// <summary>
    /// 
    /// Initializes a new instance of the <see cref="UnitSprite"/> class.
    /// 
    /// </summary>
    /// <param name="unit">Unit.</param>
    /// <param name="head">Head index.</param>
    /// <param name="body">Body index.</param>
    /// <param name="hat">Hat index.</param>
    public UnitSprite(GameObject unit, int head = 0, int body = 0, int hat = 0) {

        unitGO = unit;

        headGO = unitGO.transform.Find("Head").gameObject;
        bodyGO = unitGO.transform.Find("Body").gameObject;
        hatGO = unitGO.transform.Find("Hat").gameObject;

        SetHead(head);
        SetBody(body);
        SetHat(hat);
    }

    /// <summary>
    /// 
    /// Randomly assigns the UnitSprite's body, head, and hat components.
    /// 
    /// </summary>
    public void GenerateRandomSprite()
    {
        currentHead = Random.Range(1, 11);
        currentBody = Random.Range(1, 11);
        currentHat = Random.Range(1, 11);

        SetHead(currentHead);
        SetBody(currentBody);
        SetHat(currentHat);
    }

    /// <summary>
    /// 
    /// Sets the UnitSprite's head component.
    /// 
    /// </summary>
    /// <param name="headID">Head index.</param>
    public void SetHead(int headID) {
        
        currentHead = headID;

        MeshRenderer renderer = this.headGO.GetComponent<MeshRenderer>();
        string resourceLC = "UnitSprites/Head/head" + headID.ToString();
        renderer.material = (Material)Resources.Load(resourceLC);

    }

    /// <summary>
    /// 
    /// Sets the UnitSprite's body component.
    /// 
    /// </summary>
    /// <param name="bodyID">Body index.</param>
    public void SetBody(int bodyID) {
        currentBody = bodyID;

        MeshRenderer renderer = this.bodyGO.GetComponent<MeshRenderer>();
        string resourceLC = "UnitSprites/Body/body" + bodyID.ToString();
        renderer.material = (Material)Resources.Load(resourceLC);
    }

    /// <summary>
    /// 
    /// Sets the UnitSprite's hat.
    /// 
    /// </summary>
    /// <param name="hatID">Hat index.</param>
    public void SetHat(int hatID) {
        currentHat = hatID;

        MeshRenderer renderer = this.hatGO.GetComponent<MeshRenderer>();
        string resourceLC = "UnitSprites/Hat/hat" + hatID.ToString();
        renderer.material = (Material)Resources.Load(resourceLC);
    }
}

