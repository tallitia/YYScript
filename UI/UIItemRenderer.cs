using LuaInterface;

public class UIItemRenderer : UIBaseBehaviour
{
    public LuaFunction eventHandler;

    public LuaFunction onStart;
    public LuaFunction onCommit;
    public LuaFunction onDestroy;

    protected override void Start()
    {
        base.Start();
        if (onStart != null)
            onStart.Call(gameObject);
    }

    protected override void CommitData()
    {
        if (onCommit != null)
            onCommit.Call(gameObject, mData);
    }

    protected override void OnDestroy()
    {
        if (onDestroy != null)
            onDestroy.Call(gameObject);
    }
}