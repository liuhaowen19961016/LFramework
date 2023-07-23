using UnityEngine;

/// <summary>
/// 音频管理器
/// </summary>
public class SoundMgr : MonoSingleton<SoundMgr>
{
    public const string SoundDir = "Sounds/";//音频目录

    //BGM播放器
    private AudioSource bgmPlayer;
    public AudioSource BgmPlayer { get { return bgmPlayer; } }
    //音效播放器
    private AudioSource soundPlayer;
    public AudioSource SoundPlayer { get { return soundPlayer; } }

    //是否全局静音
    public bool IsGlobalMute { get { return bgmPlayer.mute && soundPlayer.mute; } }

    public void Awake()
    {
        if (bgmPlayer == null)
        {
            bgmPlayer = gameObject.AddComponent<AudioSource>();
            bgmPlayer.loop = true;
            bgmPlayer.playOnAwake = false;
            bgmPlayer.mute = false;
        }
        if (soundPlayer == null)
        {
            soundPlayer = gameObject.AddComponent<AudioSource>();
            soundPlayer.loop = false;
            soundPlayer.playOnAwake = false;
            soundPlayer.mute = false;
        }
    }

    #region Interface

    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySound(string soundName, float volume = 1, bool loop = false)
    {
        AudioClip clip = Resources.Load<AudioClip>(SoundDir + soundName);
        if (clip == null)
        {
            Debug.LogError("没有此音频：" + soundName);
            return;
        }
        soundPlayer.clip = clip;
        soundPlayer.volume = volume;
        soundPlayer.loop = loop;
        soundPlayer.Play();
    }

    /// <summary>
    /// 播放背景音乐
    /// </summary>
    public void PlayBGM(string bgmName, float volume = 1, bool loop = true)
    {
        AudioClip clip = Resources.Load<AudioClip>(SoundDir + bgmName);
        if (clip == null)
        {
            Debug.LogError("没有此音频：" + bgmName);
            return;
        }
        bgmPlayer.clip = clip;
        bgmPlayer.volume = volume;
        bgmPlayer.loop = loop;
        bgmPlayer.Play();
    }

    /// <summary>
    /// 背景音乐渐隐
    /// </summary>
    public void FadeBGM(float fadeDuration)
    {
        bgmFade = true;
        bgmFadeBeginTime = Time.realtimeSinceStartup;
        bgmFadeBeginVolume = bgmPlayer.volume;
        bgmFadeDuration = fadeDuration;
    }

    /// <summary>
    /// 暂停播放
    /// </summary>
    public void PausePlay()
    {
        bgmPlayer.Pause();
        soundPlayer.Pause();
    }

    /// <summary>
    /// 继续播放
    /// </summary>
    public void ResumePlay()
    {
        bgmPlayer.Play();
        soundPlayer.Play();
    }

    /// <summary>
    /// 停止播放
    /// </summary>
    public void StopPlay()
    {
        bgmPlayer.Stop();
        soundPlayer.Stop();
    }

    /// <summary>
    /// 设置静音状态
    /// </summary>
    public void SetMuteState(bool b)
    {
        bgmPlayer.mute = b;
        soundPlayer.mute = b;
    }

    /// <summary>
    /// 设置BGM静音状态
    /// </summary>
    public void SetBgmMuteState(bool b)
    {
        bgmPlayer.mute = b;
    }

    /// <summary>
    /// 设置Sound静音状态
    /// </summary>
    public void SetSoundMuteState(bool b)
    {
        soundPlayer.mute = b;
    }

    /// <summary>
    /// 停止播放BGM
    /// </summary>
    public void StopPlayBGM()
    {
        bgmPlayer.Stop();
    }

    /// <summary>
    /// 暂停播放BGM
    /// </summary>
    public void PausePlayBGM()
    {
        bgmPlayer.Pause();
    }

    /// <summary>
    /// 继续播放BGM
    /// </summary>
    public void ResumePlayBGM()
    {
        bgmPlayer.Play();
    }

    /// <summary>
    /// 停止播放音效
    /// </summary>
    public void StopPlaySound()
    {
        soundPlayer.Stop();
    }

    #endregion

    #region Tools

    private bool bgmFade;//背景音乐是否渐隐
    private float bgmFadeBeginVolume;//背景音乐渐隐开始时的音量
    private float bgmFadeBeginTime;//背景音乐渐隐开始的时间
    private float bgmFadeDuration;//背景音乐渐隐的时间
    private void Update()
    {
        if (bgmFade)
        {
            float delta = Time.realtimeSinceStartup - bgmFadeBeginTime;
            if (delta <= bgmFadeDuration)
            {
                bgmPlayer.volume = Mathf.Lerp(bgmFadeBeginVolume, 0, delta / bgmFadeDuration);
            }
            else
            {
                bgmFade = false;
                bgmPlayer.volume = 0;
            }
        }
    }

    #endregion
}