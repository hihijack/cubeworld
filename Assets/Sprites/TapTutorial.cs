using UnityEngine;
using System.Collections;

public class TapTutorial : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTap( TapGesture gesture )
    {
   		Debug.LogError("tap");
    }
}
