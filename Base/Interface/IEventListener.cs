public interface IEventListener
{
    void AddListener(string name, EventLib.EventHandler handler);
    void RemoveListener(string name, EventLib.EventHandler handler);
    void RemoveListeners(string name);
    void Brocast(string name, object data);
}