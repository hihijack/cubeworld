using UnityEngine;
using System.Collections;

public class StoneTrap : ITrigger {
	GameObject gobjStone;
	public override void OnTrigger (GameObject gobjTarget)
	{
		gobjStone = Tools.LoadResourcesGameObject(IPath.BuildItems + "stone");
		gobjStone.transform.parent = transform;
		gobjStone.transform.localPosition = new Vector3(0f, 5f, 0f);
		iTween.MoveTo(gobjStone, iTween.Hash("y", 0f, "time", 1f, "islocal", true, "easetype", "easeInOutQuad", "oncomplete", "OnAnimEnd", "oncompletetarget", gameObject));
	}
	
	public override void OnTriggerExit (GameObject gobjTarget)
	{
		
	}
	
	void OnAnimEnd(){
		gobjStone.transform.parent = null;
		DestroyObject(gameObject);
	}
}
