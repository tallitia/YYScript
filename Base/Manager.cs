using UnityEngine;
using System;

public class Manager : MonoBehaviour, IManager
{
    protected bool isInit = false;

    public virtual void OnInit()
    {
        isInit = true;
    }

    public virtual void OnRestart()
    {
        
    }
}
