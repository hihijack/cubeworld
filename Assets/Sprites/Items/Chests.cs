using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Chests : IInteractive {
	public override void OnInteract (GameObject gobjTarget)
	{
		Animation anim = Tools.GetComponentInChildByPath<Animation>(gameObject, "model");
		anim.Play("Open");
		if(GameManager.PlayModel == EPlayModel.Mine){
			StartCoroutine(CoRequestVerify());
		}else{
			GameView gameView = GameObject.FindWithTag("CPU").GetComponent<GameView>();
			gameView.PassOthers();
		}
	}
	
	IEnumerator CoRequestVerify(){
		WWW myWWW = new WWW("http://" + GameManager.ServerIP + "/cwserver/mapverify.php?playerid=" + GameManager.CurPlayerId + "&verify=1");
		
		yield return myWWW;
		
		string strRes = myWWW.text;
		
		GameView gameView = GameObject.FindWithTag("CPU").GetComponent<GameView>();
		
		JSONNode jdRes = JSONNode.Parse(strRes);
		int state = jdRes["state"].AsInt;
		if(state == 0){
			// 成功
			gameView.GeneralShowTip("验证成功");
			GameManager.IsVerify = true;
		}else{
			// 失败
			gameView.GeneralShowTip("验证失败");
		}
	}
}
