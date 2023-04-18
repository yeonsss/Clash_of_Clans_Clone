using System;
using UnityEngine;
using UnityEngine.Serialization;

public class BuildFinalChoice : MonoBehaviour
{
    public GameObject obj;
    public int buildCost;
    
    public void Init(GameObject target, int cost)
    {
        obj = target;
        buildCost = cost;
    }
}
