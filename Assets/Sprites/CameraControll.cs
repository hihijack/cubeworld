using UnityEngine;
using System.Collections;

public class CameraControll : MonoBehaviour {

	public GameObject target;
	GameView gameView;
	// Use this for initialization
	
	public float distance;
	public float height;
	
	private float rotateY;
	
	
	void Start () {
		gameView = GameObject.Find("CPU").GetComponent<GameView>();
	}
	
	// Update is called once per frame
	void Update () {
		if(target != null){
				Vector3 offset = target.transform.forward * -1 * distance;
				offset.y += height;
		
				transform.position = target.transform.position + offset;
		}
		
//		Vector3 rotateAngle = target.transform.eulerAngles;
//		rotateAngle.z = 0f;
//		transform.eulerAngles = rotateAngle;
	}
}
