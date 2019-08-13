using UnityEngine;

public class UIScreenAdjustor : MonoBehaviour
{
    void Start () 
    {
        float designAspect = 720f / 1280f;
        var layer = MainGame.GetManager<LayerManager>();
        var size = layer.ScreenSize;
        float screenAspect = size.x / size.y;
        if (screenAspect < designAspect)
        {
            float adjust = designAspect / screenAspect;
            transform.localScale = new Vector3(adjust, adjust, 1f);
        }
        else if (screenAspect > designAspect)
        {
            float adjust = designAspect / screenAspect;
            transform.localScale = new Vector3(adjust, adjust, 1f);
        }
    }
}