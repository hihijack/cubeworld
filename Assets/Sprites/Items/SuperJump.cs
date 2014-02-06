using UnityEngine;
using System.Collections;

public class SuperJump : ITrigger {

	public override void OnTrigger (GameObject gobjTarget)
	{
		Hero hero = gobjTarget.GetComponent<Hero>();
		hero.SuperJump = true;
	}
	
	public override void OnExitTrigger (GameObject gobjTarget)
	{
		Hero hero = gobjTarget.GetComponent<Hero>();
		hero.SuperJump = false;
	}
}
