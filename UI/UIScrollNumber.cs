using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class UIScrollNumber : MonoBehaviour
{
    public int count = 20;
    public float time = 1f;
    public Ease ease = Ease.OutSine;

    private ScrollRect mScroll;
    private UILayout mLayout;
    private List<string> mList;
    private Dictionary<GameObject, Text> mRenders;

    void Start()
    {
        mScroll = GetComponent<ScrollRect>();
        mLayout = Util.Get<UILayout>(transform, "Viewport/Content");
        mLayout.onCreate = OnCreateRender;
        mLayout.onUpdate = OnUpdateRender;
        mLayout.ItemRenderer = Util.Child(transform, "Viewport/Content/Render");

        mRenders = new Dictionary<GameObject, Text>();
        mList = new List<string>();
        mList.Add("-");

        mLayout.DataCount = mList.Count;
        mLayout.InvalidateData();
    }

    void OnCreateRender(GameObject go)
    {
        var text = go.GetComponent<Text>();
        mRenders.Add(go, text);
    }

    void OnUpdateRender(GameObject go, int index)
    {
        var render = mRenders[go];
        render.text = mList[index-1];
    }

    public void Play(string result, TweenCallback callback)
    {
        mList.Clear();
        mList.Add("-");
        for (int i=0; i<count-1; i++)
        {
            var rnd = UnityEngine.Random.Range(0, 10);
            mList.Add(rnd.ToString());
        }
        mList.Add(result);

        mLayout.DataCount = mList.Count;
        mLayout.InvalidateData();

        var height = (mLayout as UILayoutVertical).cellHeight;
        var trans = mLayout.transform as RectTransform;
        var pos = trans.anchoredPosition;
        pos.y = 0;
        trans.anchoredPosition = pos;
        trans.DOAnchorPosY(height * count, time).SetEase(ease).OnComplete(callback);
    }

    public void Clear()
    {
        if (mList != null)
        {
            mScroll.vertical = false;
            mList.Clear();
            mList.Add("-");
            mLayout.DataCount = mList.Count;
            mLayout.InvalidateData();
        }
    }
}
