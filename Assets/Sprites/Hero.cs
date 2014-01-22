using UnityEngine;
using System.Collections;
using System.Reflection;

public class Hero : IActor
{
	
	public float speed = 6.0f;
	
	public float jumpSpeed = 5.0f;
	
	public float gravity = 20.0f;
	
//	public float pushPow = 1f;
	
	private CharacterController cc;
	
	private GameObject g_gobjCurStepOn;
	
	private Vector3 moveDir = Vector3.zero;
	
	GameView gameView;
	
	int axisH = 0;
	int axisV = 0;
	int btnA = 0;
	int btnB = 0;
	
	bool canSecondJump = false;
	
	Animation anim;
	
	IInteractive _curInteract;// 当前交互
	IInteractive CurInteract{
		get{
			return _curInteract;
		}
		
		set{
			print("set" + value);//######
			_curInteract = value;
		}
	}
	
	void Start(){
		isEnermy = false;
		actor_type = EActorType.Hero;
		cc = GetComponent<CharacterController>();
		state = new HeroActorState_Idle(this);
		gameView = GameObject.Find("CPU").GetComponent<GameView>();
		anim = Tools.GetComponentInChildByPath<Animation>(gameObject,"model");
	}
	
	void Update(){
		
	}
	
	/// <summary>
	/// 由CPU调用更新，确保执行顺序
	/// </summary>
	public void DoUpdate(){
		axisH = gameView.VCInput_Axis;
		axisV = gameView.VCInput_Ver_Axis;
		btnA = gameView.VCInputBtnA;
		btnB = gameView.VCInputBtnB;
		moveDir.z = 0f;
		moveDir.x = 0f;
		this.state.DoUpdata();
	}
	
	void OnGUI(){
//		GUI.Label(new Rect(100, 100, 100, 50), state.ToString());
//		GUI.Label(new Rect(100, 200, 100, 50), moveDir.ToString());
	}
	
	public override void DoUpdateIdle ()
	{
		moveDir.y = -0.1f;
		cc.Move(moveDir);
		if(!cc.isGrounded){
			updataState(new IActorAction(EFSMAction.HERO_ONAIR_DOWN));
		}else{
			
			if(gameView.IsInGameState(EGameState.Running)){
			
				if(axisV != 0 || axisH != 0){
					updataState(new IActorAction(EFSMAction.HERO_RUN));
				}else if(btnA > 0){
					updataState(new IActorAction(EFSMAction.HERO_ONAIR_UP));
				}else if(btnB > 0){
					InteractCur();
				}
			}
		}
	}
	
	public override void DoUpdateRun ()
	{
		if(cc.isGrounded){
			if(axisV != 0 || axisH != 0){
				moveDir = transform.forward * axisV * speed + transform.right * axisH * speed;
				moveDir.y = -0.1f;
				cc.Move(moveDir * Time.deltaTime);
			}else{
				updataState(new IActorAction(EFSMAction.HERO_IDLE));
			}
			
			if(btnA > 0){
				updataState(new IActorAction(EFSMAction.HERO_ONAIR_UP));
			}	
		}else{
			updataState(new IActorAction(EFSMAction.HERO_ONAIR_DOWN));
		}
	}
	
	public override void OnEnterIdle ()
	{
		PlayAnim("Stand");
	}
	
	public override void OnEnterRun ()
	{
		PlayAnim("Run");
	}
	
	public override void OnEnterOnAirDown ()
	{
	}
	
	public override void DoUpdateOnAirDown ()
	{
		if(axisV != 0 || axisH != 0){
			Vector3 moveHoriz = transform.forward * axisV * speed + transform.right * axisH * speed;
			moveDir.x = moveHoriz.x;
			moveDir.z = moveHoriz.z;
		}
		moveDir.y -= gravity * Time.deltaTime;
		
		cc.Move(moveDir * Time.deltaTime);
		
		if(cc.isGrounded){
			updataState(new IActorAction(EFSMAction.HERO_IDLE));
		}
		
//		if(canSecondJump && btnA > 0){
//			updataState(new IActorAction(EFSMAction.HERO_ONAIR_UP));
//		}
	}
	
	public override void OnEnterOnAirUp ()
	{
		float jumpSpeedTemp = jumpSpeed;
		
		moveDir.y = jumpSpeedTemp;
		
		canSecondJump = false;
		
		PlayAnim("JumpStart");
	} 
	
	public override void DoUpdateOnAirUp ()
	{
		moveDir.y -= gravity * Time.deltaTime;
		
		if(axisV != 0 || axisH != 0){
			Vector3 moveHoriz = transform.forward * axisV * speed + transform.right * axisH * speed;
			moveDir.x = moveHoriz.x;
			moveDir.z = moveHoriz.z;
		}
		
		if(moveDir.y > 0){
			cc.Move(moveDir * Time.deltaTime);
		}else{
			updataState(new IActorAction(EFSMAction.HERO_ONAIR_DOWN));
		}
		
//		if(canSecondJump && btnA > 0){
//			updataState(new IActorAction(EFSMAction.HERO_ONAIR_UP));
//		}
	}

	
	public bool IsHitSomeThing(){
		return false;
	}
	
	public GameObject GetCurBGGameObject(){
		GameObject gobjR = null;
		RaycastHit[] hits;
		Vector3 posOri = gameView.main_camera.transform.position;
		Vector3 direction  = Vector3.forward;
		hits = Physics.RaycastAll(posOri, direction, 100.0f);
		if(hits.Length == 2 && hits[1].transform.name.Equals("hero")){
			gobjR = hits[0].transform.gameObject;
		}else{
//			Debug.LogError("hits.Length != 2" + hits.Length);
		}
		return gobjR;
	}
	
	public GameObject GetSpetOnGameObject(){
		return g_gobjCurStepOn;
	}
	#region Game Mehtods
	void InteractCur(){
		if(CurInteract != null){
			CurInteract.OnInteract(gameObject);
		}	
	}	
	#endregion
	
	void OnControllerColliderHit(ControllerColliderHit hit) {

	}
	
	void Killed(){
		
	}
	
	void OnTriggerEnter(Collider other) {
		if(other.CompareTag("Trigger")){
			ITrigger it = other.GetComponent<ITrigger>();
			if(it != null){
				it.OnTrigger(gameObject);
			}
		}else if(other.CompareTag("Interactive")){
			CurInteract = other.GetComponent<IInteractive>();
		}
	}
	
	void OnTriggerExit(Collider other){
		if(other.CompareTag("Interactive")){
			CurInteract = null;
		}
	}
	
	public void PlayAnim(string animName){
		if(anim != null){
			anim.CrossFade(animName);
		}
	}
}
