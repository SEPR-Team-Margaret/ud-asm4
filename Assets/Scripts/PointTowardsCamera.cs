using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTowardsCamera : MonoBehaviour {

    private Camera cam;
    private Transform camTransform;
    private float speed = 1f;

    private void Start() {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        camTransform = cam.GetComponent<Transform>();

    }
    void Update() {
        Vector3 targetDir = camTransform.position - transform.position;
        float step = speed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
        Debug.DrawRay(transform.position, newDir, Color.red);
        transform.rotation = Quaternion.LookRotation(newDir);
        //transform.rotation = new Quaternion(transform.rotation.x, 0 , transform.rotation.z, transform.rotation.w);
    }

}
