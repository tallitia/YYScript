using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public abstract class UILayout : MonoBehaviour
{
    /// 创建gameobject回调
    public Action<GameObject> onCreate;
    /// 更新回调
    public Action<GameObject, int> onUpdate;
    
    public RectOffset padding = new RectOffset();

    protected ScrollRect mScroll;
    protected RectTransform mScrollTrans;
    protected RectTransform mContent;

    protected int mDataCount = 0;
    protected GameObject mItemRenderer;
    
    protected bool mIsStart = false;
    protected bool mInvalidateRender = false;
    protected bool mInvalidateData = false;
    protected List<RectTransform> mChildren = new List<RectTransform>();
    protected virtual void Start ()
    {
        mScroll = GetComponentInParent<ScrollRect>();
        mScroll.onValueChanged.AddListener(OnClipMove);
        mScrollTrans = mScroll.GetComponent<RectTransform>();
        mContent = GetComponent<RectTransform>();

        mIsStart = true;
        OnValidateRender();
        OnValidateData();
    }
    
    protected virtual void OnDestroy()
    {
        onCreate = null;
        onUpdate = null;
    }

    protected void CommitData()
    {
        CullChildren();
        InvalidateBounds();
        ResetContent();
    }

    protected void CommitRender()
    {
        ClearChildren();
        CreateChildren();
    }

    protected void OnValidateRender()
    {
        if (mInvalidateRender)
        {
            mInvalidateRender = false;
            CommitRender();
        }
    }
    protected void OnValidateData()
    {
        if (mInvalidateData)
        {
            mInvalidateData = false;
            CommitData();
        }
    }

    /// 拖动界面
    protected void OnClipMove (Vector2 pos) 
    {
        UpdateContent();
    }

    /// 创建go
    protected void CreateChildren()
    {
        if (mItemRenderer != null)
        {
            int count = CalculateChindrenCount();
            for (int i=0; i<count; i++)
            {
                GameObject go = Util.AddChild(transform, mItemRenderer);
                RectTransform trans = go.GetComponent<RectTransform>();
                go.SetActive(i < mDataCount);
                mChildren.Add(trans);
                if (onCreate != null)
                    onCreate.Invoke(go);
            }
        }
    }

    /// 删除所有go
    protected void ClearChildren()
    {
        for (int i=0; i<mChildren.Count; i++)
            Destroy(mChildren[i].gameObject);

        mChildren.Clear();
    }

    /// 屏蔽多余的go
    protected void CullChildren()
    {
        for (int i = 0; i < mChildren.Count; ++i)
        {
            mChildren[i].gameObject.SetActive(i<mDataCount);
        }
    }

    /// 计算坐标中心
    protected virtual Vector3 CalculateCenter()
    {
        Vector2 size = mScrollTrans.rect.size;
        Vector2 pivot = mScrollTrans.pivot;
        Vector3 corners1 = new Vector3(-size.x * pivot.x, size.y * (1f-pivot.y)); //左上角
        Vector3 corners2 = new Vector3(size.x * (1f-pivot.x), -size.y * pivot.y); //右下角
        Vector3 pos1 = mContent.InverseTransformPoint(mScrollTrans.TransformPoint(corners1));
        Vector3 pos2 = mContent.InverseTransformPoint(mScrollTrans.TransformPoint(corners2));
        return (pos1 + pos2) * 0.5f;
    }

    /// 计算需要的go数量
    protected abstract int CalculateChindrenCount();

    /// 计算当前坐标对应的索引
    protected abstract int CalculateIndex(Vector2 pos);

    /// 计算显示范围
    protected abstract float CalculateExtents();

    /// 计算包裹区域
    protected abstract void CalculateBounds();

    /// 拖动的时候更新content
    protected abstract void UpdateContent();

    /// 重置content, 坐标也重置 
    protected abstract void ResetContent();

    /// 刷新content, 坐标不变
    protected abstract void RefreshContent();

    /// 更新item
    protected virtual void UpdateItem (RectTransform item, int index) 
    {
        if (DataCount == 0 || index >= DataCount || onUpdate == null)
            return;

        /// index + 1 对应lua table数组的index
        onUpdate.Invoke(item.gameObject, index + 1);
    }

    /// 重新计算包裹矩形
    public void InvalidateBounds()
    {
        CalculateBounds();
    }

    /// 重新创建render
    public void InvalidateRender()
    {
        mInvalidateRender = true;
        if (mIsStart) OnValidateRender();
    }

    /// 重新加载数据
    public void InvalidateData()
    {
        mInvalidateData = true;
        if (mIsStart) OnValidateData();
    }

    /// 刷新content, 位置不变
    public void Refresh()
    {
        CullChildren();
        InvalidateBounds();
        RefreshContent();
    }

    /// 重置content, 初始化位置
    public void Rebuild()
    {
        CullChildren();
        InvalidateBounds();
        ResetContent();
    }

    /// 清空content
    public void Clear()
    {
        mDataCount = 0;
        CullChildren();
    }

    /// 设置数据长度
    public int DataCount
    {
        set { mDataCount = value; }
        get {  return mDataCount; }
    }

    /// 设置长度，并重置刷新
    public void SetDataCount(int val)
    {
        DataCount = val;
        InvalidateData();
    }

    /// 设置渲染器
    public GameObject ItemRenderer
    {
        set
        {
            if (mItemRenderer != value)
            {
                mItemRenderer = value;
                InvalidateRender();
            }
        }
        get
        {
            return mItemRenderer;
        }
    }
}
