using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverCarAI : MonoBehaviour {
    TrackGoal currentGoal = null;
    AIManager manager;

    HoverEngine engine;

    public float accel;
    public float angle;

    int mask;

    [Header("Public AI Vars")]
    //[Range(0.1f,5.0f)]
    // Affects how strongly it reacts when needing to turn
    public float aimStrength = 1;

    //[Range(10,50)]
    // Affects the distance in front of the car, before it reacts
    public float forwardDistance = 10;
    //[Range(0.1f,1)]
    // Affects how strongly the car moves forward
    public float forwardStrength = 1;
    //[Range(0.1f,1)]
    // Affects how strongly the car moves backward
    public float backwardStrength = 1;

    //[Range(5,20)]
    // Affects the distance to the sides of the car, before it reacts
    public float turnDistance = 10;
    //[Range(0.1f,1)]
    // Affects how strongly the car moves away from walls at its sides
    public float turnStrength = 1;

    public struct Score {
        public static int sortFunction(Score a, Score b) {
            // Checkpoints > : Distance <
            if (b.points.CompareTo(a.points) == 0) {
                return a.distance.CompareTo(b.distance);
            }
            else {
                return b.points.CompareTo(a.points);
            }
        }

        public HoverCarAI car;
        public int points;
        public float distance;
    }

    public struct AIVars {
        // Randomly mutates given values
        public void mutate(float rate) {
            float min;
            float max;
            float offset;

            offset = aimStrength * rate;
            min = Mathf.Clamp(aimStrength - offset, 0.0f, 10.0f);
            max = Mathf.Clamp(aimStrength + offset, 0.0f, 10.0f);
            aimStrength = Random.Range(min, max);

            offset = forwardDistance * rate;
            min = Mathf.Clamp(forwardDistance - offset, 0.0f, 50.0f);
            max = Mathf.Clamp(forwardDistance + offset, 0.0f, 50.0f);
            forwardDistance = Random.Range(min, max);

            offset = forwardStrength * rate;
            min = Mathf.Clamp(forwardStrength - offset, 0.0f, 10.0f);
            max = Mathf.Clamp(forwardStrength + offset, 0.0f, 10.0f);
            forwardStrength = Random.Range(min, max);

            offset = backwardStrength * rate;
            min = Mathf.Clamp(backwardStrength - offset, 0.0f, 10.0f);
            max = Mathf.Clamp(backwardStrength + offset, 0.0f, 10.0f);
            backwardStrength = Random.Range(min, max);

            offset = turnDistance * rate;
            min = Mathf.Clamp(turnDistance - offset, 0.0f, 50.0f);
            max = Mathf.Clamp(turnDistance + offset, 0.0f, 50.0f);
            turnDistance = Random.Range(min, max);

            offset = turnStrength * rate;
            min = Mathf.Clamp(turnStrength - offset, 0.0f, 10.0f);
            max = Mathf.Clamp(turnStrength + offset, 0.0f, 10.0f);
            turnStrength = Random.Range(min, max);
        }

        public float aimStrength;
        public float forwardDistance;
        public float forwardStrength;
        public float backwardStrength;
        public float turnDistance;
        public float turnStrength;
    }

    int checkPoints = 0;

    void Start() {
        engine = GetComponent<HoverEngine>();
        setAIVars();
        mask = ~LayerMask.GetMask("Cars");
    }

	void Update() {
        moveToGoal();
    }

    public void setAIVars() {
        aimStrength = Random.Range(0.0f, 10.0f);

        forwardDistance = Random.Range(0.0f, 50.0f);
        forwardStrength = Random.Range(0.0f, 10.0f);
        backwardStrength = Random.Range(0.0f, 10.0f);

        turnDistance = Random.Range(0, 50);
        turnStrength = Random.Range(0.0f, 10.0f);
    }
    public void setAIVars(AIVars vars) {
        aimStrength = vars.aimStrength;

        forwardDistance = vars.forwardDistance;
        forwardStrength = vars.forwardStrength;
        backwardStrength = vars.backwardStrength;

        turnDistance = vars.turnDistance;
        turnStrength = vars.turnStrength;
    }

    void moveToGoal() {
        accel = forwardStrength;
        angle = 0;

        // Aiming at goal
        Vector3 goal = currentGoal.transform.position;
        goal.y = transform.position.y;
        Vector2 tempForwad = new Vector2(transform.up.x, transform.up.z);
        Vector2 tempDirec = new Vector2(goal.x - transform.position.x, goal.z - transform.position.z);

        // Negative : Turn right
        // Positive : Turn Left
        angle = -Vector2.SignedAngle(tempForwad, tempDirec);
        angle /= 180.0f;

        angle *= aimStrength;

        // Forward checking
        Ray forward = new Ray(transform.position, transform.up);
        RaycastHit hit;
        if (Physics.Raycast(forward, out hit, forwardDistance, mask)) {
            accel = -1 + hit.distance / forwardDistance;
            accel *= backwardStrength;
            angle *= turnStrength;
        }

        // Right Checking
        Ray right = new Ray(transform.position, transform.right);
        if (Physics.Raycast(right, out hit, turnDistance, mask)) {
            angle -= 0.1f * turnStrength;
        }

        // Left Checking
        Ray left = new Ray(transform.position, -transform.right);
        if (Physics.Raycast(left, out hit, turnDistance, mask)) {
            angle += 0.1f * turnStrength;
        }

        // Set inputs
        accel = Mathf.Clamp(accel, -1.0f, 1.0f);
        angle = Mathf.Clamp(angle, -1.0f, 1.0f);
        engine.setInput(accel, angle);

        // Forward
        //Debug.DrawRay(forward.origin, forward.direction * forwardDistance, Color.red);
        // Right
        //Debug.DrawRay(right.origin, right.direction * turnDistance, Color.blue);
        // Left
        //Debug.DrawRay(left.origin, left.direction * turnDistance, Color.green);

        // Goal
        //Debug.DrawLine(transform.position, goal, Color.magenta);
    }

    public void setManager(AIManager _manager) {
        manager = _manager;
        currentGoal = manager.getGoal(0);
        currentGoal.setActive(true);
    }

    public Score getScore() {
        Score output = new Score();
        output.car = this;
        output.points = checkPoints;
        output.distance = Vector3.Distance(transform.position, currentGoal.transform.position);

        return output;
    }

    public AIVars getAIVars() {
        AIVars output = new AIVars();
        output.aimStrength = aimStrength;
        output.forwardDistance = forwardDistance;
        output.forwardStrength = forwardStrength;
        output.backwardStrength = backwardStrength;
        output.turnDistance = turnDistance;
        output.turnStrength = turnStrength;

        return output;
    }

    public void reset() {
        transform.SetPositionAndRotation(new Vector3(0, 2.5f, 0), Quaternion.Euler(new Vector3(90, 0, 0)));
        accel = 0;
        angle = 0;
        Rigidbody rgbd = GetComponent<Rigidbody>();
        rgbd.velocity = Vector3.zero;
        rgbd.angularVelocity = Vector3.zero;
        currentGoal = manager.getGoal(0);
        checkPoints = 0;
        GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.25f);
    }

    void OnTriggerEnter(Collider other){
        TrackGoal goal = other.GetComponent<TrackGoal>();
        if (currentGoal == goal) {
            currentGoal.setActive(false);
            currentGoal = manager.getGoal(currentGoal.trackIndex + 1);
            checkPoints++;
            currentGoal.setActive(true);

            if (manager.genLength == Mathf.Infinity && currentGoal.trackIndex == 0) {
                manager.startNewGen();
            }
        }
    }
}
