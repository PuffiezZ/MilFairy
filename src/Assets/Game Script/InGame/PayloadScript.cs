using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Splines;

public class PayloadScript : MonoBehaviourPun
{
    [Header("Payload Setting")]
    public float SphereAreaRadius = 3f;
    public float MoveSpeedTarget = 5f;
    public float AccelerationTime = 2f;
    public SplineAnimate splineAnimate;

    private Coroutine speedCoroutine;
    private float distancePercentage = 0f;
    private float splineLenght = 0f;
    private bool isTurnOn = false;
    private float currentMoveSpeed = 0f;
    private Collider[] collisionPlayer;
    public void PayloadOnSetup(SplineContainer sc)
    {
        splineAnimate.Container = sc;
        splineAnimate.MaxSpeed = 0;
        splineLenght = splineAnimate.Container.CalculateLength();
    }
    private void Update()
    {
        // ในระบบ Multiplayer เรามักจะให้ Master Client เป็นคนตัดสินใจเรื่อง "เงื่อนไขการวิ่ง"
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                HandlePayloadLogic();
            }
        }
        else
        {
            HandlePayloadLogic();
        }

        // ทุกเครื่อง (รวมถึง Client) จะต้องรันการขยับตำแหน่งตามความเร็วปัจจุบันเพื่อให้ภาพนุ่มนวล
        if (currentMoveSpeed > 0)
        {
            PayloadPositionHandler();
        }
    }
    private void HandlePayloadLogic()
    {
        bool hasPlayer = CheckPlayerNearby();

        float target = (hasPlayer && isTurnOn) ? MoveSpeedTarget : 0f;
        float newSpeed = Mathf.MoveTowards(currentMoveSpeed, target, Time.deltaTime * AccelerationTime);

        if (!Mathf.Approximately(currentMoveSpeed, newSpeed))
        {
            if (PhotonNetwork.InRoom)
            {
                // ส่งค่าความเร็วไปให้ทุกคนซิงค์ตาม
                photonView.RPC(nameof(RPC_SyncPayloadSpeed), RpcTarget.All, newSpeed);
            }
            else
            {
                currentMoveSpeed = newSpeed;
            }
        }
    }
    [PunRPC]
    private void RPC_SyncPayloadSpeed(float speed)
    {
        // ทุกเครื่องจะได้รับความเร็วเท่ากัน และนำไปรันใน PayloadPositionHandler() ต่อไป
        currentMoveSpeed = speed;
    }

    private bool CheckPlayerNearby()
    {
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, SphereAreaRadius, LayerMask.GetMask("Player"));
        return hitPlayers.Length > 0;
    }
    private void PayloadPositionHandler()
    {
        // 1. คำนวณระยะทางสะสม (0-1)
        distancePercentage += (currentMoveSpeed * Time.deltaTime) / splineLenght;
        distancePercentage = Mathf.Clamp01(distancePercentage); // กันค่าเกิน 1

        Vector3 currentPosition = splineAnimate.Container.EvaluatePosition(distancePercentage);
        transform.position = currentPosition;

        Vector3 forwardDirection = splineAnimate.Container.EvaluateTangent(distancePercentage);

        Vector3 upDirection = splineAnimate.Container.EvaluateUpVector(distancePercentage);

        if (forwardDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(forwardDirection, upDirection);
        }
    }
    public void PlayloadSwitchFunction()
    {
        isTurnOn = !isTurnOn;
        Debug.Log("Payload Engine: " + (isTurnOn ? "ON" : "OFF"));
    }

    private void AdjustSpeedEnumerator()
    {
        if (isTurnOn)
        {
            currentMoveSpeed = MoveSpeedTarget;
        }
        else
        {
            currentMoveSpeed = 0f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isTurnOn ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, SphereAreaRadius);
    }
}
