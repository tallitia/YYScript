using UnityEngine;
using System.Collections;

public class UILayoutVerticalTiled : UILayout
{
    public Vector2 spacing = Vector2.zero;
    public float cellWidth = 100f;
    public float cellHeight = 100f;
    public int columnLimit = 2;

    protected override void Start()
    {
        base.Start();

        mScroll.horizontal = false;
        mScroll.vertical = true;
        mContent.anchorMin = new Vector2(0f, 1f);
        mContent.anchorMax = new Vector2(0f, 1f);
        mContent.pivot = new Vector2(0f, 1f);
    }

    protected override int CalculateChindrenCount()
    {
        return (Mathf.CeilToInt(mScrollTrans.rect.height / (cellHeight + spacing.y)) + 1) * columnLimit;
    }

    protected override int CalculateIndex(Vector2 pos)
    {
        int col = (int)(pos.x / (cellWidth + spacing.x) );
        int row = Mathf.Abs((int)(pos.y / (cellHeight + spacing.y)));
        return row * columnLimit + col;
    }

    protected override float CalculateExtents()
    {
        float half = (cellHeight + spacing.y) * 0.5f;
        int row = mChildren.Count / columnLimit;
        return half * row;
    }

    protected override void CalculateBounds()
    {
        Vector2 size = mContent.sizeDelta;
        size.x = columnLimit * (cellWidth + spacing.x) + padding.left + padding.right - spacing.x;
        size.y = Mathf.CeilToInt((float)mDataCount/columnLimit) * (cellHeight + spacing.y) + padding.top + padding.bottom - spacing.y;
        mContent.sizeDelta = size;
    }

    protected override void ResetContent()
    {
        for (int i = 0; i < mChildren.Count; ++i)
        {
            RectTransform t = mChildren[i];
            int col = i % columnLimit;
            int row = i / columnLimit;
            t.anchoredPosition = new Vector2(col*(cellWidth + spacing.x) + padding.left, -row*(cellHeight + spacing.y) - padding.top);
            t.anchorMin = new Vector2(0f, 1f);
            t.anchorMax = new Vector2(0f, 1f);
            t.pivot = new Vector2(0f, 1f);
            UpdateItem(t, i);
        }
        Vector2 pos = mContent.anchoredPosition;
        pos.y = 0;
        mContent.anchoredPosition = pos;
        mScroll.StopMovement();
    }

    protected override void RefreshContent()
    {
        float offset = -(cellHeight + spacing.y) * 0.5f;
        float extents = CalculateExtents();
        Vector3 center = CalculateCenter();
        for (int i = 0; i < mChildren.Count; ++i)
        {
            RectTransform t = mChildren[i];
            Vector3 pos = t.transform.localPosition;
            float distance = pos.y - center.y + offset;
            if (distance < -extents)
            {
                pos.y += extents * 2f;
                if (pos.y <= 0) t.localPosition = pos;
            }
            else if (distance > extents)
            {
                pos.y -= extents * 2f;
                int rowCount = Mathf.CeilToInt((float)mDataCount / columnLimit);
                if (pos.y > -rowCount * (cellHeight + spacing.y) && CalculateIndex(t.anchoredPosition) < DataCount)
                    t.localPosition = pos;
            }

            int index = CalculateIndex(t.anchoredPosition);
            if (index >= mDataCount && t.gameObject.activeSelf)
            {
                t.localPosition += new Vector3(0f, extents * 2f, 0f);
                index = CalculateIndex(t.anchoredPosition);
            }
            UpdateItem(t, index);
        }
    }

    protected override void UpdateContent()
    {
        float offset = -(cellHeight + spacing.y) * 0.5f;
        float extents = (cellHeight + spacing.y) * mChildren.Count * 0.5f / (float)columnLimit;
        Vector3 center = CalculateCenter();
        for (int i = 0; i < mChildren.Count; ++i)
        {
            RectTransform t = mChildren[i];
            float distance = t.localPosition.y - center.y + offset;
            if (distance < -extents)
            {
                float newPosition = t.localPosition.y + extents * 2f;
                if (newPosition <= 0)
                {
                    int col = (int)(t.localPosition.x / (cellWidth + spacing.x));
                    int row = Mathf.Abs((int)(newPosition / (cellHeight + spacing.y)));
                    int index = row * columnLimit + col;
                    t.localPosition += new Vector3(0f, extents * 2f);
                    UpdateItem(t, index);
                }
            }
            else if (distance > extents)
            {
                float newPosition = t.localPosition.y - extents * 2f;
                int rowCount = Mathf.CeilToInt((float)mDataCount / columnLimit);
                if (newPosition > -rowCount * (cellHeight + spacing.y))
                {
                    int col = (int)(t.localPosition.x / (cellWidth + spacing.x));
                    int row = Mathf.Abs((int)(newPosition / (cellHeight + spacing.y)));
                    int index = row * columnLimit + col;
                    if (index < mDataCount)
                    {
                        t.localPosition -= new Vector3(0f, extents * 2f);
                        UpdateItem(t, index);
                    }
                }
            }
        }
    }
}

