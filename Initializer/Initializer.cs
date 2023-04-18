
using System;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    public virtual void Init() { }

    protected void Awake()
    {
        Init();
    }

    protected virtual void Start()
    {
        
    }
}
