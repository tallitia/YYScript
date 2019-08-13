using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UILanguageTexture : MonoBehaviour
{
    public string folder;
    public string name;

    private RawImage m_Image;
    void Start()
    {
        m_Image = GetComponent<RawImage>();
        if (m_Image != null && folder != null && name != null)
        {
            var asset = MainGame.GetManager<AssetManager>();
            var lang = MainGame.GetManager<LangManager>();
            var path = "Textures/" + folder + "/" + name + "_" + lang.Current() + ".png";
            m_Image.texture = asset.LoadTexture(path);
            m_Image.SetNativeSize();
        }
    }
}
