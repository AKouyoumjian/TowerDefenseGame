using Unity.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class EnemyAI : MonoBehaviour
{
    public enum EnemyState { Navigate, Attack, Die }

    [Header("General Settings")]
    public EnemyState currentState = EnemyState.Navigate;
    public Transform targetBase;
    public int baseDamageAmount = 10;

    [Header("Navigation Settings")]
    public Transform turret;
    public float rotationSpeed = 30f;
    public float detectionRange = 10f;

    [Header("Attack Settings")]
    public bool canAttack = true;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 2f; // shots per second

    [Header("Die Settings")]
    public int health = 100;
    public GameObject destroyEffectPrefab;
    public Slider healthSlider;
    public int deathReward = 2; // renaming to more intuitive deathReward


    NavMeshAgent agent;
    float fireCooldown = 0;
    bool isEnemyDead = false;
    int maxHealth;
    Transform attackTarget;
    Quaternion initialTurretRotation;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (!targetBase)
        {
            targetBase = GameObject.FindGameObjectWithTag("Target").transform;

            if (!targetBase)
            {
                Debug.LogError("No target base set or found by Target tag!");
                return;
            }
        }

        agent.SetDestination(targetBase.position);
        // agent.destination = targetBase.position; 

        if (turret)
        {
            initialTurretRotation = turret.localRotation;
        }

        maxHealth = health;
        // set health slider
        if (healthSlider)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Navigate:
                Navigate();
                break;
            case EnemyState.Attack:
                if (canAttack)
                {
                    Attack();
                }
                else
                {
                    // if can't attack, switch back to navigate
                    currentState = EnemyState.Navigate;
                }
                break;
            case EnemyState.Die:
                Die();
                break;
        }
    }

    void Navigate()
    {
        // redundant (done in Start()) but just for safety
        // agent.SetDestination(targetBase.position);

        if (canAttack)
        {
            FindNearestTower();
        }

        // rotate back to original rotation when navigating
        if (turret)
        {
            turret.localRotation = Quaternion.Slerp(turret.localRotation, initialTurretRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void Attack()
    {
        if (!attackTarget || Vector3.Distance(transform.position, attackTarget.position) > detectionRange)
        {
            // lose target and reset to patrol
            attackTarget = null;
            currentState = EnemyState.Navigate;
            return;
        }

        // look at target with slerp instead of LookAt for smoother rotation
        if (turret)
        {
            Vector3 direction = attackTarget.position - turret.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            turret.rotation = Quaternion.Slerp(turret.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }

        // check cooldown to see if we can shoot
        if (fireCooldown <= 0)
        {
            if (HasLineOfSight(attackTarget))
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
        // only want to execute Die once, so if already set to true then return
        if (isEnemyDead)
        {
            return;
        }
        Debug.Log("Enemy died...");
        agent.isStopped = true;



        if (destroyEffectPrefab)
        {
            Instantiate(destroyEffectPrefab, transform.position, transform.rotation);
        }
        isEnemyDead = true;

        // give reward
        MoneyManager.Instance.GetMoney(deathReward);

        // Destroy(gameObject, 1);
        // destroy immediately
        Destroy(gameObject);
    }


    void FindNearestTower()
    {
        // reusing mostly for TowerAI.cs LookForEnemies()
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
        Transform nearestTower = null;
        float shortestDistance = Mathf.Infinity;

        // go through each collision to get nearest enemy to attack
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Tower"))
            {
                float distanceToTower = Vector3.Distance(transform.position, collider.transform.position);
                if (distanceToTower < shortestDistance)
                {
                    shortestDistance = distanceToTower;
                    nearestTower = collider.transform;
                }
            }
        }

        // set target and switch to attack state
        if (nearestTower)
        {
            attackTarget = nearestTower;
            Debug.Log("Tower detected: " + attackTarget.name);
            currentState = EnemyState.Attack;
            return; // return so rotate line below this function call isn't executed
        }
    }

    // from TowerAI.cs Shoot() method
    void Shoot()
    {
        if (!canAttack)
        {
            return;
        }

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
            var targetTowerTurret = attackTarget.transform.Find("Turret").transform;

            if (targetTowerTurret)
            {
                bulletBehavior.SetTarget(targetTowerTurret);
            }
            else
            {
                bulletBehavior.SetTarget(attackTarget);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            currentState = EnemyState.Die;
            health = 0;
        }

        if (healthSlider)
        {
            healthSlider.value = health;
        }
    }

    // Is there a clear line of sight to given target (tagged Tower)
    bool HasLineOfSight(Transform target)
    {
        RaycastHit hit;
        Vector3 direction = (target.position - firePoint.position).normalized;

        if (Physics.Raycast(firePoint.position, direction, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Tower"))
            {
                Debug.Log("Tower is in sight: " + hit.collider.name);
                return true;
            }
        }

        // if not we return false
        return false;
    }

    public int GetEnemyDamageValue()
    {
        return baseDamageAmount;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            BulletBehavior bulletBehavior = collision.gameObject.GetComponent<BulletBehavior>();
            if (bulletBehavior)
            {
                int damage = bulletBehavior.GetDamageValue();
                TakeDamage(damage);
                Debug.Log("Enemy took damage: " + damage);
            }
            Debug.LogWarning("Enemy hit, no bullet damage attached to bulletBehavior");
        }
    }


    // Draw range gizmo when object is selected
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

}
