using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private IHitter owner;
    private Vector2 startPosition;
    [SerializeField] private int damage;
    private enum TrajectoryType
    {
        Straight, Sine, Homing
    }
    [SerializeField] private TrajectoryType trajectoryType;

    [Header("Sine options")]
    [SerializeField] private float speed;
    [SerializeField] private float amplitude;
    [Min(1f)]
    [SerializeField] private float frequency;
    [Space(32)]
    [Header("Homing missle optins")]
    [SerializeField] private float turnSpeed;
    [SerializeField] private GameObject explosion;

    private Vector3 pos;
    private Rigidbody2D rb;
    private Vector3 axis;
    private float time;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * speed;
        axis = transform.up;
        pos = rb.position;
        startPosition = transform.position;
    }
    public Projectile Init(IHitter owner)
    {
        this.owner = owner;
        return this;
    }
    private void FixedUpdate()
    {
        if (owner == null)
        {
            if (trajectoryType == TrajectoryType.Homing)
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
            return;
        }
        if (trajectoryType == TrajectoryType.Sine)
        {
            pos += transform.right * Time.deltaTime * speed;
            rb.MovePosition(pos + axis * Mathf.Sin(time * frequency) * amplitude);
        }
        else if (trajectoryType == TrajectoryType.Homing)
        {
            Player target = Player.Singleton;
            if (target != null)
            {
                Vector2 direction = (Vector2)target.transform.position - rb.position;
                direction.Normalize();
                float rotAmount = Vector3.Cross(direction, transform.right).z;
                rb.velocity = transform.right * speed;

                rb.angularVelocity = -rotAmount * turnSpeed;
            }
        }
        if (trajectoryType != TrajectoryType.Homing && Vector2.Distance(startPosition, transform.position) >= 15)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (owner == null || collision == null)
        {
            if (trajectoryType == TrajectoryType.Homing)
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
            return;
        }
        if (collision.CompareTag("Obstacle"))
        {
            if (trajectoryType == TrajectoryType.Homing)
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
            return;
        }

        bool ignore = (owner as Enemy) != null && collision.GetComponent<Entity>() is Enemy;


        if (!ignore && !System.Object.Equals(owner, null) && owner.gameObject != collision.gameObject && collision.TryGetComponent(out IDamagable damage))
        {
            damage.Damage(owner, this.damage);
            if (trajectoryType == TrajectoryType.Homing)
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
            return;
        }
    }
    private void Update()
    {
        time += Time.deltaTime;
    }
}
