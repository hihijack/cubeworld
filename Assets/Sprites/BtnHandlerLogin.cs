using UnityEngine;
using System.Collections;

public class BtnHandlerLogin : MonoBehaviour {

	// Use this for initialization
	GameViewLogin gameView;
	void Start () {
		gameView = GameObject.Find("CPU").GetComponent<GameViewLogin>();
	}
	
	void OnClick(){
		gameView.OnBtnClick(gameObject);
	}
}
