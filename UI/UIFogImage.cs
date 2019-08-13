using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LuaInterface;

[RequireComponent(typeof(Image))]
public class UIFogImage : MonoBehaviour
{
    /// 鼠标位置半径
    public int radius = 15;

    /// 半透明区域半径
    public int alpha = 40;

    private Image mImage;
    private Sprite mSprite;

    private Transform mTransform;
    private Texture2D mTexture;

    private Vector2 mScale;
    private bool mIsInit = false;

    public void SetPixel(Vector2 pos)
    {
        int x = Mathf.FloorToInt(pos.x / mScale.x + mTexture.width / 2f);
        int y = Mathf.FloorToInt(pos.y / mScale.y + mTexture.height / 2f);

        bool isChange = false;
        for (int i = -alpha; i < alpha; i++)
        {
            var py = y + i;
            if (py < 0 || py >= mTexture.height)
                continue;

            for (int j = -alpha; j < alpha; j++)
            {
                var px = x + j;
                if (px < 0 || px >= mTexture.width)
                    continue;

                var magnitude = new Vector2(px - x, py - y).magnitude;
                if (magnitude > alpha)
                    continue;

                var color = mTexture.GetPixel(px, py);
                if (magnitude < radius)
                {
                    if (color.a == 0f)
                        continue;

                    color.a = 0f;
                    mTexture.SetPixel(px, py, color);
                    isChange = true;
                }
                else
                {
                    var a = (magnitude - radius) / (alpha - radius);
                    if (color.a <= a)
                        continue;

                    color.a = a;
                    mTexture.SetPixel(px, py, color);
                    isChange = true;
                }
            }
        }

        if (isChange) mTexture.Apply();
    }


    public void SetPixels(Color[] colors)
    {
        mTexture.SetPixels(colors);
        mTexture.Apply();
    }

    public Color[] GetPixels()
    {
        return mTexture.GetPixels();
    }

    /// 重置
    public void Init()
    {
        if (!mIsInit)
        {
            mImage = GetComponent<Image>();
            mSprite = mImage.sprite;
            mTransform = mImage.transform;
            mIsInit = true;
        }

        if (mSprite == null)
            return;

        Texture2D tex = mSprite.texture;
        mTexture = new Texture2D(tex.width, tex.height, tex.format, false);
        mTexture.SetPixels(tex.GetPixels());
        mTexture.Apply();

        mImage.sprite = Sprite.Create(mTexture, mSprite.rect, mSprite.pivot);

        var rect = mImage.rectTransform.rect;
        mScale = new Vector2();
        mScale.x = rect.width / mTexture.width;
        mScale.y = rect.height / mTexture.height;
    }

    public float GetPixelAlpha(Vector2 pos)
    {
        int x = Mathf.FloorToInt(pos.x / mScale.x + mTexture.width / 2f);
        int y = Mathf.FloorToInt(pos.y / mScale.y + mTexture.height / 2f);
        var color = mTexture.GetPixel(x, y);
        return color.a;
    }
}
