using UnityEngine;
using System.Collections;

public class GameViewLogo : MonoBehaviour {

	public UILabel uiTxt;
	public GameObject gGobjClickTip;
	
	string tip1 = "创建属于自己的世界";
	string tip2 = "为前来探险的其他玩家设下挑战";
	string tip3 = "或者到其他玩家的世界探险，找出隐藏其中的宝箱，你将得到突破他人挑战的徽章";
	string[] arrTips;
	int index = 0;
	// Use this for initialization
	void Start () {
		arrTips = new string[3]{tip1, tip2, tip3};
		uiTxt.text = arrTips[index];
//		StartCoroutine(CoClickTip());
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonUp(0)){
			index ++;
			if(index < arrTips.Length){
				uiTxt.text = arrTips[index];
			}else{
				Application.LoadLevel("Login");
			}
		}
	}
	
	IEnumerator CoClickTip(){
		while(true){
			if(gGobjClickTip.activeSelf){
				gGobjClickTip.SetActive(false);
			}else{
				gGobjClickTip.SetActive(true);
			}
			yield return new WaitForSeconds(0.5f);
		}
	}
}
