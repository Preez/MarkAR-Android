using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Some code was provided by Dr. Hoff and factored and implemented by Jose Perez

public class arrowCompasHandler : MonoBehaviour {
    // Start is called before the first frame update

    private GameObject mainCamera, target, insideArrow, MasterObject, arrow_compass;

    private float speed = 0.01f, offset = 90;

    private Vector3 target_position, positionProjected, target_position_normalized;

    void Start () {
        mainCamera = Camera.main.gameObject;
        target = GameObject.Find ("MarkerX1Comp");
        arrow_compass = GameObject.Find ("compass");
    }

    // Update is called once per frame
    void Update () {

        target_position = mainCamera.transform.InverseTransformPoint (target.transform.position);

        positionProjected = new Vector3 (target_position.x, target_position.y, 0); //Ignore Z

        target_position_normalized = positionProjected.normalized;

        float target_angle = Mathf.Rad2Deg * Mathf.Acos (target_position_normalized.x);

        if (target_position_normalized.y < 0) {
            target_angle = -target_angle;
        }

        arrow_compass.transform.localRotation = Quaternion.Lerp (arrow_compass.transform.rotation, Quaternion.Euler (0, 0, target_angle - offset), Time.time * speed);

    }


}