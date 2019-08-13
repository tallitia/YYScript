
public interface INativePlugins
{
    void Initialize();
    void Call(string method, params object[] args);
    T Call<T>(string method, params object[] args);
}
