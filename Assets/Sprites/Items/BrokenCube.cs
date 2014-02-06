using UnityEngine;
using System.Collections;

public class BrokenCube : ITrigger {

	public override void OnTrigger (GameObject gobjTarget)
	{
		StartCoroutine(CoTime(0.2f));
	}
	
	IEnumerator CoTime(float delaytiem){
		yield return new WaitForSeconds(delaytiem);
		DestroyObject(transform.parent.gameObject);
	}
}
