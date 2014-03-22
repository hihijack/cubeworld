using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using SimpleJSON;

public enum EGameState{
	Running,
	UIInfoing,
	UIPaging
}

public class GameView : MonoBehaviour
{
	
	public float VCInput_Axis;
	public float VCInput_Ver_Axis;
	private int _vcInputBtnA;
	public int VCInputBtnA{
		get{
			return _vcInputBtnA;
		}
		set{
			_vcInputBtnA = value;
		}
	}
	private int _vcInputBtnB;
	public int VCInputBtnB{
		get{
			return _vcInputBtnB;
		}
		set{
			_vcInputBtnB = value;
		}
	}
	
	public EGameState gameState;
	
	public Camera main_camera;
	
	private Hero hero;
	
	public GameObject cubeParent;
	
	bool initFinish = false;
	
	public GameObject g_GobjPlane;
	
	GameObject gGobjKeyTip;// 按键提升
	
	GameObject gGobjSecret;
	
	void Start ()
	{
		GameManager.GameModel = EGameModel.Play;
		// init hero
		StartCoroutine(InitWorld());
		
		UILabel txtTargetName = Tools.GetComponentInChildByPath<UILabel>(g_GobjPlane, "target_name/txt");
		if(GameManager.PlayModel == EPlayModel.Others){
			txtTargetName.text = GameManager.CurTargetName + "的世界";
		}else{
			txtTargetName.text =  "我的世界";
		}
		
		UILabel txtHelp = Tools.GetComponentInChildByPath<UILabel>(g_GobjPlane, "win_help/txt");
		txtHelp.text = IText.HelpPlay;
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		// keyboard controll
		/// for test.when build， close it
		#if UNITY_EDITOR||UNITY_STANDALONE_WIN||UNITY_WEBPLAYER
//		if(Input.GetKey(KeyCode.A)){
//			VCInput_Axis = -1;
//		}else if(Input.GetKey(KeyCode.D)){
//			VCInput_Axis = 1;
//		}else{
//			VCInput_Axis = 0;
//		}
//		
//		if(Input.GetKey(KeyCode.W)){
//			VCInput_Ver_Axis = 1;
//		}else if(Input.GetKey(KeyCode.S)){
//			VCInput_Ver_Axis = -1;
//		}else{
//			VCInput_Ver_Axis = 0;
//		}
		
		VCInput_Axis = Input.GetAxis("Horizontal");
		VCInput_Ver_Axis = Input.GetAxis("Vertical");
		
		if(Input.GetKeyDown(KeyCode.Space)){
			VCInputBtnA = 1;
		}else{
			VCInputBtnA = 0;
		}
		
		if(Input.GetKeyDown(KeyCode.X)){
			VCInputBtnB  = 1;
		}else{
			VCInputBtnB = 0;
		}
		
		if(Input.GetKeyDown(KeyCode.R)){
			CameraControll camealControll = main_camera.GetComponent<CameraControll>();
			camealControll.ToggleFAP();
		}
		#endif
		if(hero != null){
			hero.DoUpdate();
		}
	}
	
	void LateUpdate(){
//		VCInput_BtnA = 0;
//		VCInput_BtnB = 0;
//		VCInput_Ver_Axis = 0;
	}
	
	
	void OnGUI(){
//		GUI.Label(new Rect(50,50,100,100), VCInputBtnA.ToString());
	}
	//×××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××××
	
	public void OnBtnPress(string btnname, bool isDown){
		if("btn_down".Equals(btnname)){
			if(isDown){
				VCInput_Ver_Axis = -1;
			}else{
				VCInput_Ver_Axis = 0;
			}
		}
		if("btn_up".Equals(btnname)){
			if(isDown){
				VCInput_Ver_Axis = 1;
			}else{
				VCInput_Ver_Axis = 0;
			}
		}
		if("btn_left".Equals(btnname)){
			if(isDown){
				VCInput_Axis = -1;
			}else{
				VCInput_Axis = 0;
			}
		}
		if("btn_right".Equals(btnname)){
			if(isDown){
				VCInput_Axis = 1;
			}else{
				VCInput_Axis = 0;
			}
		}
		
		if("btn_A".Equals(btnname)){
			if(isDown){
				VCInputBtnA = 1;
			}else{
				VCInputBtnA = 0;
			}
		}
		if("btn_B".Equals(btnname)){
			if(isDown){
				VCInputBtnB  = 1;
			}else{
				VCInputBtnB = 0;
			}
		}
		
		if("btn_back".Equals(btnname)){
			if(isDown){
				if(GameManager.PlayModel == EPlayModel.Mine){
					BackToCreate();
				}else{
					BackToMenu();
				}
			}
		}
		
		if("btn_restart".Equals(btnname)){
			Application.LoadLevel("Play");
		}
	}
	
	void BackToCreate(){
		Application.LoadLevel("Main");
	}
	
	void BackToMenu(){
		Application.LoadLevel("Login");
	}
	
	public bool IsInGameState(EGameState gameState){
		return this.gameState == gameState;
	}
	
	void OnDrag(DragGesture drag){
//		Vector2 deltaMove = drag.DeltaMove * 0.1f;
//		hero.transform.Rotate(0f, deltaMove.x, 0f, Space.World);
//		
//		CameraControll camealControll = main_camera.GetComponent<CameraControll>();
//		camealControll.OnDrag(drag);
		
//		main_camera.transform.Rotate(deltaMove.y, 0, 0f, Space.World);
//		Vector3 cameraAngle = main_camera.transform.eulerAngles;
//		cameraAngle.z = 0;
//		cameraAngle.y = hero.transform.eulerAngles.y;
//		main_camera.transform.eulerAngles = cameraAngle;
	}
	
	
	public void ShowKeyTip(){
		gGobjKeyTip = Tools.AddNGUIChild(g_GobjPlane, IPath.UI + "ui_tip");
	}
	
	public void HideKeyTip(){
		if(gGobjKeyTip != null){
			DestroyObject(gGobjKeyTip);
		}
	}
	
	public void ShowSecret(string strTxt){
		gGobjSecret = Tools.AddNGUIChild(g_GobjPlane, IPath.UI + "secret");
		UILabel txt = Tools.GetComponentInChildByPath<UILabel>(gGobjSecret, "txt");
		txt.text = strTxt;
	}
	
	public void HideSecret(){
		if(gGobjSecret != null){
			DestroyObject(gGobjSecret);
		}
	}
	
	public bool IsInSecret(){
		return gGobjSecret != null;
	}
	
	IEnumerator InitWorld(){
		string worldData = "";
		if(GameManager.PlayModel == EPlayModel.Mine && !string.IsNullOrEmpty(GameManager.MyWorldDataCache)){
			worldData = GameManager.MyWorldDataCache;
		}else if(GameManager.PlayModel == EPlayModel.Others && !string.IsNullOrEmpty(GameManager.OtherWorldDataCache)){
			worldData = GameManager.OtherWorldDataCache;
		}
		
		if(!string.IsNullOrEmpty(worldData)){
			
			GameObject gobjUILoading = Tools.AddNGUIChild(g_GobjPlane, IPath.UI + "ui_loading");
			UILabel txt = Tools.GetComponentInChildByPath<UILabel>(gobjUILoading, "txt");
			UIProgressBar upb = Tools.GetComponentInChildByPath<UIProgressBar>(gobjUILoading, "pb");
			
			string[] strsData = worldData.Split('_');
			string strCubesData = strsData[0];
			string[] strCubes = strCubesData.Split('|');
			
			int allCount = strCubes.Length + 1;
			int curCount = 0;
			
			int countLoadPerFrame = 0;
			
			foreach (string item in strCubes) {
				
				// 载入进度
				curCount ++;
				float persent = (float)curCount / allCount;
				upb.value = persent;
				persent *= 100;
				txt.text = persent.ToString("0.0") + "%";
				
				if(string.IsNullOrEmpty(item)){
					continue;
				}
				
				string[] posVals = item.Split(',');
				string itemName = posVals[0];
				float localX = float.Parse(posVals[1]);
				float localY = float.Parse(posVals[2]);
				float localZ = float.Parse(posVals[3]);
				GameObject cube = Tools.LoadResourcesGameObject(IPath.BuildItems + itemName);
				
				cube.transform.parent = cubeParent.transform;
				Vector3 locPos = new Vector3(localX, localY, localZ);
				cube.transform.localPosition = locPos;
				
				countLoadPerFrame++;
				if(countLoadPerFrame >= GameManager.LoadCountPerFrame){
					countLoadPerFrame = 0;
					yield return 1;
				}
			}
			
			// 创建玩家
			string posPlayer = strsData[1];
			string[] posPlayerVals = posPlayer.Split(',');
			float x = float.Parse(posPlayerVals[0]);
			float y = float.Parse(posPlayerVals[1]);
			float z = float.Parse(posPlayerVals[2]);
			Vector3 playerPos = new Vector3(x, y + 0.6f, z);
			
			GameObject gobjHero = Tools.LoadResourcesGameObject("Prefabs/player");
			gobjHero.transform.position = playerPos;
			
			CameraControll camealControll = main_camera.GetComponent<CameraControll>();
			camealControll.target = gobjHero.transform;
			camealControll.Init();
			
			hero = gobjHero.GetComponent<Hero>();
			
			initFinish = true;
			
			yield return 1;
			
			DestroyObject(gobjUILoading);
		}
	}
	
	public void GeneralShowTipWithoutTime(string txt){
		GameObject gobjTip = Tools.AddNGUIChild(g_GobjPlane, IPath.UI + "tip");
		UILabel txtTip = Tools.GetComponentInChildByPath<UILabel>(gobjTip, "txt");
		txtTip.text = txt;
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
	
	public void PassOthers(){
		if(GameManager.CurPlayerId != GameManager.CurTargetid){
			StartCoroutine(CoRequestPass());
		}
	}
	
	IEnumerator CoRequestPass(){
		WWW myWWW = new WWW("http://" + GameManager.ServerIP +  "/cwserver/passothers.php?playerid=" + GameManager.CurPlayerId + "&targetid=" + GameManager.CurTargetid + "&version=" + GameManager.CurVersion);
		
		yield return myWWW;
		
		string strRes = myWWW.text;
		
		JSONNode jd = JSONNode.Parse(strRes);
		int state = jd["state"].AsInt;
		if(state == 0){
			GeneralShowTip("挑战成功！获得了徽章《" + GameManager.CurTargetName + "/V" + GameManager.CurVersion + "》");
		}else{
			GeneralShowTip("服务器错误");
		}
	}
}