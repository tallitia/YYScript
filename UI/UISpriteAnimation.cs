using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Image))]
public class UISpriteAnimation : MonoBehaviour
{
    [SerializeField]protected Sprite[] m_Sprites;
    public Sprite[] sprites { get { return m_Sprites; } set { m_Sprites = value; RebuildSprite(); } }

    [SerializeField]protected int m_FPS = 8;
    public int fps { get { return m_FPS; } set { m_FPS = value; RebuildData(); } }

    [SerializeField]protected bool m_IsPlay = true;
    public bool isPlay { get { return m_IsPlay; } set { m_IsPlay = value; } }

    [SerializeField]protected bool m_IsLoop = true;
    public bool isLoop { get { return m_IsLoop; } set { m_IsLoop = value; } }

    [SerializeField]protected bool m_IsSnap = true;
    public bool isSnap { get { return m_IsSnap; } set { m_IsSnap = value; } }

    [SerializeField]protected bool m_IsDestroyOnFinish = false;
    public bool isDestroyOnFinish { get { return m_IsDestroyOnFinish; } set { m_IsDestroyOnFinish = value; } }

    protected float m_DeltaTime = 0f;
    protected float m_FStep = 0f;
    protected int m_Index = 0;

    protected Image m_Image;

    public Action<GameObject> onComplete;
    void Start()
    {
        RebuildAll();
    }

    void Update()
    {
        if (m_Sprites == null || m_Sprites.Length == 0)
            return;

        if (m_FPS <= 0)
            return;

        if (!m_IsPlay)
            return;

        m_DeltaTime += Time.deltaTime;
        if (m_DeltaTime >= m_FStep) //next frame
        {
            m_DeltaTime -= m_FStep;
            if (++m_Index >= m_Sprites.Length)
            {
                m_IsPlay = m_IsLoop;
                m_Index = 0;
            }

            if (m_IsPlay) ChangeFrame(m_Index);
            else
            {
                if (onComplete != null) onComplete(gameObject);
                if (m_IsDestroyOnFinish) Destroy(gameObject);
            }
        }
    }

    protected void ChangeFrame(int index)
    {
        m_Image.sprite = m_Sprites[index];
        if (m_IsSnap) m_Image.SetNativeSize();
    }

    public void RebuildData()
    {
        m_FStep = 1f / m_FPS;
    }

    public void RebuildSprite()
    {
        if (m_Image == null) m_Image = GetComponent<Image>();
        if (m_Sprites != null && m_Sprites.Length > 0) ChangeFrame(0);
    }

    public void RebuildAll()
    {
        RebuildData();
        RebuildSprite();
    }

    public void Play()
    {
        m_Index = 0;
        m_IsPlay = true;
    }

    public void Stop()
    {
        m_IsPlay = false;
    }

    public void SetSprites(Sprite[] sprites)
    {
        m_Sprites = sprites;
        RebuildAll();
    }
    
	public float Length
	{
		get {
			int count = m_Sprites != null ? m_Sprites.Length : 0;
            return 1f / m_FPS * count;
		}
	}
}
