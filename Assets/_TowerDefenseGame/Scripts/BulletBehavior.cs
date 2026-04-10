using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    [Header("Bullet Settings")]
    public float speed = 20f;
    public float rotationSpeed = 5f;
    public float lifetime = 5f;
    public int damage = 10;
    public GameObject bulletHitPrefab;

    private Rigidbody rb;
    private Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        if (!target)
        {
            // destroy and log if no target
            Debug.Log("Bullet has no target, destroying...");
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;

        // smooth rotation
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

        // move forward now that already facing target
        rb.linearVelocity = transform.forward * speed;
    }

    public void SetTarget(Transform currentTarget)
    {
        target = currentTarget;
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bullet hit: " + collision.transform.name);

        if (bulletHitPrefab)
        {
            var pos = collision.contacts[0].point;
            Instantiate(bulletHitPrefab, pos, Quaternion.identity);
        }

    }

    public int GetDamageValue()
    {
        return damage;
    }
}
