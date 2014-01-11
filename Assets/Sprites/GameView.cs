using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public enum EGameState{
	Running,
	UIInfoing,
	UIPaging
}

public class GameView : MonoBehaviour
{
	
	public int VCInput_Axis;
	public int VCInput_Ver_Axis;
	public int VCInput_BtnA;
	public int VCInput_BtnB;
	
	public EGameState gameState;
	
	public Camera main_camera;
	
	private Hero hero;
	
	public GameObject cubeParent;
	
	bool initFinish = false;
	
	void Start ()
	{
		// init hero
		StartCoroutine(InitWorld());
	}
	
	// Update is called once per frame
	void Update ()
	{
		// keyboard controll
		/// for test.when build， close it
		#if UNITY_EDITOR||UNITY_STANDALONE_WIN
		if(Input.GetKey(KeyCode.A)){
			VCInput_Axis = -1;
		}else if(Input.GetKey(KeyCode.D)){
			VCInput_Axis = 1;
		}else{
			VCInput_Axis = 0;
		}
		
		if(Input.GetKey(KeyCode.W)){
			VCInput_Ver_Axis = 1;
		}else if(Input.GetKey(KeyCode.S)){
			VCInput_Ver_Axis = -1;
		}else{
			VCInput_Ver_Axis = 0;
		}
		
		if(Input.GetKeyDown(KeyCode.Space)){
			VCInput_BtnA = 1;
		}else if(Input.GetKeyUp(KeyCode.Space)){
			VCInput_BtnA = 0;
		}
		
		if(Input.GetKeyDown(KeyCode.X)){
			VCInput_BtnB = 1;
		}else if(Input.GetKeyUp(KeyCode.X)){
			VCInput_BtnB = 0;
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
				VCInput_BtnA = 1;
			}else{
				VCInput_BtnA = 0;
			}
		}
		if("btn_B".Equals(btnname)){
			if(isDown){
				VCInput_BtnB = 1;
			}else{
				VCInput_BtnB = 0;
			}
		}
	}
	
	public bool IsInGameState(EGameState gameState){
		return this.gameState == gameState;
	}
	
	void OnDrag(DragGesture drag){
		Vector2 deltaMove = drag.DeltaMove * 0.1f;
		hero.transform.Rotate(0f, -1 * deltaMove.x, 0f, Space.World);
		
		main_camera.transform.Rotate(deltaMove.y, -1 * deltaMove.x, 0f, Space.World);
		Vector3 cameraAngle = main_camera.transform.eulerAngles;
		cameraAngle.z = 0;
		main_camera.transform.eulerAngles = cameraAngle;
	}
	
	IEnumerator InitWorld(){
		string strCubesData = PlayerPrefs.GetString("cubes");
		string[] strCubes = strCubesData.Split('|');
		foreach (string item in strCubes) {
			
			if(string.IsNullOrEmpty(item)){
				continue;
			}
			
			string[] posVals = item.Split(',');
			float localX = float.Parse(posVals[0]);
			float localY = float.Parse(posVals[1]);
			float localZ = float.Parse(posVals[2]);
			GameObject cube = Tools.LoadResourcesGameObject("Prefabs/Cube");
			cube.transform.parent = cubeParent.transform;
			Vector3 locPos = new Vector3(localX, localY, localZ);
			cube.transform.localPosition = locPos;
			yield return 1;
		}
		
		// 创建玩家
		string posPlayer = PlayerPrefs.GetString("playerpos");
		string[] posPlayerVals = posPlayer.Split(',');
		float x = float.Parse(posPlayerVals[0]);
		float y = float.Parse(posPlayerVals[1]);
		float z = float.Parse(posPlayerVals[2]);
		Vector3 playerPos = new Vector3(x, y + 0.6f, z);
		
		GameObject gobjHero = Tools.LoadResourcesGameObject("Prefabs/player");
		gobjHero.transform.position = playerPos;
		
		CameraControll camealControll = main_camera.GetComponent<CameraControll>();
		camealControll.target = gobjHero;
		
		hero = gobjHero.GetComponent<Hero>();
		
		initFinish = true;
		
		yield return 1;
	}
}