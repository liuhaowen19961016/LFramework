using System;

public abstract class Singleton<T>
    where T : class
{
    private static T m_Ins;
    public static T Ins
    {
        get
        {
            if (m_Ins == null)
            {
                m_Ins = Activator.CreateInstance(typeof(T), true) as T;
            }
            return m_Ins;
        }
    }

    protected Singleton()
    {
        Init();
    }

    public virtual void Init()
    {

    }
}