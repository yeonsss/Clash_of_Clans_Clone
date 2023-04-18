using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static Define;

public class BuildArea : MonoBehaviour
{
    
    private Material m_BuildPossible;
    private Material m_UnBuildPossible;

    private MeshRenderer m_Render = null;
    public bool isCollision = false;
    // public GameObject building;
    
    private void Awake()
    {
        m_Render = GetComponent<MeshRenderer>();
        m_BuildPossible = Resources.Load("Materials/Buildable") as Material;
        m_UnBuildPossible = Resources.Load("Materials/UnBuildable") as Material;
        // building = transform.parent.parent.gameObject;
    }

    private void Update()
    {
        //나의 현재 좌표를 가져와서 게임 매니저에 이미 있는지 체크
        CollisionCheck();
    }

    private void CollisionCheck()
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
}
