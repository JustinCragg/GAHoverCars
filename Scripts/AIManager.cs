﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour {
    public GameObject carPrefab = null;
    public int numCars = 1;
    public float genLength = 10;

    public GameObject carsObject = null;

    List<HoverCarAI> aiCars = new List<HoverCarAI>();
    List<TrackGoal> trackGoals = new List<TrackGoal>();


    float startTime = 0;

    int generation = 1;
    float mutationRate = 1;
    public UIManager uiManager;

    int sortByIndex(TrackGoal a, TrackGoal b) {
        return a.trackIndex.CompareTo(b.trackIndex);
    }

	void Start() {
        // Assign goals
        foreach (TrackGoal goal in GetComponentsInChildren<TrackGoal>()) {
            trackGoals.Add(goal);
            goal.setActive(false);
        }
        trackGoals.Sort(sortByIndex);

        // Assign any existing hover cars
        foreach (HoverCarAI ai in GetComponentsInChildren<HoverCarAI>()){
            aiCars.Add(ai);
            ai.setManager(this);
        }
        // Create any remaining cars
        for (int i = aiCars.Count; i < numCars; i++) {
            aiCars.Add(Instantiate(carPrefab, carsObject.transform).GetComponent<HoverCarAI>());
            aiCars[aiCars.Count - 1].setManager(this);
        }
        uiManager.setGenText(generation);
        uiManager.setTimeText(genLength);

        Camera.main.transform.parent = aiCars[0].transform;
    }

    void fullReset() {
        generation = 0;
        startNewGen(true);
        uiManager.setTimeText(genLength);
    }

    void Update() {
        if (Input.GetKeyUp(KeyCode.Alpha1)) {
            genLength = 10.0f;
            fullReset();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2)) {
            genLength = 30.0f;
            fullReset();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3)) {
            genLength = 60.0f;
            fullReset();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4)) {
            genLength = Mathf.Infinity;
            fullReset();
        }

        if (Time.time - startTime > genLength) {
            startNewGen();
        }
    }

    public void startNewGen(bool fullReset = false) {
        Debug.Log(Time.time - startTime);
        resetCars(fullReset);
        resetGoals();
        generation++;
        mutationRate = Mathf.Max(1 - generation * 0.03f, 0.05f);
        uiManager.setGenText(generation);
        startTime = Time.time;
    }

    void resetCars(bool fullReset) {
        if (fullReset == true) {
            foreach (HoverCarAI car in aiCars) {
                car.setAIVars();
                car.reset();
            }
        }
        else {
            List<HoverCarAI.Score> scores = new List<HoverCarAI.Score>();
            foreach (HoverCarAI car in aiCars) {
                scores.Add(car.getScore());
            }
            scores.Sort(HoverCarAI.Score.sortFunction);

            // Copy best previous car
            aiCars[0].setAIVars(scores[0].car.getAIVars());
            aiCars[0].reset();
            aiCars[0].GetComponent<Renderer>().material.color = Color.blue;

            // Create mutation based on best car
            for (int i = 1; i < aiCars.Count * 0.95f; i++) {
                HoverCarAI.AIVars aiVars = aiCars[0].getAIVars();
                aiVars.mutate(mutationRate);
                aiCars[i].setAIVars(aiVars);
                aiCars[i].reset();
            }
            // Fresh new random mutation
            for (int i = Mathf.RoundToInt(aiCars.Count * 0.95f); i < aiCars.Count; i++) {
                aiCars[i].setAIVars();
                aiCars[i].reset();
                aiCars[i].GetComponent<Renderer>().material.color = Color.green;
            }
        }
    }

    void resetGoals() {
        foreach (TrackGoal goal in trackGoals) {
            goal.setActive(false);
        }
        trackGoals[0].setActive(true);
    }

    public TrackGoal getGoal(int index) {
        if (index + 1 > trackGoals.Count) {
            index = 0;
        }
        return trackGoals[index];
    }
}
