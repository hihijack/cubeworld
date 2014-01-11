using UnityEngine;
using System.Collections;

public class BtnHandler : MonoBehaviour {

	// Use this for initialization
	GameView gameView;
	void Start () {
		gameView = GameObject.Find("CPU").GetComponent<GameView>();
	}
	
	void OnPress(bool isDown){
		gameView.OnBtnPress(gameObject.name, isDown);
	}
}
