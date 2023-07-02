using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour
    where T : MonoBehaviour
{
    private static GameObject root;

    private static T m_Ins = null;
    public static T Ins
    {
        get
        {
            if (root == null)
            {
                root = GameObject.Find("MonoSingleton");
                if (root == null)
                {
                    root = new GameObject("MonoSingleton");
                    DontDestroyOnLoad(root);
                }
            }
            if (m_Ins == null)
            {
                m_Ins = FindObjectOfType(typeof(T)) as T;
                string name = typeof(T).ToString();
                if (m_Ins == null)
                {
                    m_Ins = new GameObject(name, typeof(T)).GetComponent<T>();
                }
                else
                {
                    m_Ins.gameObject.name = name;
                }
                m_Ins.transform.parent = root.transform;
            }
            return m_Ins;
        }
    }
}