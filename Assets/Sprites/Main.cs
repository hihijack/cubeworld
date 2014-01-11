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
		PlayerPrefs.SetString("cubes", strCubesData);
		PlayerPrefs.SetString("playerpos", strPlayerPosData);
		
		Application.LoadLevel("Play");
	}
}
