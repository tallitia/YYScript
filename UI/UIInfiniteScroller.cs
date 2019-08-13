using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class UIInfiniteScroller : MonoBehaviour
{
    public Vector2 speed = Vector2.zero;
    private RectTransform mRect;
    private Vector2 mSize;
    void Start()
    {
        mRect = GetComponent<RectTransform>();
        mSize = mRect.sizeDelta;
    }

    void Update()
    {
        var pos = mRect.anchoredPosition;
        pos.x -= speed.x * Time.deltaTime;
        pos.y += speed.y * Time.deltaTime;
        if (pos.x < -mSize.x) pos.x += mSize.x;
        if (pos.y < -mSize.y) pos.y += mSize.y;
        mRect.anchoredPosition = pos;
    }

    public void Play()
    {
        enabled = true;
    }

    public void Stop()
    {
        enabled = false;
    }
}
