using UnityEngine;
using System.Collections;

public class CameraControll : MonoBehaviour
{
	// The target we are following
	public Transform target;
	// The distance in the x-z plane to the target
	public float distance = 4.0f;
	// the height we want the camera to be above the target
	public float height = 3.0f;
	// How much we 
	public float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;
	// Place the script in the Camera-Control group in the component menu
	
	public float horizontalAimingSpeed = 360f;
	public float  verticalAimingSpeed = 360f;
	private float angleH = 0f;
	private float angleV = 0f;
	private float rollSpeed = 360f;
	private bool isFAP = false;//是否第一视角
	
	public Camera cameraUI;
	
	void LateUpdate ()
	{
		// Early out if we don't have a target
		if (!target)
			return;
			
		bool isRight = false;
		
		//镜头控制
		if (Input.GetMouseButton (0) || Input.GetMouseButton(1)) {
			string touchLayerName = Tools.GetTouchLayer(cameraUI);
			
			if(!touchLayerName.Contains("UI")){
				angleH += Mathf.Clamp (Input.GetAxis ("Mouse X"), -1, 1) * horizontalAimingSpeed * Time.deltaTime;
				angleV -= Mathf.Clamp (Input.GetAxis ("Mouse Y"), -1, 1) * verticalAimingSpeed * Time.deltaTime;		
				if(Input.GetMouseButton (0)){
					isRight = true;
				}
			}
		}
		//滚轮
		if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
			if (Input.GetAxis ("Mouse ScrollWheel") < 0 || distance > 2) {
				distance -= Input.GetAxis ("Mouse ScrollWheel") * rollSpeed * Time.deltaTime;   
			}
			if (distance < 2) {
				distance = 2;
			}
		}
		Quaternion aimRotation = Quaternion.Euler (angleV, angleH, 0);
		transform.position = target.position;
		transform.position -= aimRotation * Vector3.forward * distance;
		transform.LookAt (target);
		//人物转向
		if (isRight) {
			float aimHero = transform.eulerAngles.y;
			target.eulerAngles = new Vector3 (0, aimHero, 0);	
		}
	}

	void Start ()
	{
			
	}
	
	public void Init ()
	{
		Vector3 pos = target.position;
		pos -= target.rotation * Vector3.forward * distance;
		pos.y = target.position.y + height;
		transform.position = pos;
		
		// Always look at the target
		transform.LookAt (target);
		
		Quaternion rot = target.rotation;
		rot.y = transform.rotation.y;
		target.rotation = rot;
		//初始欧拉角
		angleH = transform.eulerAngles.y;
		angleV = transform.eulerAngles.x;
	}
}
