using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class AreaColorStack : IComparable<AreaColorStack>
{
    public int InstanceId { get; set; }
    public AreaType type { get; set; }

    public int CompareTo(AreaColorStack other)
    {
        if ((int)type > (int)other.type) return 1;
        if ((int)type < (int)other.type) return -1;
        return 0;
    }
}

public class Area : MonoBehaviour
{
    private List<AreaColorStack> m_ColorStack = new List<AreaColorStack>();
    Renderer m_Rend;
    public AreaType type = AreaType.Empty;
    // public AreaType prevType = AreaType.None;

    public void PushArea(AreaType t, int id)
    {
        m_ColorStack.Add(new AreaColorStack() {InstanceId = id, type = t});
        m_ColorStack.Sort((a, b) => b.CompareTo(a));
    }

    public void PopArea(int id)
    {
        var target = m_ColorStack.Find((elem) => elem.InstanceId == id);
        if (target == null) return;
        m_ColorStack.Remove(target);
        m_ColorStack.Sort((a, b) => b.CompareTo(a));
    }

    void Awake()
    {
        m_Rend = GetComponent<Renderer>();
        m_Rend.material = GetMaterial(AreaType.Empty);
    }

    private void Update()
    {
        if (m_ColorStack.Count < 1)
        {
            type = AreaType.Empty;
            m_Rend.material = GetMaterial(type);
            return;
        }
        if (type == m_ColorStack[0].type) return;
        type = m_ColorStack[0].type;
        m_Rend.material = GetMaterial(type);
    }

    private Material GetMaterial(AreaType t)
    {
        Material result = null;
        switch (t)
        {
            case AreaType.Empty :
                result = Resources.Load("Materials/Grass") as Material;
                break;
            case AreaType.NoSpawnArea :
                result = Resources.Load("Materials/Area") as Material;
                break;
            case AreaType.Building :
                result = Resources.Load("Materials/Dirt") as Material;
                break;
            case AreaType.Collision :
                result = Resources.Load("Materials/UnBuildable") as Material;
                break;
            case AreaType.UnCollision :
                result = Resources.Load("Materials/Buildable") as Material;
                break;
        }

        return result;
    }

    // IEnumerator LerpMaterial(AreaType originType, AreaType changeType)
    // {
    //     if (Enum.IsDefined(typeof(AreaType), originType) == false ||
    //         Enum.IsDefined(typeof(AreaType), changeType) == false) yield break;
    //     
    //     Material origin = GetMaterial(originType);
    //     Material change = GetMaterial(changeType);
    //     
    //     float startTime = Time.time;
    //
    //     while(true) {
    //         yield return null;
    //         float lerp = Mathf.Lerp(0, 1, Time.time - startTime);
    //         m_Rend.material.Lerp(origin, change, lerp);
    //         if (lerp >= 1)
    //         {
    //             m_ColorLerpCol = null;
    //             type = changeType;
    //             yield break;
    //         }
    //     }
    // }
    
}
