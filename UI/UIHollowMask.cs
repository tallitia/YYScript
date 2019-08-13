using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UIHollowMask : MonoBehaviour
{
    private const float ANIM_TIME = 0.15f;

    private bool mIsStart = false;
    private Image mImage;
    private Vector2 mSceneSize;

    private Rect mRect = new Rect();
    private float mTime = 0f;
    private Vector4 mSpeed = new Vector2();

    void Start()
    {
        if (mIsStart) return;
        var asset = MainGame.GetManager<AssetManager>();
        mImage = GetComponent<Image>();

        var layer = MainGame.GetManager<LayerManager>();
        mSceneSize = layer.ScreenSize;

        var mat = asset.LoadAsset<Material>("Shaders/MaskHollow.mat");
        mImage.material = Instantiate(mat) as Material;
        enabled = false;
        mIsStart = true;
    }

    void Update()
    {
        mTime += Time.deltaTime;
        if (mTime >= ANIM_TIME)
        {
            mTime = ANIM_TIME;
            enabled = false;
        }

        var x = mSpeed.x * mTime;
        var y = mSpeed.y * mTime;
        var w = mSceneSize.x - mSpeed.z * mTime;
        var h = mSceneSize.y - mSpeed.w * mTime;
        RawSet(x, y, w, h);
    }

    void RawSet(float x, float y, float w, float h)
    {
        x += mSceneSize.x / 2f;
        y += mSceneSize.y / 2f;
        w /= 2f;
        h /= 2f;

        x /= mSceneSize.x;
        y /= mSceneSize.y;
        w /= mSceneSize.x;
        h /= mSceneSize.y;

        var mat = mImage.material;
        mat.SetFloat("_centerX", x);
        mat.SetFloat("_centerY", y);
        mat.SetFloat("_sizeX", w);
        mat.SetFloat("_sizeY", h);
    }

    public void Play()
    {
        if (mRect.width == 0 || mRect.height == 0)
        {
            enabled = false;
            return ;
        }
        
        RawSet(0f, 0f, mSceneSize.x, mSceneSize.y);

        mSpeed.x = mRect.x / ANIM_TIME;
        mSpeed.y = mRect.y / ANIM_TIME;
        mSpeed.z = (mSceneSize.x - mRect.width) / ANIM_TIME;
        mSpeed.w = (mSceneSize.y - mRect.height) / ANIM_TIME;
        mTime = 0f;
        enabled = true;
    }

    /// pivot 0 中心点  1 上方  -1 下方 
    public void Set(float x, float y, float w, float h, int pivot)
    {
        if (!mIsStart) Start();
        if (pivot != 0) y += pivot * mSceneSize.y / 2f;

        mRect.x = x;
        mRect.y = y;
        mRect.width = w;
        mRect.height = h;
        RawSet(x, y, w, h);
    }

    public void Set(RectTransform trans, int pivot)
    {
        var parent = mImage.transform.parent;
        var pos = parent.InverseTransformPoint(trans.position);
        var size = trans.sizeDelta;
        Set(pos.x, pos.y, size.x, size.y, pivot);
    }

#if UNITY_EDITOR
	// [SerializeField]
    // private Rect TestRect;
    // [ContextMenu("Execute")]
    // private void Execute()
    // {
        // Set(TestRect.x, TestRect.y, TestRect.width, TestRect.height, 0);
        // Play();
    // }

    // public RectTransform TestTarget;
    // [ContextMenu("Attach")]
    // private void Attach()
    // {
        // Set(TestTarget, 0);
        // Play();
    // }

    // [ContextMenu("Parse")]
    // private void Parse()
    // {
        // TestRect.x = mRect.x;
        // TestRect.y = mRect.y;
        // TestRect.width = mRect.width;
        // TestRect.height = mRect.height;
    // }

    // [ContextMenu("ParseBottom")]
    // private void ParseBottom()
    // {
        // TestRect.x = mRect.x;
        // TestRect.y = mRect.y + mSceneSize.y/2f;
        // TestRect.width = mRect.width;
        // TestRect.height = mRect.height;
        // Debug.Log(TestRect.x + ", " + TestRect.y + ", " + TestRect.width + ", " + TestRect.height);
    // }

    // [ContextMenu("ParseTop")]
    // private void ParseTop()
    // {
        // TestRect.x = mRect.x;
        // TestRect.y = mRect.y - mSceneSize.y/2f;
        // TestRect.width = mRect.width;
        // TestRect.height = mRect.height;
        // Debug.Log(TestRect.x + ", " + TestRect.y + ", " + TestRect.width + ", " + TestRect.height);
    // }

#endif
}
