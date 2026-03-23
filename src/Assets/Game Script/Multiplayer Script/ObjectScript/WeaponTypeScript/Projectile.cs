using UnityEngine;
using Photon.Pun;

public class Projectile : MonoBehaviour
{
    public float damageAmount = 20f;
    public float lifeTime = 3f;
    public LayerMask ignoredLayers;
    private float currentLifeTime;

    private void OnEnable()
    {
        currentLifeTime = lifeTime;
    }
    private void Update()
    {
        CountDownLifeTime();
    }

    private void CountDownLifeTime()
    {
        currentLifeTime -= Time.deltaTime;
        if(currentLifeTime <= 0)
        {
            DestroyProjectile();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object is in the ignored layers
        if ((ignoredLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            return; // Ignore the collision
        }

        // Debug log to check which GameObject was hit and where
        Debug.Log($"[Projectile] Hit! GameObject: {other.gameObject.name} at Position: {transform.position}");

        // Check if the other object has an IDamageable interface
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            // Apply damage
            damageable.TakeDamage(damageAmount, this .gameObject);

            // Destroy the projectile after hitting something
            DestroyProjectile();
        }
    }

    private void DestroyProjectile()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonView pv = GetComponent<PhotonView>();
            // Check if it's a networked object and we are the owner
            if (pv != null && pv.IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            // If it's not a networked object, just destroy it locally
            else if (pv == null)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
