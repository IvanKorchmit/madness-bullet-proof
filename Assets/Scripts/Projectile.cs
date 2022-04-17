using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Entity owner;
    private enum TrajectoryType
    {
        Straight, Sine
    }
    [SerializeField] private TrajectoryType trajectoryType;
    [SerializeField] private float speed;
    [SerializeField] private float amplitude;
    [Min(1f)]
    [SerializeField] private float frequency;
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
    }
    public Projectile Init(Entity owner)
    {
        this.owner = owner;
        return this;
    }
    private void FixedUpdate()
    {
        if (trajectoryType == TrajectoryType.Sine)
        {
            pos += transform.right * Time.deltaTime * speed;
            rb.MovePosition(pos + axis * Mathf.Sin(time * frequency) * amplitude);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        bool ignore = (owner as Enemy) != null && collision.GetComponent<Entity>() is Enemy;


        if (!ignore && owner != null && owner.gameObject != collision.gameObject && collision.TryGetComponent(out IDamagable damage))
        {
            damage.Damage(owner, 1);
            Destroy(gameObject);
        }
    }
    private void Update()
    {
        time += Time.deltaTime;
    }
}
