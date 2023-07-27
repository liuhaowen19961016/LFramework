using UnityEngine;

/// <summary>
/// 特效接口
/// </summary>
public interface IEffect : IEffectAction
{
    EEffectType EffectType { get; }//特效类型
    GameObject EffectGo { get; }//特效实体
    EffectDefine EffectDefine { get; }//特效配置
    float Duration { get; }//持续时间

    /// <summary>
    /// 设置世界坐标位置
    /// </summary>
    void SetWorldPosition(Vector3 worldPos);

    /// <summary>
    /// 设置世界坐标旋转
    /// </summary>
    void SetWorldRotation(Vector3 worldRotation);

    /// <summary>
    /// 设置局部坐标位置
    /// </summary>
    void SetLocalPosition(Vector3 localPos);

    /// <summary>
    /// 设置局部坐标旋转
    /// </summary>
    void SetLocalRotation(Vector3 localRotation);

    /// <summary>
    /// 设置缩放
    /// </summary>
    void SetScale(Vector3 scale);

    /// <summary>
    /// 设置速度
    /// </summary>
    void SetSpeed(float speed);

    /// <summary>
    /// 设置层级
    /// </summary>
    void SetLayer(int layer, bool inculdeAllChildren);

    /// <summary>
    /// 设置层级排序
    /// </summary>
    void SetOrder(int order);

    /// <summary>
    /// 改变挂点
    /// </summary>
    void ChangeRoot(Transform parent);
}

/// <summary>
/// 特效事件接口
/// </summary>
public interface IEffectAction
{
    /// <summary>
    /// 初始化（获取实体、设置参数）
    /// </summary>
    bool Init(EffectDefine effectDefine);

    /// <summary>
    /// 获取特效的回调
    /// </summary>
    void OnGet();

    /// <summary>
    /// 回收特效的回调
    /// </summary>
    void OnPut();
}

/// <summary>
/// 特效类型
/// </summary>
public enum EEffectType
{
    Null,
    Common = 1,             //通用特效
    UI,                     //UI特效
}