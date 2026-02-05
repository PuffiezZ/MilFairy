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
    private FSM fsm;

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
        fsm = GetComponent<FSM>();

        fsm.onStateEnter += CallUpdateStateFunc_FSM;
        if (stateControllerType == StateControllerType.BehaviourTree)
        {
            behaviorTree.StartBehavior();
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
    public void CallUpdateStateFunc_FSM(IState state)
    {
        if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_UpdateStateFSM), RpcTarget.AllBuffered);
        }
        else
        {
            UpdateStateFSM();
        }

    }
    [PunRPC]
    private void RPC_UpdateStateFSM()
    {
        UpdateStateFSM();
    }
    private void UpdateStateFSM()
    {
        //currentState = (EnemyState)newStateNumber;

        //behaviorTree.RestartBehavior();
        finiteStateMachine.graph.UpdateGraph();
        stateOverheadsUI.UpdateStateText(finiteStateMachine.GetCurrentState().FSM.currentStateName);
    }
    public void UpdateFSMVariable<T>(string varName, T value)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Photon RPC รองรับการส่ง object ที่เป็นประเภทพื้นฐานได้เลย
            photonView.RPC(nameof(RPC_SyncFSMVariable), RpcTarget.AllBuffered, name, (object)value);
        }
    }

    [PunRPC]
    private void RPC_SyncFSMVariable(string varName, object value)
    {
        // อัปเดตค่าลงใน Blackboard
        fsmBlackboard.SetVariableValue(varName, value);

        // อัปเดต UI เพื่อให้สอดคล้องกับงานวิจัยของคุณ
        stateOverheadsUI.UpdateStateText(((EnemyState)value).ToString());
    }
    #endregion
}

