using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CartStabilizer : MonoBehaviour
{
    [Tooltip("จุดศูนย์ถ่วง (ยิ่งต่ำยิ่งคว่ำยาก)")]
    public Vector3 centerOfMassOffset = new Vector3(0, -0.5f, 0);

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // ย้ายจุดศูนย์ถ่วงลงไปใต้ดินหลอกๆ เพื่อให้ฐานแน่น
        rb.centerOfMass = centerOfMassOffset;
    }

    void OnDrawGizmosSelected()
    {
        if (rb != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + transform.rotation * centerOfMassOffset, 0.1f);
        }
    }
}