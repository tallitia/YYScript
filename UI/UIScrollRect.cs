using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScrollRect : ScrollRect {
    
    protected override void SetNormalizedPosition(float value, int axis)
    {
        base.SetNormalizedPosition(value, axis);
    }

    public void SetPosition(float value,int axis)
    {
        SetNormalizedPosition(value, axis);
        base.UpdateBounds();
    }
}
