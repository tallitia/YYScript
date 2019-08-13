using UnityEngine;
using UnityEngine.UI;

public class LayerManager : Manager
{
    private Vector3 mScreenScale;
    private Vector2 mScreenSize;
    private GameObject mAdjustor;

    private void Awake()
    {
        LogUtil.StartLog("LayerManager Awake");
        BindRoot();
    }

    public override void OnRestart()
    {
        LogUtil.StartLog("LayerManager OnInit");
        base.OnRestart();
        BindRoot();
    }

    private void BindRoot()
    {
        GameObject root = GameObject.FindGameObjectWithTag("UIRoot");
        if (root == null)
        {
            Util.LogError("UIRoot is null!!");
            return;
        }

        UIRoot = root.GetComponent<RectTransform>();
        mScreenSize = UIRoot.sizeDelta;
        mScreenScale = UIRoot.localScale;

        UIBackground = UIRoot.Find("UIBackground") as RectTransform;
        UIModule = UIRoot.Find("UIModule") as RectTransform;
        UIWindow = UIRoot.Find("UIWindow") as RectTransform;
        UIPopup = UIRoot.Find("UIPopup") as RectTransform;
        UIEffect = UIRoot.Find("UIEffect") as RectTransform;
        Native = new GameObject("Native").transform;

        // 宽屏适配
        WideScreen();
    }


    private Transform CreateLayer(Transform parent, string name, int order)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;

        RectTransform trans = go.GetComponent<RectTransform>();
        if (trans == null) trans = go.AddComponent<RectTransform>();
        trans.anchorMin = new Vector2(0f, 0f);
        trans.anchorMax = new Vector2(1f, 1f);
        trans.offsetMin = Vector2.zero;
        trans.offsetMax = Vector2.zero;

        Canvas canvas = go.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = order;

        go.AddComponent<GraphicRaycaster>();
        return go.transform;
    }

    private void AdjustOffset(Vector2 max, Vector2 min)
    {
        UIBackground.offsetMax = max;
        UIBackground.offsetMin = min;
        UIModule.offsetMax = max;
        UIModule.offsetMin = min;
        UIWindow.offsetMax = max;
        UIWindow.offsetMin = min;
        UIPopup.offsetMax = max;
        UIPopup.offsetMin = min;
        UIEffect.offsetMax = max;
        UIEffect.offsetMin = min;
    }

    private void WideScreen()
    {
        var ratio = mScreenSize.x / mScreenSize.y;
        if (ratio > 0.57f)
        {
            var prefab = Resources.Load<GameObject>("WideView");
            mAdjustor = GameObject.Instantiate(prefab, UIRoot);

            var scaler = UIRoot.GetComponent<CanvasScaler>();
            scaler.matchWidthOrHeight = 1f;

            var scale = mScreenSize.y / 1280f;
            var offset = (mScreenSize.x / scale - 720f) / 2f;
            var max = new Vector2(-offset, 0);
            var min = new Vector2(offset, 0);
            AdjustOffset(max, min);
            mScreenSize.x = 720f;
            mScreenSize.y /= scale;
            mScreenScale *= scale;
        }
    }

    public void Adjust(float pixel)
    {
        if (mScreenSize.y > pixel)
        {
            var prefab = Resources.Load<GameObject>("AdjustView");
            mAdjustor = Object.Instantiate(prefab, UIRoot);

            var max = new Vector2(0f, -58f);
            var min = new Vector2(0f, 58f);
            AdjustOffset(max, min);
            mScreenSize.y -= 116f;
        }
    }

    public RectTransform UIRoot { private set; get; }
    public RectTransform UIBackground { private set; get; }
    public RectTransform UIModule { private set; get; }
    public RectTransform UIWindow { private set; get; }
    public RectTransform UIPopup { private set; get; }
    public RectTransform UIEffect { private set; get; }
    public Transform Native { private set; get; }
    public Vector3 ScreenScale { private set; get; }
    public Vector2 ScreenSize { private set; get; }

}
