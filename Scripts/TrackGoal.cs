using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGoal : MonoBehaviour {
    public int trackIndex = -1;

    public void setActive(bool active) {
        if (active == true) {
            GetComponent<Renderer>().material.color = new Color(1, 1, 0, 0.25f);
        }
        else {
            GetComponent<Renderer>().material.color = new Color(0, 0, 0, 0);
        }
    }
}
