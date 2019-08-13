using UnityEngine;
using System.Collections;

public class UIFollower : MonoBehaviour
{
    public Transform follow;
    public Vector3 offset;

    private Transform mTrans;
    void Start()
    {
        mTrans = transform;
    }

    void Update()
    {
        if (follow != null)
        {
            var pos1 = mTrans.position;
            var pos2 = follow.position + offset;
            if (pos1.x != pos2.x || pos1.y != pos2.y)
            {
                pos1.x = pos2.x;
                pos1.y = pos2.y;
                mTrans.position = pos1;
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Execute")]
    private void Execute()
    {
        mTrans = transform;
        Update();
    }
#endif
}
