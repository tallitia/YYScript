using UnityEngine;
using UnityEngine.UI;

public class UILanguageSelector : MonoBehaviour
{
    /// lua方法名称 
    private const string METHOD_NAME = "lang_mgr.get_one";

    /// 文本
    private Text m_text;

    /// 模块
    public string module;

    /// index
    public int index;

    // Use this for initialization
    void Start()
    {
        m_text = GetComponent<Text>();
        if (m_text != null && !string.IsNullOrEmpty(module))
        {
            string ret = Util.InvokeLuaFunction<string, int, string>(METHOD_NAME, module, index);
            if (ret != null) m_text.text = ret;
        }
    }
}
