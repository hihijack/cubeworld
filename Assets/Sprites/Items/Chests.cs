using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Chests : IInteractive {
	public override void OnInteract (GameObject gobjTarget)
	{
		if(GameManager.PlayModel == EPlayModel.Mine){
			StartCoroutine(CoRequestVerify());
		}
	}
	
	IEnumerator CoRequestVerify(){
		WWW myWWW = new WWW("http://localhost/cwserver/mapverify.php?playerid=" + GameManager.CurPlayerId + "&verify=1");
		
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
