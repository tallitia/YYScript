using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LuaInterface;

[RequireComponent(typeof(Image))]
public class ScrapeImage : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    /// 计算像素百分比的位置偏移
    public RectOffset padding = new RectOffset();
    /// 鼠标位置半径
    public int radius = 10;
    /// 百分比触发完成事件
    public float percent = 1f;
    /// 开始回调
    public LuaFunction onStart;
    /// 结束回调
    public LuaFunction onFinish;

    private Image mImage;
    private Transform mTransform;
    private Sprite mSprite;

    private Texture2D mTexture;
    private Rect mInteraction;
    private int mTotalPixels = 0;
    private int mChangePixels = 0;
    private bool mIsDown = false;
    void Start()
    {
        mImage = GetComponent<Image>();
        mTransform = mImage.transform;
        mSprite = mImage.sprite;
        mInteraction = new Rect();
        Reset();
    }

    void Update()
    {
        if (mIsDown)
        {
            Vector3 pos = mTransform.InverseTransformPoint(Input.mousePosition);
            pos.x += mTexture.width / 2f;
            pos.y += mTexture.height / 2f;
            SetPixel((int)pos.x, (int)pos.y);
        }
    }

    bool IsPointInRect(int x, int y, Rect rect)
    {
        if (x >= rect.xMin && x <= rect.xMax &&
            y >= rect.yMin && y <= rect.yMax)
            return true;

        return false;
    }

    void SetPixel(int x, int y)
    {
        for (int i = -radius; i < radius; i++)
        {
            var py = y + i;
            if (py < 0 || py >= mTexture.height)
                continue;

            for (int j = -radius; j<radius; j++)
            {
                var px = x + j;
                if (px < 0 || px >= mTexture.width)
                    continue;
                
                if (new Vector2(px - x, py - y).magnitude > radius)
                    continue;

                Color color = mTexture.GetPixel(px, py);
                if (color.a == 0f)
                    continue;
                
                color.a = 0f;
                mTexture.SetPixel(px, py, color);

                if (IsPointInRect(x, y, mInteraction))
                    mChangePixels++;
            }
        }
        mTexture.Apply();

        /// 计算进度
        float ratio = (float)mChangePixels / mTotalPixels;
        if (ratio >= percent && onFinish != null)
        {
            onFinish.Call();
            enabled = false;
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        mIsDown = true;
		if (onStart != null) 
			onStart.Call ();
    }

    public void OnPointerUp(PointerEventData data)
    {
        mIsDown = false;
    }

    /// 重置
    public void Reset()
    {
        if (mSprite == null)
            return;
        
        Texture2D tex = mSprite.texture;
        mTexture = new Texture2D(tex.width, tex.height, tex.format, false);
        mTexture.SetPixels(tex.GetPixels());
        mTexture.Apply();

        mImage.sprite = Sprite.Create(mTexture, mSprite.rect, mSprite.pivot);
        mChangePixels = 0;

        mInteraction.x = padding.left;
        mInteraction.y = padding.top;
        mInteraction.width = Mathf.Max(0, mTexture.width - padding.left - padding.right);
        mInteraction.height = Mathf.Max(0, mTexture.height - padding.top - padding.bottom);
        mTotalPixels = (int)(mInteraction.width * mInteraction.height);

        percent = Mathf.Min(1f, percent);
        enabled = true;
		mIsDown = false;
    }
}
   