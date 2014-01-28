using UnityEngine;
using System.Collections;

public class IActor : MonoBehaviour{
	public int id;
	public EActorType actor_type;
	public bool isEnermy;
	public IActorState state;
	public IActorAction action;
	
    public void updataState(IActorAction action) {
        if(action.actiontype != EFSMAction.NONE){
//            Debug.Log("updataState - " + this.state + " by action:" + action.actiontype);//########
            IActorState asCur = this.state;
            IActorState asNext = asCur.toNextState(action.actiontype);
            if (asNext != null)
            {
                this.state = asNext;
				this.action = action;
                this.state.OnEnter();
            }
        }
    }

    public bool IsInState(System.Type type) {
        return state.GetType() == type;
    }
	
	public virtual void DoUpdateAttack() {
       
    }

    public virtual void DoUpdateIdle() { }
	
	public virtual void DoUpdateMove(){}
	
	
	public virtual void DoUpdateRun(){}
	
	public virtual void OnEnterRun(){}
	
	public virtual void OnEnterAttack(){}
	
	public virtual void OnEnterUnAttack(){}
	
	public virtual void OnEnterIdle(){}
	
	public virtual void OnEnterMove(){}
	
	public virtual void OnEnterHeroFlash(){}
	
	public virtual void DoUpdateHeroFlash(){}
	
	public virtual void OnEnterHeroFlashAttack(){}
	
	public virtual void OnEnterUnAttack_By_Flash(){}
	
	public virtual void OnEnterOnAirDown(){}
	public virtual void DoUpdateOnAirDown(){}
	
	public virtual void OnEnterOnAirUp(){}
	public virtual void DoUpdateOnAirUp(){}
	
	public virtual void OnEnterDie(){}
	public virtual void DoUpdateDie(){}
}
