using UnityEngine;

/// <summary>
/// Mono单例模版
/// </summary>
public class MonoSingleton<T> : MonoBehaviour
    where T : MonoBehaviour
{
    public T _ins;
    public T Ins
    {
        get
        {
            if (_ins == null)
            {

            }
            return _ins;
        }
    }
}
