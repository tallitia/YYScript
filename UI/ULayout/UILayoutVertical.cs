using UnityEngine;
using System.Collections;

public class UILayoutVertical : UILayout
{
    public float spacing = 0f;
    public float cellHeight = 100f;
    protected override void Start()
    {
        base.Start();

        mScroll.horizontal = false;
        mScroll.vertical = true;        
        mContent.anchorMin = new Vector2(0.5f, 1f);
        mContent.anchorMax = new Vector2(0.5f, 1f);
        mContent.pivot = new Vector2(0.5f, 1f);
    }

    protected override int CalculateChindrenCount()
    {
        return Mathf.CeilToInt(mScrollTrans.rect.height / (cellHeight + spacing)) + 1;
    }

    protected override int CalculateIndex(Vector2 pos)
    {
        return Mathf.Abs((int)(pos.y / (cellHeight + spacing)));
    }

    protected override float CalculateExtents()
    {
        float half = (cellHeight + spacing) * 0.5f;
        return half * mChildren.Count; 
    }

    protected override void CalculateBounds()
    {
        Vector2 size = mContent.sizeDelta;
        size.y = mDataCount * (cellHeight + spacing) + padding.top + padding.bottom - spacing;
        mContent.sizeDelta = size;
    }

    protected override void ResetContent()
    {
        for (int i = 0; i < mChildren.Count; ++i)
        {
            RectTransform t = mChildren[i];
            t.anchoredPosition = new Vector2(padding.left, -i * (cellHeight + spacing) - padding.top);
            t.anchorMin = new Vector2(0.5f, 1f);
            t.anchorMax = new Vector2(0.5f, 1f);
            t.pivot = new Vector2(0.5f, 1f);
            UpdateItem(t, i);
        }
        Vector2 pos = mContent.anchoredPosition;
        pos.y = 0;
        mContent.anchoredPosition = pos;
        mScroll.StopMovement();
    }

    protected override void RefreshContent()
    {
        float extents = (cellHeight + spacing) * mChildren.Count * 0.5f;
        for (int i = 0; i < mChildren.Count; ++i)
        {
            RectTransform t = mChildren[i];
            int index = CalculateIndex(t.anchoredPosition);
            if (index >= mDataCount && t.gameObject.activeSelf)
            {
                t.anchoredPosition += new Vector2(0f, extents * 2f);
                index = CalculateIndex(t.anchoredPosition);
            }
            UpdateItem(t, index);
        }
    }

    protected override void UpdateContent()
    {
        float offset = -(cellHeight + spacing) * 0.5f;
        float extents = CalculateExtents();
        Vector3 center = CalculateCenter();
        for (int i = 0; i < mChildren.Count; ++i)
        {
            RectTransform t = mChildren[i];
            Vector3 pos = t.localPosition;

            float distance = pos.y - center.y + offset;
            if (distance < -extents)
            {
                pos.y += extents * 2f;  //new position
                if (pos.y <= 0)
                {
                    t.localPosition = pos;
                    UpdateItem(t, CalculateIndex(t.anchoredPosition));
                }
            }
            else if (distance > extents)
            {
                pos.y -= extents * 2f;
                if (pos.y > -mDataCount * (cellHeight + spacing))
                {
                    t.localPosition = pos;
                    UpdateItem(t, CalculateIndex(t.anchoredPosition));
                }
            }
        }
    }
}

