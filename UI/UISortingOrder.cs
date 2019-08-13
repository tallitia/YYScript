using UnityEngine;

public class UISortingOrder : MonoBehaviour
{
    public int orderInParent = 1;

    public void Reset()
    {
        var canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            var parent = transform.parent.GetComponentInParent<Canvas>();
            if (parent != null)
            {
                canvas.overrideSorting = true;
                canvas.sortingOrder = parent.sortingOrder + orderInParent;
            }
            return;
        }

        var renders = GetComponentsInChildren<Renderer>();
        if (renders.Length > 0)
        {
            var parent = transform.parent.GetComponentInParent<Canvas>();
            if (parent != null)
            {
                for (int i = 0; i < renders.Length; i++)
                {
                    renders[i].sortingOrder = parent.sortingOrder + orderInParent;
                }
            }
        }
    }

    [ContextMenu("Execute")]
    void Start()
    {
        Reset();
    }
}
