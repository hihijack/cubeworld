using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public static class GameResources{
	public static Dictionary<int, BuildItem> dicItems = new Dictionary<int, BuildItem>();
	
	public static void AddBuildItemBD(BuildItem item){
		if(!dicItems.ContainsKey(item.id)){
			dicItems.Add(item.id, item);	
		}
	}
	
	public static BuildItem GetBuildItemBD(int id){
		BuildItem builditem = null;
		if(dicItems.ContainsKey(id)){
			builditem = dicItems[id];
		}else{
			Debug.LogError("GetBuildItemBD Error:" + id);
		}
		return builditem;
	}
	
	public static void InitBaseData(){
		JSONNode jdNodes = JSONNode.Parse((Resources.Load("GameData/Items",typeof(TextAsset)) as TextAsset).ToString());
		for (int i = 0; i < jdNodes.Count; i++) {
			JSONNode item = jdNodes[i];
			BuildItem buildItem = new BuildItem(item);
			AddBuildItemBD(buildItem);
		}
	}
}
