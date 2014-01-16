using UnityEngine;
using System.Collections;
using System.Text;

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
	
	public GameObject oriCube;
	
	public GameObject g_GobjBtns;
	public UIToggle ut_destroy;
	public UIButton btn_play;
	public UIButton btn_save;
	public UIButton btn_load;
	
	public float cameraSpeed = 2f;
	
	Vector3 touchPos;
	EModel model;
	
	Vector3 touchPosFirst;
	
	BuildItem _curBuildItem;
	BuildItem CurBuildItem{
		set{
			_curBuildItem = value;
		}
		
		get{
			return _curBuildItem;
		}
	}
	
	int[] defaultBuildItemId = {1, 2, 3};
	
	int axisV = 0;
	int axisH = 0;
	
	void Start () {
		
		// init start
		GameResources.InitBaseData();
		// init end
		
		InitBuildItemUI();
		
		ut_destroy.onChange.Add(new EventDelegate(this, "OnModelChange"));
		
		btn_play.onClick.Add(new EventDelegate(this, "OnBtnPlay"));
		btn_save.onClick.Add(new EventDelegate(this, "OnBtnSave"));
		btn_load.onClick.Add(new EventDelegate(this, "OnBtnLoad"));
		
		StartCoroutine(CoBtnLoad());
	}
	
	// Update is called once per frame
	void Update () {
		
		camera.transform.Translate(axisH * cameraSpeed, axisV * cameraSpeed, 0);
		
		if(Input.GetMouseButtonDown(0)){
			
			Vector3 posMouse = Input.mousePosition;
			posMouse.z = 10;
			
			Ray ray = camera.ScreenPointToRay(posMouse);
			RaycastHit rh;
			if(Physics.Raycast(ray, out rh)){
				
				GameObject gobjHit = rh.collider.gameObject;
				if(gobjHit.layer != LayerMask.NameToLayer("UI")){
					if(model == EModel.Create){
						Vector3 normalHit = rh.normal;
						Vector3 posNew = gobjHit.transform.position + normalHit;
						
						GameObject gobjItem = Tools.LoadResourcesGameObject(IPath.BuildItems + CurBuildItem.resourceName);
						gobjItem.name = CurBuildItem.resourceName;
						
						gobjItem.transform.parent = parent.transform;
						gobjItem.transform.position = posNew ;
						gobjItem.transform.localEulerAngles = Vector3.zero;
					}
					else if(model == EModel.Destroy){
						if(gobjHit != oriCube){
							DestroyObject(gobjHit);
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
		Vector3 move = gesture.DeltaMove * 0.3f;
		camera.transform.Rotate(move.y, -1 * move.x, 0f);
		Vector3 angle = camera.transform.eulerAngles;
		angle.z = 0;
		camera.transform.eulerAngles = angle;
//		if(model == EModel.Move){
//			Vector3 move = gesture.DeltaMove * 0.05f;
//			camera.transform.Translate(move, Space.World);
//		}else if(model == EModel.Rotate){
//			
//		}
	}
	
	void OnPinch(PinchGesture gesture){
		float delta = gesture.Delta;
		camera.transform.Translate(0f, 0f, delta * 0.05f);
	}
	
	void OnBtnPlay(){
		Save();
		Application.LoadLevel("Play");
	}
	
	void OnBtnSave(){
		Save();
	}
	
	void OnBtnLoad(){
		StartCoroutine(CoBtnLoad());
	}
	
	IEnumerator CoBtnLoad(){
		string strCubesData = PlayerPrefs.GetString("cubes");
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
			foreach (string item in strCubes) {
				
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
		string posPlayer = PlayerPrefs.GetString("playerpos");
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
	}
	
	void Save(){
		parent.transform.position = Vector3.zero;
		parent.transform.eulerAngles = Vector3.zero;
		
		string strPlayerPosData = "";
		string strCubesData = "";
		
		StringBuilder strBuilder = new StringBuilder();
		
		foreach (Transform tfCube in parent.transform) {
			if(tfCube.CompareTag("PlayerPos")){
				strPlayerPosData = tfCube.position.x + "," + tfCube.position.y + "," + tfCube.position.z;
			}else{
				string strData = tfCube.name + "," + tfCube.localPosition.x + "," + tfCube.localPosition.y + "," + tfCube.localPosition.z + "|";
				strBuilder.Append(strData);	
			}
		}
		
		strCubesData = strBuilder.ToString();
		
		// 方块保存数据
		if(!string.IsNullOrEmpty(strCubesData)){
			PlayerPrefs.SetString("cubes", strCubesData);
		}
		
		if(!string.IsNullOrEmpty(strPlayerPosData)){
			PlayerPrefs.SetString("playerpos", strPlayerPosData);
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
	}
	
	public void OnBtnPress(string btnname, bool isDown){
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
	}
}
