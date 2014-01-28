using UnityEngine;
using System.Collections;

public class PlatformUpDown : ITrigger {

	public bool isTriggerBox;
	
	// Use this for initialization
	void Start () {
		if(GameManager.GameModel == EGameModel.Play && !isTriggerBox){
			iTween.MoveAdd(gameObject, iTween.Hash("y", 5f, "time", 5f, "delay", 1f, "looptype", iTween.LoopType.pingPong, "easetype", "easeInOutQuad"));
		}
	}
	
	public override void OnTrigger (GameObject gobjTarget)
	{
		gobjTarget.transform.parent = transform;
	}
	
	public override void OnTriggerExit (GameObject gobjTarget)
	{
		gobjTarget.transform.parent = null;
	}
}
