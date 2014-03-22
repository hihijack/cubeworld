using UnityEngine;
using System.Collections;
using SimpleJSON;

public enum EGameModel{
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
//	public static string ServerIP = "172.18.10.13";
	public static string ServerIP = "localhost";
//	public static string ServerIP = "hihijack.sinaapp.com";
	
	public static bool debug = false;
	
	public static int LoadCountPerFrame = 10;
	
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

	public static int CurTargetid = 0;
	public static string CurTargetName = "";
	public static int CurVersion = 0;
	
	public static bool HasLogin = false;
	
	public static EGameModel _gameModel;
	public static EGameModel GameModel{
		get{
			return _gameModel;
		}
		set{
			_gameModel = value;
		}
	}
}
