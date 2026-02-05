using NaughtyAttributes;
using NodeCanvas.Framework;
using NodeCanvas.StateMachines;
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
    [ShowIf(nameof(stateControllerType), StateControllerType.FSM)]
    [BoxGroup("FSM")]
    [SerializeField] private FSM fsm;


    private EnemyState currentState = EnemyState.ChasePayload;
    private MonsterBase monsterBase;
    public EnemyState CurrentState { get { return currentState; } }
    public Blackboard FSMblackboard { get { return fsmBlackboard; } }

    private void Start()
    {
        monsterBase = GetComponent<MonsterBase>();

        fsm.onStateEnter += CallUpdateStateFunc_FSM;
    }
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

