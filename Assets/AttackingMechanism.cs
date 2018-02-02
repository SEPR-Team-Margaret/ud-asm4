using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingMechanism : MonoBehaviour {

    GameObject tracking;
    Vector3 targetPos;

    [SerializeField] float error;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (tracking == null)
            return;
        Vector2 rPos = Random.insideUnitCircle;
        targetPos = new Vector3(rPos.x * error + tracking.transform.position.x, rPos.y * error + tracking.transform.position.z, 0);
	}

    public void SetTarget(GameObject obj)
    {
        tracking = obj;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(transform.position, targetPos - transform.position);
    }
}
