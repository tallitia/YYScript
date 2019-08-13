using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UITextAnimation : MonoBehaviour
{
    /// 动画间隔
    public float time = 1f;
    /// 动画的字数, 0为全部
    public int count = 0;

    private Text mText;
    private string mBackup;
    private float mDeltaTime = 0f;
    private int mIndex = 0;

    void Start()
    {
        mText = GetComponent<Text>();
        mBackup = mText.text;
        if (count == 0) count = mBackup.Length;
    }

    void Update()
    {
        mDeltaTime += Time.deltaTime;
        if (mDeltaTime >= time)
        {
            mDeltaTime -= time;
            mIndex += 1;
            mIndex %= (count + 1);
            Anim();
        }
    }

    void Anim()
    {
        int len = mBackup.Length - count;
        mText.text = mBackup.Substring(0, len + mIndex);
    }
}
