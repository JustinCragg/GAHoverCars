using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverEngine : MonoBehaviour {
    Rigidbody rgbd = null;

    public bool player = false;

    // Hovering
    float hoverHeight = 5.0f;

    // Steering
    public float forwardAccel = 1.0f;
    public float turnAccel = 1.0f;
    float accelInput;
    float turnInput;

    // Self-Righting
    public float rightStrength = 5.0f;

    void Start() {
        rgbd = GetComponent<Rigidbody>();
    }

    void Update() {
        if (player == true) {
            accelInput = Input.GetAxis("Vertical");
            turnInput = Input.GetAxis("Horizontal");
            if (Input.GetKey(KeyCode.Space)) {
                transform.position = new Vector3(0, 2.5f, 0);
                transform.rotation = Quaternion.Euler(90, 0, 0);
                Rigidbody rgbd = GetComponent<Rigidbody>();
                rgbd.velocity = new Vector3(0, 0, 0);
                rgbd.angularVelocity = new Vector3(0, 0, 0);
            }
        }
        selfRight();
    }

    public void setInput(float accel, float turn) {
        accelInput = accel;
        turnInput = turn;
    }

    void FixedUpdate() {
        hover();
        steering();
    }

    void hover() {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        int mask = ~LayerMask.GetMask("Cars");
        if (Physics.Raycast(ray, out hit, hoverHeight, mask)) {
            float force = (hoverHeight - hit.distance) * -Physics.gravity.y;
            rgbd.AddRelativeForce(-Vector3.forward * force);
        }
    }

    void steering() {
        rgbd.AddForce(transform.up * accelInput * forwardAccel, ForceMode.Acceleration);
        rgbd.AddTorque(-transform.forward * turnInput * turnAccel, ForceMode.Acceleration);
    }

    void selfRight() {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        Quaternion targetRot = transform.rotation;

        int mask = ~LayerMask.GetMask("Cars");
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask)) {
            targetRot = Quaternion.FromToRotation(-transform.forward, hit.normal) * transform.rotation;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rightStrength);
    }
}