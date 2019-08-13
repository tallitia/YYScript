using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UILanguageImage : MonoBehaviour
{
    public string module;
    public string sprite;

    private Image m_Image;
    void Start()
    {
        m_Image = GetComponent<Image>();
        if (m_Image != null && module != null && sprite != null)
        {
            var asset = MainGame.GetManager<AssetManager>();
            var lang = MainGame.GetManager<LangManager>();
            var path = "Sprites/" + module + "/" + sprite + "_" + lang.Current() + ".png";
            m_Image.sprite = asset.LoadSprite(path);
            m_Image.SetNativeSize();
        }
    }
}
