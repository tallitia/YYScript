using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

[ExecuteInEditMode]
[RequireComponent(typeof(RawImage))]
public class UIImageAnimation : MonoBehaviour
{
    [SerializeField]protected Texture m_Texture;
    public Texture texture { get { return m_Texture; } set { m_Texture = value; RebuildTexture(); } }

    [SerializeField]protected int m_HCount = 1;
    public int hCount { get { return m_HCount; } set { m_HCount = value; RebuildAll(); } }

    [SerializeField]protected int m_VCount = 1;
    public int vCount { get { return m_VCount; } set { m_VCount = value; RebuildAll(); } }

    [SerializeField]protected int m_FPS = 8;
    public int fps { get { return m_FPS; } set { m_FPS = value; RebuildData(); } }

    [SerializeField]protected int m_Direction = 0;
    public int dir { get { return m_Direction; } set { m_Direction = value; RebuildRect(); } }

    [SerializeField]protected int m_Loop = -1;
    public int times { get { return m_Loop; } set { m_Loop = value; } }

    [SerializeField]protected bool m_IsPlay = true;
    public bool isPlay { get { return m_IsPlay; } set { m_IsPlay = value; } }

    public Action onComplete;

    protected float m_DeltaTime = 0f;
    protected float m_HStep = 0f;
    protected float m_VStep = 0f;
    protected float m_FStep = 0f;
    protected int m_Index = 0;

    protected RawImage m_Image;
    protected Rect m_UVRect;

    void Start()
    {
        RebuildAll();
    }

#if UNITY_EDITOR
    void OnEnable()
    {
        RebuildAll();
    }
#endif

    void Update()
    {
        if (m_Texture == null)
            return;

        if (m_FPS <= 0)
            return;

        if (!m_IsPlay)
            return;

        m_DeltaTime += Time.deltaTime;
        if (m_DeltaTime >= m_FStep) //next frame
        {
            m_DeltaTime -= m_FStep;

            if (++m_Index >= m_HCount) //next loop
            {
                if (m_Loop > 0)
                    m_Loop -= 1;

                if (m_Loop == 0)
                    m_IsPlay = false;
                else
                    m_Index = 0;
            }

            if (m_IsPlay) ChangeFrame(m_Index);
            else if (onComplete != null) onComplete();
        }
    }

    protected void ChangeFrame(int index)
    {
        m_UVRect.x = m_HStep * index;
        m_Image.uvRect = m_UVRect;
    }

    public void RebuildData()
    {
        m_HStep = 1f / m_HCount;
        m_VStep = 1f / m_VCount;
        m_FStep = 1f / m_FPS;
    }

    public void RebuildRect()
    {
        if (m_Image == null)
        {
            m_Image = GetComponent<RawImage>();
            m_UVRect = m_Image.uvRect;
        }
        m_Index = 0;
        m_DeltaTime = 0f;
        m_UVRect.width = m_HStep;
        m_UVRect.height = m_VStep;
        m_UVRect.x = m_HStep * m_Index;
        m_UVRect.y = m_VStep * m_Direction;
        m_Image.uvRect = m_UVRect;
    }

    public void RebuildTexture()
    {
        if (m_Image == null)
        {
            m_Image = GetComponent<RawImage>();
            m_UVRect = m_Image.uvRect;
        }

        m_Image.texture = m_Texture;
        if (m_Texture != null)
        {
            Vector2 size = new Vector2();
            size.x = m_Texture.width / m_HCount;
            size.y = m_Texture.height / m_VCount;
            m_Image.rectTransform.sizeDelta = size;
            m_Index = 0;
            ChangeFrame(0);
        }
    }

    public void RebuildAll()
    {
        RebuildData();
        RebuildRect();
        RebuildTexture();
    }

    public void Play()
    {
        m_IsPlay = true;
        m_Index = 0;
    }

    public void Stop()
    {
        m_IsPlay = false;
    }
}
