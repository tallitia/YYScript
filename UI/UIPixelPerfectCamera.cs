using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPixelPerfectCamera : MonoBehaviour
{

    public int ppuScale = 2;
    public int pixelsPerUnit = 32;

    // Use this for initialization
    void Start()
    {
        double orthoSize = 1f / ((2f / Screen.height) * pixelsPerUnit);
        orthoSize = orthoSize / ppuScale;

        Debug.Log("######:" + orthoSize);
        Camera.main.orthographicSize = (float)orthoSize;
    }
}