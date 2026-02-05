using NaughtyAttributes;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
using Opsive.BehaviorDesigner.Runtime;
using Opsive.BehaviorDesigner.Runtime.Tasks.Events;
using Opsive.GraphDesigner.Runtime;
using Opsive.Shared.Events;
using Photon.Pun;
using UnityEngine;
using static UtilityDev;

public class MonsterState : MonoBehaviourPunCallbacks
{
    [SerializeField] private StateControllerType stateControllerType;
    [Header("State Management")]
    [SerializeField] private StateUIOverhead stateOverheadsUI;

    [ShowIf(nameof(stateControllerType), StateControllerType.FSM)]
    [BoxGroup("FSM")]
    [SerializeField] private FSMOwner finiteStateMachine;
    [ShowIf(nameof(stateControllerType), StateControllerType.FSM)]
    [BoxGroup("FSM")]
    [SerializeField] private Blackboard fsmBlackboard;

    [ShowIf(nameof(stateControllerType), StateControllerType.BehaviourTree)]
    [BoxGroup("Behaviour Tree")]
    [SerializeField] private BehaviorTree behaviorTree;
    [ShowIf(nameof(stateControllerType), StateControllerType.BehaviourTree)]
    [BoxGroup("Behaviour Tree")]
    [SerializeField] private float framerateUpdate = 60;

    private EnemyState currentState = EnemyState.ChasePayload;
    private MonsterBase monsterBase;
    public EnemyState CurrentState { get { return currentState; } }
    public Blackboard FSMblackboard { get { return fsmBlackboard; } }

    private void Start()
    {
        monsterBase = GetComponent<MonsterBase>();
        CallChangeStateFunc(EnemyState.ChasePayload);

        stateOverheadsUI.UpdateStateText(CurrentState.ToString());

        if(stateControllerType == StateControllerType.BehaviourTree)
        {
            behaviorTree.StartBehavior();
        }


    }
    private void Update()
    {
        if(stateControllerType == StateControllerType.BehaviourTree)
        {
            // ตัวอย่าง: สั่งให้ทำงานเฉพาะเฟรมที่ต้องการ เพื่อประหยัด CPU
            if (Time.frameCount % framerateUpdate == 0) // ทำงานทุกๆ 5 เฟรม
            {
                behaviorTree.Tick();
            }
        }

    }
    public void CallEventState(string nameEvent,object arg1 = null)
    {
        EventHandler.ExecuteEvent(behaviorTree, nameEvent);
        Debug.Log($"<color=yellow>[BT Event]</color> Sent: {nameEvent}");
    }
    #region Set Tree State Region
    public void OnInvokeSetTreeStateVariable<T>(string nameVariable, T inputObject)
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_SetTreeStateVariable),RpcTarget.All,nameVariable,inputObject);
        }
        else
        {
            SetTreeStateVariable(nameVariable,inputObject);
        }
    }

    [PunRPC]
    protected void RPC_SetTreeStateVariable<T>(string nameVariable, T inputObject)
    {
        SetTreeStateVariable(nameVariable, inputObject);
    }
    protected void SetTreeStateVariable<T>(string nameVariable, T inputObject)
    {
        var variable = behaviorTree.GetVariable(nameVariable);
        if (variable != null)
        {
            behaviorTree.SetVariableValue(nameVariable, inputObject);
        }
        else
        {
            Debug.LogError($"<color=red>[BT Error]</color> Variable '{nameVariable}' not found in Behavior Tree!");
        }
    }
    #endregion
    #region Change State Region
    public void CallChangeStateFunc(EnemyState newState)
    {
        int stateInt = (int)newState;
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_ChangeState), RpcTarget.AllBuffered, stateInt);
        }
        else
        {
            ChangeState(stateInt);
        }

    }
    [PunRPC]
    private void RPC_ChangeState(int newStateNumber)
    {
        ChangeState(newStateNumber);
    }
    private void ChangeState(int newStateNumber)
    {
        currentState = (EnemyState)newStateNumber;
        stateOverheadsUI.UpdateStateText(CurrentState.ToString());
        //behaviorTree.RestartBehavior();
    }
    #endregion
}

