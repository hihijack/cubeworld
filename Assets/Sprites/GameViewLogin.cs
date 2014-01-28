using UnityEngine;
using System.Collections;
using SimpleJSON;

public class GameViewLogin : MonoBehaviour {
	
	GameObject gGobjUI;
	
	public GameObject gUIParent;
	
	public GameObject g_GobjPlane;
	
	string strDebug = "";
	
	void Start () {
		GameManager.GameModel = EGameModel.MainMenu;
		if(GameManager.HasLogin){
			ShowUIMainMenu();
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
	
	void OnGUI(){
		GUI.Label(new Rect(50f, 50f, 300f, 300f), "V0.3");
//		GUI.Label(new Rect(50f, 400f, 300f, 300f), strDebug);
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
	
	void ShowUIMainMenu(){
		GeneralShowUI(IPath.UI + "ui_mainmenu");
		// 编辑按钮监听
		UIButton btnEdit = Tools.GetComponentInChildByPath<UIButton>(gGobjUI, "myworld/btn_edit");
		btnEdit.onClick.Add(new EventDelegate(this, "OnToEdit"));
		
		// 取玩家列表
		StartCoroutine(CoRequestPlayerList());
	}
	
	IEnumerator CoRequestPlayerList(){
		WWW myWWW = new WWW("http://" + GameManager.ServerIP + "/cwserver/getplayerlist.php");
		yield return myWWW;
		
		
		string strRes = myWWW.text;
		
		print("Response:" + strRes);
		
		JSONNode jd = JSONNode.Parse(strRes);
		JSONNode playersData = jd["players"];
		 // 玩家列表显示
		GameObject gobjGrid = Tools.GetGameObjectInChildByPathSimple(gGobjUI, "playerlist/grid");
		UIGrid grid = gobjGrid.GetComponent<UIGrid>();
		for (int i = 0; i < playersData.Count; i++) {
			JSONNode item = playersData[i];
			int id = item["id"].AsInt;
			string name = item["name"];
			int allcount = item["allcount"].AsInt;
			int passcount = item["passcount"].AsInt;
			int version = item["version"].AsInt;
			
			GameObject gobjItem = Tools.AddNGUIChild(gobjGrid, IPath.UI + "player_item");
			gobjItem.name = "player_" + id;
			// 玩家信息
			string strRate = "";
			if(allcount > 0){
				float rate = (float)passcount / allcount;
				rate *= 100;
				strRate = rate.ToString("0.00");
			}else{
				strRate = "0";
			}
			
			UILabel txtInfo = Tools.GetComponentInChildByPath<UILabel>(gobjItem, "playerinfo");
			txtInfo.text = name + " " + "通过率" + strRate + "(" + passcount + " /"  + allcount + ")" + " V" + version;
			
			PlayerInfo pi = new PlayerInfo();
			pi.id = id;
			pi.name = name;
			DataCache dc = gobjItem.AddComponent<DataCache>();
			dc.data = pi;
		}
		grid.Reposition();
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
			DataCache dc = gobjBtn.GetComponent<DataCache>();
			PlayerInfo pi = dc.data as PlayerInfo;
			int playerid = pi.id;
			string playername = pi.name;
			StartCoroutine(CoRequestChallengeOther(playerid, playername));
		}
	}
	
	// 玩家列表点击
	IEnumerator CoRequestChallengeOther(int playerid, string playername){
		WWW myWWW = new WWW("http://" + GameManager.ServerIP + "/cwserver/challengeother.php?playerid=" + GameManager.CurPlayerId + "&targetid=" + playerid);
		yield return  myWWW;
		
		string strRes = myWWW.text;
		
		print("Response:" + strRes);//##########
		
		if(!string.IsNullOrEmpty(strRes)){
			JSONNode jd = JSONNode.Parse(strRes);
			
			int state = jd["state"].AsInt;
			if(state == 0){
				string strWorlddata = jd["worlddata"];
				int version = jd["version"].AsInt;
				
				GameManager.OtherWorldDataCache = strWorlddata;
				GameManager.PlayModel = EPlayModel.Others;
				GameManager.CurTargetid = playerid;
				GameManager.CurVersion = version;
				GameManager.CurTargetName = playername;
				
				Application.LoadLevel("Play");
			}else{
				GeneralShowTip("进入失败");				
			}
			
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
		WWW myWWW = new WWW("http://" + GameManager.ServerIP +"/cwserver/register.php?playername=" + playerName);
		yield return  myWWW;
		
		string strRes = "";
		
		strRes = myWWW.text;
		
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
		
		string strReq = "http://" + GameManager.ServerIP +"/cwserver/login.php?playerid=" + playerid;
		WWW myWWW = new WWW(strReq);
		
		print("SendRequest:" + strReq);//#########
		
		yield return  myWWW;
		
		string strRes = myWWW.text;
		
		print("Response:" + strRes);//##########
		strDebug = strRes;
		
		
		JSONNode jd = JSONNode.Parse(strRes);
		
		int state = jd["state"].AsInt;
		if(state == 0){
			// 登录成功
			GameManager.HasLogin = true;
			ShowUIMainMenu();
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
	
	public void GeneralShowTip(string txt){
		GameObject gobjTip = Tools.AddNGUIChild(g_GobjPlane, IPath.UI + "tip");
		UILabel txtTip = Tools.GetComponentInChildByPath<UILabel>(gobjTip, "txt");
		txtTip.text = txt;
		StartCoroutine(CoUITipTiming(gobjTip, 2f));
	}
	
	IEnumerator CoUITipTiming(GameObject gobj, float dur){
		yield return new WaitForSeconds(dur);
		DestroyObject(gobj);
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
