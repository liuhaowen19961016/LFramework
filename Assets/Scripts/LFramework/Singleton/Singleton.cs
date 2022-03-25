public abstract class Singleton<T>
    where T : class, new()
{
    static T m_Ins;
    public static T Ins
    {
        get
        {
            if (m_Ins == null)
            {
                m_Ins = new T();
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

    public static void Destroy()
    {
        m_Ins = null;
    }
}
