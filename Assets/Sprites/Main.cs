using UnityEngine;
using System.Collections;

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
	
	public UIToggle ut_create;
	public UIToggle ut_createPlayer;
	public UIToggle ut_move;
	public UIToggle ut_rotate;
	public UIToggle ut_destroy;
	public UIButton btn_play;
	public UIButton btn_save;
	public UIButton btn_load;
	
	Vector3 touchPos;
	EModel model;
	
	Vector3 touchPosFirst;
	
	void Start () {
		ut_create.onChange.Add(new EventDelegate(this, "OnModelChange"));
		ut_createPlayer.onChange.Add(new EventDelegate(this, "OnModelChange"));
		ut_move.onChange.Add(new EventDelegate(this, "OnModelChange"));
		ut_rotate.onChange.Add(new EventDelegate(this, "OnModelChange"));
		ut_destroy.onChange.Add(new EventDelegate(this, "OnModelChange"));
		
		btn_play.onClick.Add(new EventDelegate(this, "OnBtnPlay"));
		btn_save.onClick.Add(new EventDelegate(this, "OnBtnSave"));
		btn_load.onClick.Add(new EventDelegate(this, "OnBtnLoad"));
		
		StartCoroutine(CoBtnLoad());
	}
	
	// Update is called once per frame
	void Update () {
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
						GameObject gobjCube = Instantiate(cube) as  GameObject;
						gobjCube.transform.parent = parent.transform;
						gobjCube.transform.position = posNew ;
						gobjCube.transform.localEulerAngles = Vector3.zero;
					}
					else if(model == EModel.Destroy){
						if(gobjHit != oriCube){
							DestroyObject(gobjHit);
						}
					}
					else if(model == EModel.CreatePlayer){
						Vector3 normalHit = rh.normal;
						Vector3 posNew = gobjHit.transform.position + normalHit;
						GameObject gobjCube = Tools.LoadResourcesGameObject("Prefabs/CubePlayerPos");
						gobjCube.transform.parent = parent.transform;
						gobjCube.transform.position = posNew ;
						gobjCube.transform.localEulerAngles = Vector3.zero;
					}
				}
			}
		}
	}
	
	public void OnModelChange(){
		UIToggle utCur = UIToggle.current;
		if(utCur != null && (utCur.value == true)){
			if(utCur.name.Equals("tg_create")){
				model = EModel.Create;
			}else if(utCur.name.Equals("tg_move")){
				model = EModel.Move;
			}else if(utCur.name.Equals("tg_rotate")){
				model = EModel.Rotate;
			}else if(utCur.name.Equals("tg_destroy")){
				model = EModel.Destroy;
			}else if(utCur.name.Equals("tg_create_player")){
				model = EModel.CreatePlayer;
			}
		}
	}
	
	void OnDrag(DragGesture gesture){
		if(model == EModel.Move){
			Vector3 move = gesture.DeltaMove * 0.05f;
			parent.transform.Translate(move, Space.World);
		}else if(model == EModel.Rotate){
			Vector3 move = gesture.DeltaMove * 0.3f;
			parent.transform.Rotate(move.y, -1 * move.x, 0f, Space.World);
		}
		
	}
	
	void OnPinch(PinchGesture gesture){
		float delta = gesture.Delta;
		Vector3 pos = camera.transform.position;
		pos.z += (delta * 0.05f);
		camera.transform.position = pos;
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
				float localX = float.Parse(posVals[0]);
				float localY = float.Parse(posVals[1]);
				float localZ = float.Parse(posVals[2]);
				if(!(localX == 0 && localY == 0 && localZ == 0)){
					GameObject cube = Tools.LoadResourcesGameObject("Prefabs/Cube");
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
			GameObject cube = Tools.LoadResourcesGameObject("Prefabs/CubePlayerPos");
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
		
		string strCubesData = "";
		string strPlayerPosData = "";
		foreach (Transform tfCube in parent.transform) {
			if(tfCube.CompareTag("Cube")){
				string strData = tfCube.localPosition.x + "," + tfCube.localPosition.y + "," + tfCube.localPosition.z + "|";
				strCubesData += strData;
			}else if(tfCube.CompareTag("PlayerPos")){
				strPlayerPosData = tfCube.position.x + "," + tfCube.position.y + "," + tfCube.position.z;
			}
		}
		// 方块保存数据
		if(!string.IsNullOrEmpty(strCubesData)){
			PlayerPrefs.SetString("cubes", strCubesData);
		}
		
		if(!string.IsNullOrEmpty(strPlayerPosData)){
			PlayerPrefs.SetString("playerpos", strPlayerPosData);
		}
	}
}
