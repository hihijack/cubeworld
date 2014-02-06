using UnityEngine;
using System.Collections;

public class Dangerous : ITrigger {
	
	public override void OnTrigger (GameObject gobjTarget)
	{
		Hero hero = gobjTarget.GetComponent<Hero>();
		hero.updataState(new IActorAction(EFSMAction.HERO_DIE));
	}
}
