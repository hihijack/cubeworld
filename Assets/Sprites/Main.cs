using UnityEngine;
using System.Collections;
using System.Text;
using SimpleJSON;

public enum EModel{
	Create,
	Move,
	Rotate,
	Destroy,
	CreatePlayer
}

public class Main : MonoBehaviour {

	public GameObject cube;
	public GameObject parent;
	
	public Camera camera;
	public Camera cameraUI;
	
	public GameObject oriCube;
	
	public GameObject g_GobjBtns;
	public GameObject g_GobjPlane;
	
	public UIToggle ut_destroy;
	public UIButton btn_play;
	public UIButton btn_save;
	public UIButton btn_back;
	
	public float cameraSpeed = 2f;
	
	Vector3 touchPos;
	EModel model;
	
	Vector3 touchPosFirst;
	
	public GameObject g_UI_Items;
	
	BuildItem _curBuildItem;
	BuildItem CurBuildItem{
		set{
			_curBuildItem = value;
		}
		
		get{
			return _curBuildItem;
		}
	}
	
	int[] defaultBuildItemId = {1, 2, 3, 4, 5, 6};
	
	int axisV = 0;
	int axisH = 0;
	
	void Start () {
		GameManager.GameModel = EGameModel.Edit;
		
		InitBuildItemUI();
		
		UISetVerifty();
		
		ut_destroy.onChange.Add(new EventDelegate(this, "OnModelChange"));
		
		btn_play.onClick.Add(new EventDelegate(this, "OnBtnPlay"));
		btn_save.onClick.Add(new EventDelegate(this, "OnBtnSave"));
		
		btn_back.onClick.Add(new EventDelegate(this, "OnBackToMenu"));
		
		if(!string.IsNullOrEmpty(GameManager.MyWorldDataCache)){
			StartCoroutine(CoInitWorld(GameManager.MyWorldDataCache));
		}else{
			StartCoroutine(CoRequestWorldData());
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		#if UNITY_EDITOR||UNITY_STANDALONE_WIN||UNITY_WEBPLAYER
		if(Input.GetKey(KeyCode.A)){
			axisH = -1;
		}else if(Input.GetKey(KeyCode.D)){
			axisH = 1;
		}else{
			axisH = 0;
		}
		
		if(Input.GetKey(KeyCode.W)){
			axisV = 1;
		}else if(Input.GetKey(KeyCode.S)){
			axisV = -1;
		}else{
			axisV = 0;
		}
		#endif
		
		
		camera.transform.Translate(axisH * cameraSpeed, axisV * cameraSpeed, 0);
		
		if(Input.GetMouseButtonDown(0)){
			
			Vector3 posMouse = Input.mousePosition;
			posMouse.z = 10;
			
			Ray ray = camera.ScreenPointToRay(posMouse);
			RaycastHit rh;
			if(Physics.Raycast(ray, out rh)){
				if(!Tools.IsTouchLayer(cameraUI, "UI")){
					GameObject gobjHit = rh.collider.gameObject;
					if(model == EModel.Create){
						Vector3 normalHit = rh.normal;
						Vector3 posNew = gobjHit.transform.position + normalHit;
						
						GameObject gobjItem = Tools.LoadResourcesGameObject(IPath.BuildItems + CurBuildItem.resourceName);
						gobjItem.name = CurBuildItem.resourceName;
						
						gobjItem.transform.parent = parent.transform;
						gobjItem.transform.position = posNew ;
						gobjItem.transform.localEulerAngles = Vector3.zero;
						
						if(GameManager.IsVerify){
							UISetVerifty(false);
						}
					}
					else if(model == EModel.Destroy){
						if(gobjHit != oriCube){
							DestroyObject(gobjHit);
							if(GameManager.IsVerify){
								UISetVerifty(false);
							}
						}
					}
				}
			}
		}
	}
	
	public void OnModelChange(){
		
		UIToggle utCur = UIToggle.current;
		if(utCur != null && (utCur.value == true)){
			if(utCur.name.Contains("tg_create")){
				model = EModel.Create;
				DataCache dc = utCur.gameObject.GetComponent<DataCache>();
				CurBuildItem = dc.data as BuildItem;
			}else if(utCur.name.Equals("tg_move")){
				model = EModel.Move;
			}else if(utCur.name.Equals("tg_rotate")){
				model = EModel.Rotate;
			}else if(utCur.name.Equals("tg_destroy")){
				model = EModel.Destroy;
			}
		}
	}
	
	void OnDrag(DragGesture gesture){
		if(!HasUI()){
			Vector3 move = gesture.DeltaMove * 0.3f;
			camera.transform.Rotate(-1 * move.y, move.x, 0f);
			Vector3 angle = camera.transform.eulerAngles;
			angle.z = 0;
			camera.transform.eulerAngles = angle;
		}
//		if(model == EModel.Move){
//			Vector3 move = gesture.DeltaMove * 0.05f;
//			camera.transform.Translate(move, Space.World);
//		}else if(model == EModel.Rotate){
//			
//		}
	}
	
	void OnPinch(PinchGesture gesture){
		if(!HasUI()){
			float delta = gesture.Delta;
			camera.transform.Translate(0f, 0f, delta * 0.05f);
		}
	}
	
	bool HasUI(){
		return g_UI_Items.activeSelf;
	}
	
	void OnBtnPlay(){
		Save(true);
	}
	
	void OnBtnSave(){
		Save(false);
	}
	
//	void OnBtnLoad(){
//		StartCoroutine(CoRequestWorldData());
//	}
	
	IEnumerator CoRequestWorldData(){
		WWW myWWW = new WWW("http://" + GameManager.ServerIP +"/cwserver/getworlddata.php?playerid=" + GameManager.CurPlayerId);
		yield return  myWWW;
		
		string strRes = myWWW.text;
		
		print("Response:" + strRes);//##########
		
		
		if(!string.IsNullOrEmpty(strRes)){
			JSONNode jd = JSONNode.Parse(strRes);
			string strWorlddata = jd["worlddata"];
			int verify = jd["verify"].AsInt;
			
			GameManager.IsVerify = verify > 0 ? true : false;
			
			if(!string.IsNullOrEmpty(strWorlddata) && !strWorlddata.Equals("null")){
				GameManager.MyWorldDataCache = strWorlddata;
				StartCoroutine(CoInitWorld(strWorlddata));
			}
		}
		
		UISetVerifty();
		
	}
	
	void UISetVerifty(){
		UILabel txtVerify = Tools.GetComponentInChildByPath<UILabel>(g_GobjPlane, "btns/txt_verify");
		if(GameManager.IsVerify){
			txtVerify.text = "已验证";
			txtVerify.color = Color.green;
		}else{
			txtVerify.text = "未验证";
			txtVerify.color = Color.red;
		}
	}
	
	void UISetVerifty(bool verify){
		UILabel txtVerify = Tools.GetComponentInChildByPath<UILabel>(g_GobjPlane, "btns/txt_verify");
		if(verify){
			txtVerify.text = "已验证";
			txtVerify.color = Color.green;
		}else{
			txtVerify.text = "未验证";
			txtVerify.color = Color.red;
		}
	}
	
	IEnumerator CoInitWorld(string worlddata){
		string[] strsData = worlddata.Split('_');
		
		string strCubesData = strsData[0];
		
		GameObject gobjUILoading = Tools.AddNGUIChild(g_GobjPlane, IPath.UI + "ui_loading");
		UILabel txt = Tools.GetComponentInChildByPath<UILabel>(gobjUILoading, "txt");
		UIProgressBar upb = Tools.GetComponentInChildByPath<UIProgressBar>(gobjUILoading, "pb");
		
		if(!string.IsNullOrEmpty(strCubesData)){
			// 删除已有，除了原方块
			foreach (Transform tfCube in parent.transform) {
				if(tfCube.gameObject != oriCube){
					Destroy(tfCube);
					yield return 1;
				}
			}
			
			// 加载子物体，除了原方块（位置为000）
			string[] strCubes = strCubesData.Split('|');
			
			int allCount = strCubes.Length + 1;
			int curCount = 0;
			
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
				if(!(localX == 0 && localY == 0 && localZ == 0)){
					GameObject cube = Tools.LoadResourcesGameObject(IPath.BuildItems + itemName);
					cube.name = itemName;
					cube.transform.parent = parent.transform;
					Vector3 locPos = new Vector3(localX, localY, localZ);
					cube.transform.localPosition = locPos;
					cube.transform.localEulerAngles = Vector3.zero;
					yield return 1;
				}
			}
		}
		
		// 创建玩家位置
		string posPlayer = strsData[1];
		if(!string.IsNullOrEmpty(posPlayer)){
			string[] posPlayerVals = posPlayer.Split(',');
			float x = float.Parse(posPlayerVals[0]);
			float y = float.Parse(posPlayerVals[1]);
			float z = float.Parse(posPlayerVals[2]);
			GameObject cube = Tools.LoadResourcesGameObject(IPath.BuildItems + "player");
			cube.transform.parent = parent.transform;
			Vector3 locPos = new Vector3(x, y, z);
			cube.transform.localPosition = locPos;
			cube.transform.localEulerAngles = Vector3.zero;
			yield return 1;
		}
		
		DestroyObject(gobjUILoading);
	}
	
	void Save(bool toPlay){
		parent.transform.position = Vector3.zero;
		parent.transform.eulerAngles = Vector3.zero;
		
		string strPlayerPosData = "";
		string strCubesData = "";
		
		StringBuilder strBuilder = new StringBuilder();
		
		bool hasChest = false;
		
		foreach (Transform tfCube in parent.transform) {
			if(tfCube.CompareTag("PlayerPos")){
				strPlayerPosData = tfCube.position.x + "," + tfCube.position.y + "," + tfCube.position.z;
			}else{
				if(tfCube.name.Equals("chest")){
					hasChest = true;
				}
				string strData = tfCube.name + "," + tfCube.localPosition.x + "," + tfCube.localPosition.y + "," + tfCube.localPosition.z + "|";
				strBuilder.Append(strData);	
			}
		}
		
		strCubesData = strBuilder.ToString();
		
		// 方块保存数据
		if(!hasChest){
			GeneralShowTip("必须至少创建一个宝箱");
		}else{
			if(!string.IsNullOrEmpty(strCubesData) && !string.IsNullOrEmpty(strPlayerPosData)){
				string strData = strCubesData + "_" + strPlayerPosData;
				StartCoroutine(CoRequestSaveCubeData(strData, toPlay));
			}else{
				GeneralShowTip("必须创建一个玩家初始位置");
			}
		}
	}
	
	IEnumerator CoRequestSaveCubeData(string cubedata, bool toPlay){
		WWW myWWW = new WWW("http://" + GameManager.ServerIP +"/cwserver/saveworlddata.php?playerid=" + GameManager.CurPlayerId + "&" + "worlddata=" + cubedata);
		yield return  myWWW;
		
		string strRes = myWWW.text;
		
		print("Response:" + strRes);//##########
		
		JSONNode jd = JSONNode.Parse(strRes);
		int state = jd["state"].AsInt;
		
		if(state == 0){
			// 成功
			GeneralShowTip("保存成功");
			GameManager.MyWorldDataCache = cubedata;
			if(toPlay){
				GameManager.PlayModel = EPlayModel.Mine;
				GameManager.IsVerify = false;
				Application.LoadLevel("Play");
			}
		}else{
			// 失败
			GeneralShowTip("保存失败");
		}
	}
	
	void InitBuildItemUI(){
		for (int i = 0; i < defaultBuildItemId.Length; i++) {
			int id = defaultBuildItemId[i];
			BuildItem buildItem = GameResources.GetBuildItemBD(id);
			
			if(buildItem != null){
				string iconName = "icon_" + buildItem.resourceName;
				GameObject gobjItem = Tools.GetGameObjectInChildByPathSimple(g_GobjBtns, "tg_create_" + i);
				UISprite icon = Tools.GetComponentInChildByPath<UISprite>(gobjItem, "icon");
				icon.spriteName = iconName;
				DataCache dc = gobjItem.AddComponent<DataCache>();
				dc.data = buildItem;
				
				UIToggle ut = gobjItem.GetComponent<UIToggle>();
				ut.onChange.Add(new EventDelegate(this, "OnModelChange"));
				
				if(ut.startsActive){
					CurBuildItem = buildItem;
				}
			}
		}
		
		// 列表初始化
		GameObject gobjGridBase = Tools.GetGameObjectInChildByPathSimple(g_GobjBtns, "ui_items/items_base/listview/grid");
		UIGrid gridBase = gobjGridBase.GetComponent<UIGrid>();
		GameObject gobjGridActive = Tools.GetGameObjectInChildByPathSimple(g_GobjBtns, "ui_items/items_active/listview/grid");
		UIGrid gridActive = gobjGridActive.GetComponent<UIGrid>();
		foreach (BuildItem item in GameResources.dicItems.Values) {
			GameObject gobjparent = null;
			if(item.type == EBuildItemType.Base){
				gobjparent = gobjGridBase;
			}else if(item.type == EBuildItemType.Active){
				gobjparent = gobjGridActive;
			}
			
			if(gobjparent != null){
				GameObject gobjItem = Tools.AddNGUIChild(gobjparent, IPath.UI + "ui_builditem");
				UISprite us = Tools.GetComponentInChildByPath<UISprite>(gobjItem, "icon");
				us.spriteName = "icon_" + item.resourceName;
				DataCache dc = gobjItem.AddComponent<DataCache>();
				dc.data = item;
			}
		}
		gridBase.Reposition();
		gridActive.Reposition();
	}
	
	public void OnBtnPress(GameObject gobjBtn, bool isDown){
		string btnname = gobjBtn.name;
		if("btn_down".Equals(btnname)){
			if(isDown){
				axisV = -1;
			}else{
				axisV = 0;
			}
		}
		if("btn_up".Equals(btnname)){
			if(isDown){
				axisV = 1;
			}else{
				axisV = 0;
			}
		}
		if("btn_left".Equals(btnname)){
			if(isDown){
				axisH = -1;
			}else{
				axisH = 0;
			}
		}
		if("btn_right".Equals(btnname)){
			if(isDown){
				axisH = 1;
			}else{
				axisH = 0;
			}
		}
		
		if(btnname.Contains("ui_builditem")){
			DataCache dc = gobjBtn.GetComponent<DataCache>();
			BuildItem bi = dc.data as BuildItem;
			
			UIToggle toggleCur = UIToggle.GetActiveToggle(1);
			DataCache dcTemp = toggleCur.GetComponent<DataCache>();
			if(dcTemp != null){
				BuildItem biTemp = dcTemp.data as BuildItem;
				
				CurBuildItem = bi;
				dcTemp.data = bi;
				UISprite usIcon = Tools.GetComponentInChildByPath<UISprite>(toggleCur.gameObject, "icon");
				usIcon.spriteName = "icon_" + bi.resourceName;
			}
		}
	}
	
	void GeneralShowTip(string txt){
		GameObject gobjTip = Tools.AddNGUIChild(g_GobjPlane, IPath.UI + "tip");
		UILabel txtTip = Tools.GetComponentInChildByPath<UILabel>(gobjTip, "txt");
		txtTip.text = txt;
		StartCoroutine(CoUITipTiming(gobjTip, 2f));
	}
	
	IEnumerator CoUITipTiming(GameObject gobj, float dur){
		yield return new WaitForSeconds(dur);
		DestroyObject(gobj);
	}
	
	
	void OnBackToMenu(){
		Application.LoadLevel("Login");
	}
}
