using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UiManager : MonoBehaviour {
	public Text Scores;
	public Slider slider;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateScores(int scores){
		Scores.text = "Points : " + scores.ToString ();
	}

	public void reduceSlider(float value){
		slider.value = value;
	}

	public void replay() {
		Application.LoadLevel(Application.loadedLevel);

    }
}
