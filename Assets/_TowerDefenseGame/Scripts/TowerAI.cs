using System;
using Unity.VisualScripting;
using UnityEngine;

public class TowerAI : MonoBehaviour
{
    public enum TowerState { Patrol, Attack, Die }

    [Header("General Settings")]
    public TowerState currentState = TowerState.Patrol;
    public GameObject buildEffectPrefab;

    [Header("Patrol Settings")]
    public Transform turret;
    public float rotationSpeed = 30f;
    public float maxRotationAngle = 90f;
    public float detectionRange = 10f;

    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 2f; // shots per second

    [Header("Die Settings")]
    public int health = 100;
    public GameObject destroyEffectPrefab;


    Transform target;
    float fireCooldown = 0;
    bool isTowerDead = false;

    void Start()
    {
        if (buildEffectPrefab)
        {
            Instantiate(buildEffectPrefab, transform.position, transform.rotation);
        }

        // TakeDamage(100); // for testing die state
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case TowerState.Patrol:
                Patrol();
                break;
            case TowerState.Attack:
                Attack();
                break;
            case TowerState.Die:
                Die();
                break;
        }
    }

    void Patrol()
    {
        // Debug.Log("Patrolling...");

        if (turret)
        {
            // turret.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            // rotate back and forth between maxRotationAngle
            float angle = Mathf.PingPong(Time.time * rotationSpeed, maxRotationAngle * 2) - maxRotationAngle;
            turret.localRotation = Quaternion.Euler(0, angle, 0);
        }

        // detect enemies
        LookForEnemies();
    }

    void Attack()
    {
        // Debug.Log("Attacking...");

        if (!target || Vector3.Distance(transform.position, target.position) > detectionRange)
        {
            // lose target and reset to patrol
            target = null;
            currentState = TowerState.Patrol;
            return;
        }

        // look at target with slerp instead of LookAt for smoother rotation
        Vector3 direction = target.position - turret.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        turret.rotation = Quaternion.Slerp(turret.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        // check cooldown to see if we can shoot
        if (fireCooldown <= 0)
        {
            if (HasLineOfSight(target))
            {
                Shoot();
            }

            // reset cooldown
            fireCooldown = 1f / fireRate;
        }
        else
        {
            fireCooldown -= Time.deltaTime;
        }
    }

    void Die()
    {
        // Debug.Log("TowerDead...");

        // only want to execute Die once, so if already set to true then return
        if (isTowerDead)
        {
            return;
        }

        Destroy(gameObject, 1);
        if (destroyEffectPrefab)
        {
            Instantiate(destroyEffectPrefab, transform.position, transform.rotation);
        }
        isTowerDead = true;
    }

    void LookForEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
        Transform nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        // go through each collision to get nearest enemy to attack
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                float distanceToEnemy = Vector3.Distance(transform.position, collider.transform.position);
                if (distanceToEnemy < shortestDistance)
                {
                    shortestDistance = distanceToEnemy;
                    nearestEnemy = collider.transform;
                }
            }
        }

        if (nearestEnemy)
        {
            target = nearestEnemy;
            Debug.Log("Target detected: " + target.name);
            currentState = TowerState.Attack;
        }
    }

    void Shoot()
    {
        if (!projectilePrefab || !firePoint)
        {
            Debug.LogWarning("Projectile prefab or fire point not assigned.");
            return;
        }

        var bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // get BulletBehavior and set target
        BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
        if (bulletBehavior)
        {
            bulletBehavior.SetTarget(target);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            currentState = TowerState.Die;
        }
    }

    // Is there a clear line of sight to given target (tagged enemy)
    bool HasLineOfSight(Transform target)
    {
        RaycastHit hit;
        Vector3 direction = (target.position - firePoint.position).normalized;

        if (Physics.Raycast(firePoint.position, direction, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Enemy is in sight: " + hit.collider.name);
                return true;
            }
        }

        // if not we return false
        return false;
    }

    void OnCollisionEnter(Collision collision)
    {
        // only EnemyBullet can damage, not "Bullet"
        if (collision.collider.CompareTag("EnemyBullet"))
        {
            BulletBehavior bulletBehavior = collision.gameObject.GetComponent<BulletBehavior>();
            if (bulletBehavior)
            {
                int damage = bulletBehavior.GetDamageValue();
                TakeDamage(damage);
                Debug.Log("Tower took damage: " + damage);
            }
            Debug.LogWarning("Tower hit, no bullet damage attached to bulletBehavior");
        }
    }

    // Draw range gizmo when object is selected
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // take into account firepoint's positon when drawing line of sign line
        Vector3 lineEndVector = firePoint.position + (firePoint.forward * detectionRange);
        Debug.DrawLine(transform.position, lineEndVector, Color.green);
    }

    void OnDrawGizmos()
    {
        // take into account firepoint's positon when drawing line of sign line
        Vector3 lineEndVector = firePoint.position + (firePoint.forward * detectionRange);
        Debug.DrawLine(transform.position, lineEndVector, Color.green);
    }


}
