using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;

public class AudioManager : Manager
{
    public const string CLICK_SOUND = "Audio/Sound/UI_dianji.mp3";
    
    private const string MUSIC_SET = "MUSIC";
    private const string SOUND_SET = "SOUND";

    private new AudioSource audio;
    private Dictionary<string, AudioClip> dict;
    private Dictionary<string, AudioSource> loops;
    private Queue<AudioSource> queue;
    private Coroutine coroutine;

    /// 能否播放音乐
    private bool menable = true;

    /// 能否播放音效
    private bool senable = true;

    /// 缓存
    private string music;
    /// 二级缓存
    private string backup;

    void Awake()
    {
        LogUtil.StartLog("AudioManager Awake");
        audio = gameObject.AddComponent<AudioSource>();
        dict = new Dictionary<string, AudioClip>();
        loops = new Dictionary<string, AudioSource>();
        queue = new Queue<AudioSource>();
    }

    public override void OnInit()
    {
        base.OnInit();
        menable = PlayerPrefs.GetInt(MUSIC_SET, 1) == 1;
        senable = PlayerPrefs.GetInt(SOUND_SET, 1) == 1;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //PlaySound(CLICK_SOUND);
        }
    }

    /// 添加一个声音
    void Add(string key, AudioClip value)
    {
        if (dict.ContainsKey(key) || value == null) return;
        dict.Add(key, value);
    }

    /// 获取一个声音
    AudioClip Get(string key)
    {
        if (!dict.ContainsKey(key)) return null;
        return dict[key];
    }

    /// 载入一个音频
    AudioClip Load(string path)
    {
        AudioClip ac = Get(path);
        if (ac == null)
        {
            var asset = MainGame.GetManager<AssetManager>();
            ac = asset.LoadAudioClip(path);
            var name = Path.GetFileNameWithoutExtension(path);
            Add(name, ac);
        }
        return ac;
    }

    /// 创建一个播放器
    AudioSource CreatePlayer()
    {
        var go = new GameObject();
        go.transform.SetParent(transform);
        go.name = "AudioPlayer";
        return go.AddComponent<AudioSource>();
    }

    /// 获取一个播放器
    AudioSource GetPlayer()
    {
        if (queue.Count > 0)
            return queue.Dequeue();
        return CreatePlayer();
    }

    /// 删除一个播放器
    void DelPlayer(AudioSource player)
    {
        if (player.isPlaying)
            player.Stop();

        player.loop = false;
        queue.Enqueue(player);
    }

    IEnumerator Play(AudioSource player, AudioClip clip, Action onComplete)
    {
        
        if (clip)
        {
            player.clip = clip;
            player.Play();

            var time = clip.length / player.pitch;
            yield return new WaitForSeconds(time);
        }

        if (onComplete != null) onComplete();
    }

    /// 播放背景音乐
    public void PlayMusic(string path)
    {
        return;
        PlayMusicSet(path);
    }

    /// 播放一组音乐
    public void PlayMusicSet(string set)
    {
        return;
        backup = music;
        music = set;

        if (!menable) return;

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        var index = 0;
        var list = set.Split('|');
        var clip = Load(list[index]);

        Action next = null;
        next = () => {
            index = (index + 1) % list.Length;
            clip = Load(list[index]);
            coroutine = StartCoroutine(Play(audio, clip, next));
        };

        coroutine = StartCoroutine(Play(audio, clip, next));
    }

    /// 暂停音乐
    public void PauseMusic()
    {
        if (coroutine != null)
        {
            //StopCoroutine(coroutine);
            coroutine = null;
        }

        audio.Pause();
    }

    /// 停止音乐
    public void StopMusic()
    {
        if (coroutine != null)
        {
            //StopCoroutine(coroutine);
            coroutine = null;
        }

        audio.Stop();
    }

    /// 播放上一次音乐
    public void BackMusic()
    {
        return;
        if (backup != null) 
            PlayMusicSet(backup);
    }

    /// 播放音效
    public void PlaySound(string path, float volume=1.0f)
    {
        if (!isInit || !senable) return;

        var player = GetPlayer();
        player.volume = volume;
        var clip = Load(path);
        StartCoroutine(Play(player, clip, () =>
        {
            DelPlayer(player);
        }));
    }

    /// 播放循环音效
    public void PlayLoopSound(string key, string path)
    {
        return;
        AudioSource player;
        if (!loops.TryGetValue(key, out player))
        {
            player = GetPlayer();
            player.loop = true;
            player.volume = senable ? 1f : 0f;
            loops[key] = player;
        }

        var clip = Load(path);
        if (clip == null)
        {
            loops.Remove(key);
            DelPlayer(player);
            return;
        }

        StartCoroutine(Play(player, clip, null));
    }

    /// 停止播放循环音效
    public void StopLoopSound(string key)
    {
        if (loops.ContainsKey(key))
        {
            DelPlayer(loops[key]);
            loops.Remove(key);
        }
    }

    public bool SEnable
    {
        set
        {
            if (senable != value)
            {
                foreach (var pair in loops)
                {
                    pair.Value.volume = value ? 1f : 0f;
                }
            }
            senable = value;
            PlayerPrefs.SetInt(SOUND_SET, value ? 1 : 0);
        }
        get { return senable; }
    }

    public bool MEnable
    {
        set
        {
            menable = value;
            PlayerPrefs.SetInt(MUSIC_SET, value ? 1 : 0);
            if (!value) StopMusic();
            else PlayMusicSet(music);
        }
        get { return menable; }
    }
}