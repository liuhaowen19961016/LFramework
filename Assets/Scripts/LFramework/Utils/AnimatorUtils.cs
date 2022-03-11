using UnityEngine;

/// <summary>
/// 动画工具类
/// </summary>
public static class AnimatorUtils
{
    /// <summary>
    /// 得到动画片段的长度
    /// </summary>
    public static float GetAnimationClipLength(Animator animator, string animationName)
    {
        if (animator == null)
        {
            Debug.LogError("动画状态机为null");
            return 0;
        }
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Equals(animationName))
            {
                return clip.length;
            }
        }
        Debug.LogError("找不到此动画片段：" + animationName);
        return 0;
    }

    /// <summary>
    /// 得到动画片段的长度
    /// </summary>
    public static float GetAnimationClipLength(Animator animator, int index)
    {
        if (animator == null)
        {
            Debug.LogError("动画状态机为null");
            return 0;
        }
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        if (clips == null
            || clips.Length <= index)
        {
            Debug.LogError("动画片段下标错误：" + index);
            return 0;
        }
        return clips[index].length;
    }
}
