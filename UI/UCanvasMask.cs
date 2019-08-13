using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class UCanvasMask : Graphic, ICanvasRaycastFilter
{
    protected override void Awake()
    {
        base.Awake();
        color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        toFill.Clear();
    }

    public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        return true;
    }
}