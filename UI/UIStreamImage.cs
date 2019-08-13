using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UIStreamImage : MonoBehaviour
{
    public string path;

    void Start()
    {
        var img = GetComponent<RawImage>();
        var uri = Path.Combine(Application.streamingAssetsPath, path);
        var bytes = SDKAdapter.GetInstance().LoadAsset(uri);
        if (bytes != null)
        {
            var tex = new Texture2D(2, 2, TextureFormat.RGB24, false);
            tex.LoadImage(bytes);
            img.texture = tex;
            img.SetNativeSize();
        }
    }
}