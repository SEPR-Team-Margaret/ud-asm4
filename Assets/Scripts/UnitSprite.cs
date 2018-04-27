using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class UnitSprite {
    [System.NonSerialized] private GameObject unitGO;

    [System.NonSerialized] private GameObject headGO;
    [SerializeField] public int currentHead;

    [System.NonSerialized] private GameObject bodyGO;
    [SerializeField] public int currentBody;

    [System.NonSerialized] private GameObject hatGO;
    [SerializeField] public int currentHat;

    public UnitSprite(GameObject unit, int head = 0, int body = 0, int hat = 0) {
        unitGO = unit;

        headGO = unitGO.transform.Find("Head").gameObject;
        bodyGO = unitGO.transform.Find("Body").gameObject;
        hatGO = unitGO.transform.Find("Hat").gameObject;

        SetHead(head);
        SetBody(body);
        SetHat(hat);
    }

    public void GenerateRandomSprite()
    {
        currentHead = Random.Range(1, 11);
        currentBody = Random.Range(1, 11);
        currentHat = Random.Range(1, 11);

        SetHead(currentHead);
        SetBody(currentBody);
        SetHat(currentHat);
    }

    public void SetHead(int headID) {
        currentHead = headID;

        MeshRenderer renderer = this.headGO.GetComponent<MeshRenderer>();
        string resourceLC = "UnitSprites/Head/head" + headID.ToString();
        renderer.material = (Material)Resources.Load(resourceLC);

        //This doesn't seem to work, the path is returning a null material for some reason...
    }

    public void SetBody(int bodyID) {
        currentBody = bodyID;

        MeshRenderer renderer = this.bodyGO.GetComponent<MeshRenderer>();
        string resourceLC = "UnitSprites/Body/body" + bodyID.ToString();
        renderer.material = (Material)Resources.Load(resourceLC);
    }

    public void SetHat(int hatID) {
        currentHat = hatID;

        MeshRenderer renderer = this.hatGO.GetComponent<MeshRenderer>();
        string resourceLC = "UnitSprites/Hat/hat" + hatID.ToString();
        renderer.material = (Material)Resources.Load(resourceLC);
    }
}

