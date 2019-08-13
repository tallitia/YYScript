using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIDepth : MonoBehaviour {
    public int order=2;
    public bool isUI = true;
    void Start()
    {
        if (isUI)
        {
            //canvas
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
            }
            canvas.overrideSorting = true;
            canvas.sortingOrder = order;

            //graphic raycaster
            GraphicRaycaster gr = GetComponent<GraphicRaycaster>();
            if (gr == null)
            {
                gr = gameObject.AddComponent<GraphicRaycaster>();
            }
        }
        else
        {
            Renderer[] renders = GetComponentsInChildren<Renderer>();

            foreach (Renderer render in renders)
            {
                render.sortingOrder = order;
            }
        }
    }
    void OnEnable()
    {
        if (isUI)
        {
            //canvas
            Canvas canvas = GetComponent<Canvas>();
            if (canvas == null)
            {
                canvas = gameObject.AddComponent<Canvas>();
            }
            canvas.overrideSorting = true;
            canvas.sortingOrder = order;

            //graphic raycaster
            GraphicRaycaster gr = GetComponent<GraphicRaycaster>();
            if (gr == null)
            {
                gr = gameObject.AddComponent<GraphicRaycaster>();
            }
        }
        else
        {
            Renderer[] renders = GetComponentsInChildren<Renderer>();

            foreach (Renderer render in renders)
            {
                render.sortingOrder = order;
            }
        }
    }

}
