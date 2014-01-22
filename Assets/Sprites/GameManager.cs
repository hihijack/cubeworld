using UnityEngine;
using System.Collections;
using SimpleJSON;

public enum ESceneType{
	MainMenu,
	Edit,
	Play
}

/// <summary>
/// 进入的世界与当前玩家关系
/// </summary>
public enum EPlayModel{
	Mine,
	Others
}

public static class GameManager{
	public static int CurPlayerId;
	public static string CurPlayerName;
	public static string MyWorldDataCache;
	public static string OtherWorldDataCache;
	
	public static JSONNode JDPlayerList;
		

	private static EPlayModel _playModel;
	/// <summary>
	/// 进入的世界与当前玩家关系
	/// </summary>
	public  static EPlayModel PlayModel{
		get{
			return _playModel;
		}
		set{
			_playModel = value;
		}
	}
	
	public static bool IsVerify = false;
}
