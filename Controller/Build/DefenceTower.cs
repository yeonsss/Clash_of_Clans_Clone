using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DefenceTower : Build
{
    private float detectRange = 10f;
    private float time = 3f;
    private Transform target;
    private LayerMask mask;
    private Object missilePrefab;
    private Coroutine C_AttackCor;

    protected override void Awake()
    {
        base.Awake();
        mask = LayerMask.GetMask("Object");
        missilePrefab = Resources.Load("Prefabs/Particle/DefenceTowerMissile");
    }

    protected override void Update()
    {
        if (buildActive == false || m_CooldownComplete == false) return;

        if (C_AttackCor == null)
        {
            C_AttackCor = StartCoroutine("AttackCoroutine");    
        }
    }

    protected bool DetectEnemy()
    {
        if (target != null && target.gameObject.activeSelf == true) return true;
        
        var colliders = Physics.OverlapSphere(transform.position, detectRange, mask);
        foreach (var collider in colliders)
        {
            if (collider.transform == transform) continue;
            if (collider.CompareTag("Enemy") && collider.gameObject.activeSelf == true)
            {
                target = collider.transform;
                return true;
            }
        }
        return false;
    }

    protected void AttackEnemy()
    {
        var obj = Instantiate(missilePrefab, transform.position, Quaternion.identity);
        if (obj == null) return;
        var pc = obj.GetComponent<ProjectileController>();
        pc.Attacker = transform;
        pc.target = target;
        pc.AttackPoint = 10f;
    }
    
    protected override void UseSkill()
    {
    }
    
    IEnumerator AttackCoroutine()
    {
        yield return new WaitForSecondsRealtime(time);
        if (DetectEnemy() != false)
        {
            AttackEnemy();
        }
        C_AttackCor = null;
    }
}
