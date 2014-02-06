using UnityEngine;
using System.Collections;
using System.ComponentModel;
using System;
using SimpleJSON;

public enum ECommandAI{
	COMMAND_NONE,
	COMMAND_TARGET_PLAYER,
	COMMAND_IDLE,
	COMMAND_ATTACK_PLAYER
}

public enum EActorState{
	STATE_MOVE,
	STATE_IDLE,
	STATE_ATTACK,
    STATE_ATTACKINTERVAL
}

public enum EFSMAction {
    NONE,
    MOVE,
    ATTACK_PLAYER,
    IDLE,
	UNATTACK,
	UNATTACK_BY_FLASH,
    ATTACK_INTERVAL,
	HERO_IDLE,
	HERO_RUN,
	HERO_ONAIR_DOWN,
	HERO_ONAIR_UP,
	HERO_DIE
}

public enum ERuneStoneType{
	NONE,
	JUMP
}

public enum EActorType{
	Gard,
	Hero
}

public enum EBuildItemType{
	Base,
	Active
}

public class BuildItem{
	public int id;
	public string name;
	public string resourceName;
	public EBuildItemType type;
	
	public BuildItem(JSONNode jd){
		id = jd["id"].AsInt;
		name = jd["name"];
		resourceName = jd["resources_name"];
		type = (EBuildItemType)jd["type"].AsInt;
	}
}

public class PlayerInfo{
	public int id;
	public string name;
}