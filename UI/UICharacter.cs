using System;
using UnityEngine;
using UnityEngine.UI;

public class UICharacter : MonoBehaviour
{
    public enum Status
    {
        Down = 0,
        Left,
        Right,
        Up
    }

    public UISpriteAnimation anim;
    public CharacterData character;
    public Status status = Status.Down;
    public Action onFinish { set; get; }

    protected Transform m_Trans;
    protected Vector3 m_MoveSpeed;
    protected Vector3 m_MoveTarget;

    protected float m_SmoothTime = 0f;
    protected float m_MoveTime = 0f;
    protected bool m_IsMoving = false;

    protected bool m_IsInit = false;

    protected virtual void Awake()
    {
        m_Trans = transform;
        InitData(character);
    }

    protected virtual void Update()
    {
        UpdateFrame(Time.unscaledDeltaTime);
    }

    protected virtual void UpdateFrame(float deltaTime)
    {
        if (m_IsMoving)
        {
            if (m_SmoothTime > 0)
            {
                deltaTime += m_SmoothTime;
                m_SmoothTime = 0f;
            }

            m_MoveTime -= deltaTime;
            if (m_MoveTime <= 0)
            {
                m_SmoothTime = -m_MoveTime;
                m_Trans.localPosition = m_MoveTarget;
                m_IsMoving = false;
                FinishMove();
            }
            else
            {
                var pos = m_Trans.localPosition;
                m_Trans.localPosition = pos + m_MoveSpeed * deltaTime;
            }
        }
    }

    protected virtual void FinishMove()
    {
        if (onFinish != null)
            onFinish.Invoke();
    }

    public void InitData(CharacterData data)
    {
        character = data;
        if (character == null) return;
        var list = character.GetSprites((int)status);
        anim.SetSprites(list);
    }

    public Status CalculateStatus(Vector3 dir)
    {
        if (dir.x > 0.5f) return Status.Right;
        if (dir.x < -0.5f) return Status.Left;
        if (dir.y > 0.5f) return Status.Up;
        return Status.Down;
    }

    public void SetStatus(Status val)
    {
        if (status != val)
        {
            status = val;
            anim.SetSprites(character.GetSprites((int)status));
        }
    }

    public bool IsMoving
    {
        get { return m_IsMoving; }
    }

    public int Direction
    {
        get { return (int)status; }
        set { SetStatus((Status)value); }
    }


    public virtual void MoveTo(Vector3 target, float Speed)
    {
        Vector3 start = m_Trans.localPosition;
        start.z = 0f;
        float distance = Vector3.Distance(start, target);
        if (distance > 0)
        {
            Vector3 dir = Vector3.Normalize(target - start);
            SetStatus(CalculateStatus(dir));

            m_MoveTarget = target;
            m_MoveSpeed = dir * Speed;
            m_MoveTime = distance / Speed;
            m_IsMoving = true;
        }
        else
        {
            FinishMove();
        }
    }

    public virtual void Stop()
    {
        m_IsMoving = false;
    }
}