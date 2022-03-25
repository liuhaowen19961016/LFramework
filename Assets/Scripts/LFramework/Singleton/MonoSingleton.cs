using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour
    where T : MonoBehaviour
{
    private static T m_Ins = null;
    public static T Ins
    {
        get
        {
            if (m_Ins == null)
            {
                m_Ins = FindObjectOfType(typeof(T)) as T;
                if (m_Ins == null)
                {
                    m_Ins = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                    var root = GameObject.Find("MonoSingleton");
                    if (root == null)
                    {
                        root = new GameObject("MonoSingleton");
                        DontDestroyOnLoad(root);
                    }
                    m_Ins.transform.parent = root.transform;
                }
            }
            return m_Ins;
        }
    }
}