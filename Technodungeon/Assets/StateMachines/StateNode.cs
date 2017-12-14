using UnityEngine;
using System.Collections;

public class StateNode : StateMachineBehaviour {

    public string stateName = "EditMe";

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        var entity = animator.gameObject.GetComponent<Entity>();
        if (entity != null)
            entity.StateMachineEvent("Enter", stateName, animator, stateInfo, layerIndex);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        var entity = animator.gameObject.GetComponent<Entity>();
        if (entity != null)
            entity.StateMachineEvent("Update", stateName, animator, stateInfo, layerIndex);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        var entity = animator.gameObject.GetComponent<Entity>();
        if (entity != null)
            entity.StateMachineEvent("Exit", stateName, animator, stateInfo, layerIndex);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        var entity = animator.gameObject.GetComponent<Entity>();
        if (entity != null)
            entity.StateMachineEvent("Move", stateName, animator, stateInfo, layerIndex);
    }

    public string getName() {
        return stateName;
    }
}