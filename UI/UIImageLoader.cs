using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class UIImageLoader : MonoBehaviour
{
    [SerializeField]protected string m_Source;
    public string source { get { return m_Source; } set { m_Source = value; SetDirty(); } }

    private AssetManager m_Asset;
    //private GameManager m_Game;

    private RawImage m_Image;
    private bool m_IsStart = false;
    private bool m_IsDirty = false;
    private string m_EventName;
    private void Start()
    {
        m_Asset = LuaHelper.GetAssetManager();
        //m_Game = LuaHelper.GetGameManager();

        m_Image = GetComponent<RawImage>();
        m_IsStart = true;

        if (m_IsDirty) Rebuild();
    }

    private void OnDestroy()
    {
        //if (m_Game != null && m_EventName != null)
        //{
        //    m_Game.RemovePatchEvent(m_EventName, OnLoadComplete);
        //    m_EventName = null;
        //}
    }

    private void Rebuild()
    {
        m_IsDirty = false;

        if (m_Asset == null || m_Source == null)
            return;

        Texture tex = m_Asset.LoadAsset<Texture>(m_Source);
        if (tex == null)
        {
            string assetname = System.IO.Path.GetFileNameWithoutExtension(m_Source);
            tex = m_Asset.LoadAsset<Texture>(AppConst.PatchAsset, assetname);

            if (m_EventName != null)
                //m_Game.RemovePatchEvent(m_EventName, OnLoadComplete);

            m_EventName = m_Asset.GetAssetBundleName(m_Source);
            //m_Game.AddPatchEvent(m_EventName, OnLoadComplete);
        }
        m_Image.texture = tex;
    }

    private void OnLoadComplete()
    {
        Texture tex = m_Asset.LoadAsset<Texture>(m_Source);
        m_Image.texture = tex;

        //m_Game.RemovePatchEvent(m_EventName, OnLoadComplete);
        m_EventName = null;
    }

    public void SetDirty()
    {
        m_IsDirty = true;
        if (m_IsStart) Rebuild();
    }
}