using System;
using System.Collections;
using UnityEngine;

public class HowitzerController : MonoBehaviour
{
    public Transform target;
    public Transform attacker;
    private float _attackPoint;
    private float _speed;
    private float _totalDistance;

    public void SetInfo(Transform target, Transform attacker, float attackPoint, float speed)
    {
        this.target = target;
        this.attacker = attacker;
        _attackPoint = attackPoint;
        _speed = speed;
    }
    
    private void Start()
    {
        if (target != null && attacker != null)
        {
            transform.position = attacker.position; 
            _totalDistance = Vector3.Distance(target.position, attacker.position);
        }
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        // 포물선 이동
        if (target == null) Destroy(gameObject);
        try
        {
            var currentDistance = _totalDistance - Vector3.Distance(transform.position, target.position);
            float theta = currentDistance * (Mathf.PI / _totalDistance);
            var y = Mathf.Sin(theta) * 1.5f;

            var movePos = Vector3.MoveTowards(
                transform.position, target.position, _speed * Time.deltaTime);

            movePos.y = y;
            transform.position = movePos;
            OnCollisionByDistance(target.gameObject);
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
    
}