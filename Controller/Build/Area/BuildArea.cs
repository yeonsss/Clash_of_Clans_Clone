using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static Define;

public class BuildArea : MonoBehaviour
{
    public Material m_BuildPossible;
    public Material m_UnBuildPossible;
    public MeshRenderer m_Render;
    public bool isCollision = false;
    private Collider[] _colliders;
    private int _maxColliders = 3;

    public void Awake()
    {
        m_Render = GetComponent<MeshRenderer>();
        m_BuildPossible = Resources.Load("Materials/Buildable") as Material;
        m_UnBuildPossible = Resources.Load("Materials/UnBuildable") as Material;
    }

    private void Update()
    {
        //나의 현재 좌표를 가져와서 게임 매니저에 이미 있는지 체크
        CollisionCheck();
    }

    public void CollisionCheck()
    {
        var myPos = transform.position;
        Wtor(myPos.x, myPos.z, (int)WIDTH, out var rx, out var ry);

        var ar = SpawnManager.GetArea(rx, ry);
        if (ar == null) return;
        
        if (ar.type == AreaType.Building)
        {
            isCollision = true;
            m_Render.material = m_UnBuildPossible;
        }
        else
        {
            isCollision = false;
            m_Render.material = m_BuildPossible;
        }
    }
    
    public void EditModeInit()
    {
        _colliders = new Collider[_maxColliders];
    }
    
    public void CollisionCheckForEditMode()
    {
        // 충돌 체크
        // var boxSize = new Vector3(0.8f, 1, 0.8f);
        var myPos = transform.position;
        var layerMask = LayerMask.GetMask("DetectArea");
        // 바닥 충돌은 제외

        var size = Physics.OverlapSphereNonAlloc(myPos, 0.01f, _colliders, layerMask);
        if (size == 1)
        {
            isCollision = false;
            m_Render.material = m_BuildPossible;
        }
        else if (size > 1)
        {
            isCollision = true;
            m_Render.material = m_UnBuildPossible;
        }
    }
}
