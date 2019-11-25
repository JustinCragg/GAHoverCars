using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public Text genText;

    public void setGenText(int gen) {
        genText.text = "Gen: " + gen.ToString();
    }
}
