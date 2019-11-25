using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {
    public GameObject player;
    float speed = 0;
    int num = 0;

    private void Start() {
        speed = Random.Range(5, 15);
        num = Random.Range(0, 3);
        if (num == 1) {
            num = 0;
        }
        else {
            num = 1;
        }
    }

    private void Update() {
        if (num == 0) {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position + player.transform.up * 10, speed * Time.deltaTime);
        }
        else {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }
}