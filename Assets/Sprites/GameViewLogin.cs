using UnityEngine;
using System.Collections;
using SimpleJSON;

public class GameViewLogin : MonoBehaviour {
	
	GameObject gGobjUI;
	
	public GameObject gUIParent;
	
	void Start () {
		if(GameManager.JDPlayerList != null){
			ShowUIMainMenu(GameManager.JDPlayerList);
		}else{
			// init start
			GameResources.InitBaseData();
			// init end
			
			GetLocalSavedPlayerId();
			if(GameManager.CurPlayerId >= 0){
				// 本地存有帐号，显示登录
				ShowUILogin();
			}else{
				// 无帐号，显示注册
				ShowUIRegister();
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.F1)){
			PlayerPrefs.DeleteAll();
			print("本地帐号移除");
		}
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
	
	void SavePlayerToLocla(){
		PlayerPrefs.SetInt("playerid", GameManager.CurPlayerId);
		PlayerPrefs.SetString("playername", GameManager.CurPlayerName);
	}
	
	
	
	void ShowUILogin(){
		GeneralShowUI(IPath.UI + "ui_login");
		UILabel txtName = Tools.GetComponentInChildByPath<UILabel>(gGobjUI, "name");
		txtName.text = GameManager.CurPlayerName;
		UIButton btnLogin = Tools.GetComponentInChildByPath<UIButton>(gGobjUI, "btn_login");
		GeneralAddBtnAction(btnLogin, "OnLoginComfirm");
	}
	
	void ShowUIRegister(){
		GeneralShowUI(IPath.UI + "ui_register");
		UIButton btnComfrim = Tools.GetComponentInChildByPath<UIButton>(gGobjUI, "btn_comfirm");
		btnComfrim.onClick.Add(new EventDelegate(this, "OnRegisterComfirm"));
	}
	
	void ShowUIMainMenu(JSONNode playersData){
		GeneralShowUI(IPath.UI + "ui_mainmenu");
		
		// 玩家列表显示
		GameObject gobjGrid = Tools.GetGameObjectInChildByPathSimple(gGobjUI, "playerlist/grid");
		for (int i = 0; i < playersData.Count; i++) {
			JSONNode item = playersData[i];
			int id = item["id"].AsInt;
			string name = item["name"];
			
			GameObject gobjItem = Tools.AddNGUIChild(gobjGrid, IPath.UI + "player_item");
			gobjItem.name = "player_" + id;
			UILabel txtInfo = Tools.GetComponentInChildByPath<UILabel>(gobjItem, "playerinfo");
			txtInfo.text = name;
		}
		
		// 编辑按钮监听
		UIButton btnEdit = Tools.GetComponentInChildByPath<UIButton>(gGobjUI, "myworld/btn_edit");
		btnEdit.onClick.Add(new EventDelegate(this, "OnToEdit"));
	}
	
	void GeneralShowUI(string path){
		if(gGobjUI != null){
			DestroyObject(gGobjUI);
		}
		gGobjUI = Tools.AddNGUIChild(gUIParent, path);
	}
	
#region BtnHandler
	public void OnBtnClick(GameObject gobjBtn){
		string btnName = gobjBtn.name;
		if(btnName.Contains("player")){
			// ToDO 玩家列表点击
			int playerid = int.Parse(btnName.Split('_')[1]);
			StartCoroutine(CoRequestWorldData(playerid));
		}
	}
	
	// 玩家列表点击
	IEnumerator CoRequestWorldData(int playerid){
		WWW myWWW = new WWW("http://localhost/cwserver/getworlddata.php?playerid=" + playerid);
		yield return  myWWW;
		
		string strRes = myWWW.text;
		
		print("Response:" + strRes);//##########
		
		if(!string.IsNullOrEmpty(strRes)){
			JSONNode jd = JSONNode.Parse(strRes);
			
			string strWorlddata = jd["worlddata"];
			
			GameManager.OtherWorldDataCache = strWorlddata;
			GameManager.PlayModel = EPlayModel.Others;
			Application.LoadLevel("Play");
		}
	}
	
	void OnToEdit(){
		Application.LoadLevel("Main");
	}
	
	void OnRegisterComfirm(){
		UIInput uiName = Tools.GetComponentInChildByPath<UIInput>(gGobjUI, "input_name");
		StartCoroutine(SendRegister(uiName.value));
	}
	void OnLoginComfirm(){
		StartCoroutine(CoRequest_Login(GameManager.CurPlayerId));
	}
	
#endregion
	
	#region CoRequset
	
	IEnumerator SendRegister(string playerName){
		WWW myWWW = new WWW("http://localhost/cwserver/register.php?playername=" + playerName);
		yield return  myWWW;
		
		string strRes = myWWW.text;
		
		print("Response:" + strRes);//##########
		
		JSONNode jd = JSONNode.Parse(strRes);
		
		int state = jd["state"].AsInt;
		if(state == 0){ //成功
			int playerid = jd["playerid"].AsInt;
			string playername = jd["playername"];
			GameManager.CurPlayerId = playerid;
			GameManager.CurPlayerName = playername;
			SavePlayerToLocla();
			// 跳转登录
			ShowUILogin();
		}else if(state == 1){ //重名
			UILabel txtTip = Tools.GetComponentInChildByPath<UILabel>(gGobjUI, "txt_tip");
			txtTip.text = "已被使用";
			txtTip.color = Color.red;
		}
	}
	
	IEnumerator CoRequest_Login(int playerid){
		
		string strReq = "http://localhost/cwserver/login.php?playerid=" + playerid;
		WWW myWWW = new WWW(strReq);
		
		print("SendRequest:" + strReq);//#########
		
		yield return  myWWW;
		
		string strRes = myWWW.text;
		
		print("Response:" + strRes);//##########
		
		JSONNode jd = JSONNode.Parse(strRes);
		
		int state = jd["state"].AsInt;
		if(state == 0){
			// 登录成功，跳转主界面
			GameManager.JDPlayerList = jd["players"]; 
			ShowUIMainMenu(GameManager.JDPlayerList);
		}else{
			// 登录失败
			UILabel txtTip = Tools.GetComponentInChildByPath<UILabel>(gGobjUI, "txt_tip");
			txtTip.text = "登录失败";
		}
		
		
	}
	
	#endregion
	
#region General
	void GeneralAddBtnAction(UIButton btn, string method){
		btn.onClick.Add(new EventDelegate(this, method));
	}
	
//	IEnumerator GeneralCoRequest(string phpModelName, string strParams, out string strResponse){
//		string strReq = "http://localhost/cwserver/" + phpModelName +".php?" + strParams;
//		
//		print("SendRequest:" + strReq);
//		
//		WWW myWWW = new WWW(strReq);
//		yield return myWWW;
//		
//		strResponse = myWWW.text;
//	
//		print("Response:" + strRes);//##########
//	}
#endregion
}
