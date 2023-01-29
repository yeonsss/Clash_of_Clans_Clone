using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BuildFinalChoice : MonoBehaviour
{
    public GameObject obj;
    
    public void Init(GameObject target)
    {
        obj = target;
    }
}
