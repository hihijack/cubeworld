using UnityEngine;
using System.Collections;

public class BtnHandlerMain : MonoBehaviour {

	// Use this for initialization
	Main gameView;
	void Start () {
		gameView = GameObject.Find("CPU").GetComponent<Main>();
	}
	
	void OnPress(bool isDown){
		gameView.OnBtnPress(gameObject, isDown);
	}
}
