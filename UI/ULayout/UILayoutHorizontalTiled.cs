using UnityEngine;
using System.Collections;

public class UILayoutHorizontalTiled : UILayout
{
    public Vector2 spacing = Vector2.zero;
    public float cellWidth = 100f;
    public float cellHeight = 100f;
    public int rowLimit = 2;

    protected override void Start()
    {
        base.Start();

        mScroll.horizontal = true;
        mScroll.vertical = false;
        mContent.anchorMin = new Vector2(0f, 1f);
        mContent.anchorMax = new Vector2(0f, 1f);
        mContent.pivot = new Vector2(0f, 1f);
    }

    protected override int CalculateChindrenCount()
    {
        return (Mathf.CeilToInt(mScrollTrans.rect.width / (cellWidth + spacing.x)) + 1) * rowLimit;
    }

    protected override int CalculateIndex(Vector2 pos)
    {
        int col = (int)(pos.x / (cellWidth + spacing.x));
        int row = Mathf.Abs((int)(pos.y / (cellHeight + spacing.y)));
        return row + col * rowLimit;
    }

    protected override float CalculateExtents()
    {
        float half = (cellWidth + spacing.x) * 0.5f;
        int col = mChildren.Count / rowLimit;
        return half * col;
    }

    protected override void CalculateBounds()
    {
        Vector2 size = mContent.sizeDelta;
        size.x = Mathf.CeilToInt((float)mDataCount/rowLimit) * (cellWidth + spacing.x) + padding.left + padding.right - spacing.x;
        size.y = rowLimit * (cellHeight + spacing.y) + padding.top + padding.bottom - spacing.y;
        mContent.sizeDelta = size;
    }

    protected override void ResetContent()
    {
        for (int i = 0; i < mChildren.Count; ++i)
        {
            RectTransform t = mChildren[i];
            int col = i / rowLimit;
            int row = i % rowLimit;
            t.anchoredPosition = new Vector2(col*(cellWidth + spacing.x) + padding.left, -row*(cellHeight + spacing.y) - padding.top);
            t.anchorMin = new Vector2(0f, 1f);
            t.anchorMax = new Vector2(0f, 1f);
            t.pivot = new Vector2(0f, 1f);
            UpdateItem(t, i);
        }
        Vector2 pos = mContent.anchoredPosition;
        pos.x = 0;
        mContent.anchoredPosition = pos;
        mScroll.StopMovement();
    }

    protected override void RefreshContent()
    {
        float offset = (cellWidth + spacing.x) * 0.5f;
        float extents = CalculateExtents();
        Vector3 center = CalculateCenter();
        for (int i = 0; i < mChildren.Count; ++i)
        {

            RectTransform t = mChildren[i];
            Vector3 pos = t.localPosition;

            float distance = pos.x - center.x + offset;
            if (distance < -extents)
            {
                pos.x += extents * 2f;
                int colCount = Mathf.CeilToInt((float)mDataCount / rowLimit);
                if (pos.x < colCount * (cellWidth + spacing.x) && CalculateIndex(pos) < mDataCount)
                    t.localPosition = pos;
            }
            else if (distance > extents)
            {
                pos.x -= extents * 2f;
                if (pos.x >= 0)
                    t.localPosition = pos;
            }


            int index = CalculateIndex(t.anchoredPosition);
            if (index >= mDataCount && t.gameObject.activeSelf)
            {
                t.localPosition -= new Vector3(extents * 2f, 0f, 0f);
                index = CalculateIndex(t.anchoredPosition);
            }
            UpdateItem(t, index);
        }
    }

    protected override void UpdateContent()
    {
        float half = (cellWidth + spacing.x) * 0.5f;
        float extents = CalculateExtents();
        Vector3 center = CalculateCenter();
        for (int i = 0; i < mChildren.Count; ++i)
        {
            RectTransform t = mChildren[i];
            Vector3 pos = t.localPosition;

            float distance = pos.x - center.x + half;
            if (distance < -extents)
            {
                pos.x += extents * 2f;
                int colCount = Mathf.CeilToInt((float)mDataCount / rowLimit);
                if (pos.x < colCount * (cellWidth + spacing.x) && CalculateIndex(pos) < mDataCount)
                {
                    t.localPosition = pos;
                    UpdateItem(t, CalculateIndex(pos));
                }
            }
            else if (distance > extents)
            {
                pos.x -= extents * 2f;
                if (pos.x >= 0)
                {
                    t.localPosition = pos;
                    UpdateItem(t, CalculateIndex(pos));
                }
            }
        }
    }
}

