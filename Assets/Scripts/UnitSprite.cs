using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UnitSprite {
    private GameObject unitGO;

    private GameObject headGO;
    [SerializeField] private int currentHead;

    private GameObject bodyGO;
    [SerializeField] private int currentBody;

    private GameObject hatGO;
    [SerializeField] private int currentHat;

    public UnitSprite(GameObject unit, int head = 0, int body = 0, int hat = 0) {
        unitGO = unit;

        headGO = unitGO.transform.Find("Head").gameObject;
        bodyGO = unitGO.transform.Find("Body").gameObject;
        hatGO = unitGO.transform.Find("Hat").gameObject;

        SetHead(head);
        SetBody(body);
        SetHat(hat);
    }

    public void SetHead(int headID) {
        MeshRenderer renderer = this.headGO.GetComponent<MeshRenderer>();
        string resourceLC = "UnitSprites/Head/head" + headID.ToString();
        renderer.material = (Material)Resources.Load(resourceLC);

        //This doesn't seem to work, the path is returning a null material for some reason...
    }

    public void SetBody(int bodyID) {
        MeshRenderer renderer = this.bodyGO.GetComponent<MeshRenderer>();
        string resourceLC = "UnitSprites/Body/body" + bodyID.ToString();
        renderer.material = (Material)Resources.Load(resourceLC);
    }

    public void SetHat(int hatID) {
        MeshRenderer renderer = this.hatGO.GetComponent<MeshRenderer>();
        string resourceLC = "UnitSprites/Hat/hat" + hatID.ToString();
        renderer.material = (Material)Resources.Load(resourceLC);
    }
}

