using System;
using UnityEngine;

public class UIParticleAnimation : MonoBehaviour
{
    public float duration = 1f;
    public bool isLoop = false;
    public Action<GameObject> onComplete;

    private ParticleSystem[] mPars;
    private float mDeltaTime = 0f;
    private float mScale = 1f;

    void Start()
    {
        mPars = GetComponentsInChildren<ParticleSystem>(true);
    }

    void Update()
    {
        if (isLoop)
        {
            enabled = false;
            return;
        }

        mDeltaTime += Time.deltaTime;
        if (mDeltaTime >= duration)
        {
            gameObject.SetActive(false);
            if (onComplete != null)
                onComplete(gameObject);
        }
    }

    void OnEnable()
    {
        mDeltaTime = 0f;
    }

    public void SetScale(float scale)
    {
        if (mPars == null)
            return;

        if (mScale != scale)
        {
            for (int i=0; i< mPars.Length; i++)
            {
                var trans = mPars[i].transform;
                var old = trans.localScale;
                old.x *= scale;
                old.y *= scale;
                old.z *= scale;
                trans.localScale = old;
            }
            mScale = scale;
        }
    }

    public void Play()
    {
        gameObject.SetActive(true);
    }
}
