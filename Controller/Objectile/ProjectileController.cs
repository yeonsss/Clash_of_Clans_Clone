using System;
using UnityEngine;
using static Define;

public class ProjectileController : MonoBehaviour
{
    public Transform target;
    public Transform Attacker;
    public float AttackPoint = 100f;
    public float speed = 9.0f;

    private void Update()
    {
        Move();
    }
    
    private void Move()
    {
        if (target == null) Destroy(gameObject);
        try
        {
            transform.LookAt(target);
            transform.position = Vector3.MoveTowards(
                transform.position, target.position, speed * Time.deltaTime);

            if (transform.position.x < X_MIN || transform.position.x > X_MAX) Destroy(gameObject);
            if (transform.position.z < Y_MIN || transform.position.z > Y_MAX) Destroy(gameObject);
        }
        catch
        {
            Destroy(gameObject);
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.transform.name);
        if (other.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<MonsterController>().Attacked(AttackPoint);
            Destroy(gameObject);
        }
    }
}
