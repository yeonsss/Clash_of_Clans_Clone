using System;
using UnityEngine;
using UnityEngine.Serialization;
using static Define;

public class ProjectileController : MonoBehaviour
{
    private Transform _target;
    private Transform _attacker;
    private float _attackPoint = 100f;
    private float _speed = 9.0f;

    private void Update()
    {
        Move();
    }
    
    public void SetInfo(Transform target, Transform attacker, float attackPoint, float speed)
    {
        this._target = target;
        this._attacker = attacker;
        _attackPoint = attackPoint;
        _speed = speed;
    }
    
    private void Move()
    {
        if (_target == null) Destroy(gameObject);
        try
        {
            transform.LookAt(_target);
            transform.position = Vector3.MoveTowards(
                transform.position, _target.position, _speed * Time.deltaTime);

            if (transform.position.x < X_MIN || transform.position.x > X_MAX) Destroy(gameObject);
            if (transform.position.z < Y_MIN || transform.position.z > Y_MAX) Destroy(gameObject);

            OnCollisionByDistance(_target.gameObject);
        }
        catch
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionByDistance(GameObject target)
    {
        if (target == null) return;
        var offset = gameObject.transform.position - target.transform.position;
        var distanceSqr = offset.sqrMagnitude;
        
        if (distanceSqr < 0.3)
        {
            if (target.CompareTag("Building"))
                target.GetComponent<Build>().Attacked(_attackPoint);
            else
                target.GetComponent<MonsterController>().Attacked(_attackPoint);
            
            Destroy(gameObject);
        }
        
    }

    // private void OnParticleCollision(GameObject other)
    // {
    //     if (other == null || other.activeSelf == false)
    //     {
    //         Destroy(gameObject);
    //     }
    //     
    //     if (other.CompareTag("Enemy"))
    //     {
    //         other.gameObject.GetComponent<MonsterController>().Attacked(AttackPoint);
    //         Destroy(gameObject);
    //     }
    // }
}
