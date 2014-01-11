using System.Collections;
using UnityEngine;

public class IActorAction{
	public EFSMAction actiontype;
	public IActorAction(){}
	public IActorAction(EFSMAction actiontype){
		this.actiontype = actiontype;
	}
}

//public class ActionHeroFlash : IActorAction{
//	public float x;
//	public float y;
//	public float xStart;
//	public float yStart;
//	public ActionHeroFlash(float x, float y, float xStart, float yStart){
//		this.actiontype = EFSMAction.HERO_FLASH;
//		this.x = x;
//		this.y = y;
//		this.xStart = xStart;
//		this.yStart = yStart;
//	}
//}
