using UnityEngine;

public class UINativeAdjustor : MonoBehaviour
{
    public float scale = 1f;
    void Start()
    {
        var layer = MainGame.GetManager<LayerManager>();
        if (layer != null)
        {
            var adjust = layer.ScreenScale;
            adjust.x = scale / adjust.x;
            adjust.y = scale / adjust.y;
            adjust.z = scale / adjust.z;
            transform.localScale = adjust;
        }
    }
    
    void Reset()
    {
        GameObject root = GameObject.FindGameObjectWithTag("UIRoot");
        if (root != null)
        {
            var adjust = root.transform.localScale;
            adjust.x = scale / adjust.x;
            adjust.y = scale / adjust.y;
            adjust.z = scale / adjust.z;
            transform.localScale = adjust;
        }
    }
}