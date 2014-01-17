using UnityEngine;
using System.Collections;
using SimpleJSON;

public class GameViewLogin : MonoBehaviour {
	
	GameObject gGobjUI;
	
	public GameObject gUIParent;
	
	void Start () {
		GetLocalSavedPlayerId();
		if(GameManager.CurPlayerId >= 0){
			// 本地存有帐号，显示登录
			ShowUILogin();
		}else{
			// 无帐号，显示注册
			ShowUIRegister();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void GetLocalSavedPlayerId(){
		int playerid = -1;
		if(PlayerPrefs.HasKey("playerid")){
			playerid = PlayerPrefs.GetInt("playerid");
		}
		
		string playername = "";
		if(PlayerPrefs.HasKey("playername")){
			playername = PlayerPrefs.GetString("playername");
		}
		
		GameManager.CurPlayerId = playerid;
		GameManager.CurPlayerName = playername;
	}
	
	void ShowUILogin(){
		GeneralShowUI(IPath.UI + "ui_login");
		UILabel txtName = Tools.GetComponentInChildByPath<UILabel>(gGobjUI, "name");
		txtName.text = GameManager.CurPlayerName;
	}
	
	void ShowUIRegister(){
		GeneralShowUI(IPath.UI + "ui_register");
		UIButton btnComfrim = Tools.GetComponentInChildByPath<UIButton>(gGobjUI, "btn_comfirm");
		btnComfrim.onClick.Add(new EventDelegate(this, "OnRegisterComfirm"));
	}
	
	void GeneralShowUI(string path){
		if(gGobjUI != null){
			DestroyObject(gGobjUI);
		}
		gGobjUI = Tools.AddNGUIChild(gUIParent, path);
	}
	
	void OnRegisterComfirm(){
		UIInput uiName = Tools.GetComponentInChildByPath<UIInput>(gGobjUI, "input_name");
		StartCoroutine(SendRegister(uiName.value));
	}
	
	IEnumerator SendRegister(string playerName){
		WWW myWWW = new WWW("http://localhost/cubeworld/register.php?playername=" + playerName);
		yield return  myWWW;
		
		print(myWWW.text);//##########
		
//		JSONNode jd = JSONNode.Parse(myWWW.text);
		
	}
}
