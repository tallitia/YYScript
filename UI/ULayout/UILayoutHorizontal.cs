using UnityEngine;
using System.Collections;

public class UILayoutHorizontal : UILayout
{
    public float spacing = 0f;
    public float cellWidth = 100f;
    protected override void Start()
    {
        base.Start();

        mScroll.horizontal = true;
        mScroll.vertical = false;
        mContent.anchorMin = new Vector2(0f, 0.5f);
        mContent.anchorMax = new Vector2(0f, 0.5f);
        mContent.pivot = new Vector2(0f, 0.5f);
    }

    protected override int CalculateChindrenCount()
    {
        return Mathf.CeilToInt(mScrollTrans.rect.width / (cellWidth + spacing)) + 1;
    }

    protected override int CalculateIndex(Vector2 pos)
    {
        return Mathf.Abs((int)(pos.x / (cellWidth + spacing)));
    }

    protected override float CalculateExtents()
    {
        float half = (cellWidth + spacing) * 0.5f;
        return half * mChildren.Count;
    }

    protected override void CalculateBounds()
    {
        Vector2 size = mContent.sizeDelta;
        size.x = mDataCount * (cellWidth + spacing) + padding.left + padding.right - spacing;
        mContent.sizeDelta = size;
    }

    protected override void ResetContent()
    {
        for (int i = 0; i < mChildren.Count; ++i)
        {
            RectTransform t = mChildren[i];
            t.anchoredPosition = new Vector2(padding.left + i * (cellWidth + spacing), -padding.top);
            t.anchorMin = new Vector2(0f, 0.5f);
            t.anchorMax = new Vector2(0f, 0.5f);
            t.pivot = new Vector2(0f, 0.5f);
            UpdateItem(t, i);
        }

        Vector2 pos = mContent.anchoredPosition;
        pos.x = 0;
        mContent.anchoredPosition = pos;
        mScroll.StopMovement();
    }

    protected override void RefreshContent()
    {
        float extents = (cellWidth + spacing) * mChildren.Count * 0.5f;
        for (int i = 0; i < mChildren.Count; ++i)
        {
            RectTransform t = mChildren[i];
            int index = CalculateIndex(t.anchoredPosition);
            if (index >= mDataCount && t.gameObject.activeSelf)
            {
                t.anchoredPosition -= new Vector2(extents * 2f, 0f);
                index = CalculateIndex(t.anchoredPosition);
            }
            UpdateItem(t, index);
        }
    }

    protected override void UpdateContent()
    {
        float offset = (cellWidth + spacing) * 0.5f;
        float extents = CalculateExtents();
        Vector3 center = CalculateCenter();
        for (int i = 0; i < mChildren.Count; ++i)
        {
            RectTransform t = mChildren[i];
            Vector3 pos = t.localPosition;

            float distance = pos.x - center.x + offset;
            if (distance < -extents)
            {
                pos.x = pos.x + extents * 2f;
                if (pos.x < mDataCount * (cellWidth + spacing))
                {
                    t.localPosition = pos;
                    UpdateItem(t, CalculateIndex(t.anchoredPosition));
                }
            }
            else if (distance > extents)
            {
                pos.x = pos.x - extents * 2f;
                if (pos.x >= 0)
                {
                    t.localPosition = pos;
                    UpdateItem(t, CalculateIndex(t.anchoredPosition));
                }
            }
        }
    }
}

