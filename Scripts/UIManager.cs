using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
    public Text genText;
    public Text timeText;

    public void setGenText(int gen) {
        genText.text = "Gen: " + gen.ToString();
    }

    public void setTimeText(float time) {
        if (time == Mathf.Infinity) {
            timeText.text = "Time: One Lap";
        }
        else {
            timeText.text = "Time: " + time.ToString();
        }
    }

    void Update() {
        if (Input.GetKeyUp(KeyCode.Return)) {
            int index = SceneManager.GetActiveScene().buildIndex;
            index++;
            if (index > 2) {
                index = 0;
            }
            SceneManager.LoadScene(index);
        }
    }
}
