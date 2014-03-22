using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Secret : IInteractive {
	public string worlds;
	GameView gameview;
	
	void Start(){
		gameview = GameObject.FindGameObjectWithTag("CPU").GetComponent<GameView>();
	}
	
	public override void OnInteract (GameObject gobjTarget)
	{
		Animation anim = Tools.GetComponentInChildByPath<Animation>(gameObject, "model");
		anim.Play("Open");
		
		gameview.ShowSecret(worlds);
	}
}
