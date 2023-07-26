/// <summary>
/// 对象池接口
/// </summary>
public interface IObjectPool<T>
{
    /// <summary>
    /// 从池子中取
    /// </summary>
    T Get();

    /// <summary>
    /// 放回池子
    /// </summary>
    bool Put(T t);

    /// <summary>
    /// 全部放回池子
    /// </summary>
    void PutAll();
}