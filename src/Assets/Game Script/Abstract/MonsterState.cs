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
    [SerializeField] public SignalDefinition hurtSignal;


    private EnemyState currentState = EnemyState.ChasePayload;
    private MonsterBase monsterBase;
    public EnemyState CurrentState { get { return currentState; } }
    public Blackboard FSMblackboard { get { return fsmBlackboard; } }
    public FSMOwner FSMOwner { get { return finiteStateMachine; } }

    private void Start()
    {
        monsterBase = GetComponent<MonsterBase>();
        monsterBase.OnMonsterDie += () =>
        {
            finiteStateMachine.StopBehaviour();
        };
        ResetOnSpawn();
    }
    
    public override void OnEnable()
    {
        ResetOnSpawn();
    }
    public override void OnDisable()
    {
        finiteStateMachine.behaviour.onStateEnter -= CallUpdateStateFunc_FSM;

    }
    private void ResetOnSpawn()
    {
        if (fsmBlackboard != null)
        {
            // ล้างข้อมูลเก่าเพื่อเริ่มรอบใหม่
            fsmBlackboard.SetVariableValue("FirstSeenPlayer", null);
            fsmBlackboard.SetVariableValue("TargetObject", null);
            fsmBlackboard.SetVariableValue("PlayerInVision", null);
            fsmBlackboard.SetVariableValue("TargetPOS", Vector3.zero);
        }
        MonsterBase mBase = GetComponent<MonsterBase>();
        mBase.OnDefaultSetData();
        finiteStateMachine.behaviour.onStateEnter += CallUpdateStateFunc_FSM;
        finiteStateMachine.StartBehaviour();
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
        //finiteStateMachine.graph.UpdateGraph();
        stateOverheadsUI.UpdateStateText(finiteStateMachine.behaviour.currentStateName);
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

