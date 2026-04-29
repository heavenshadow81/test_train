using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SoundData
{
    public string SoundName;
    public AudioClip Clip;
    public float volume;
    public bool loop;
}

public class SoundMGR : MonoBehaviour
{
    public static SoundMGR Instance;

    public List<AudioSource> audioSources = new List<AudioSource>(); // AudioSource 리스트로 변경
    public AudioSource bgmSource;

    public List<SoundData> soundData = new List<SoundData>();

    private Dictionary<string, AudioSource> activeSounds = new Dictionary<string, AudioSource>();

    private void Awake()
    {
        Instance = null;
        Instance = this;
    }

    /// <summary>
    /// 지정된 사운드를 재생합니다.
    /// - 사용 가능한 AudioSource를 찾거나, 없으면 새로 생성합니다.
    /// - SoundData를 기준으로 AudioSource를 설정한 뒤 재생합니다.
    /// </summary>
    public void SoundPlay(string soundName)
    {
        var soundDt = soundData.Find(x => x.SoundName == soundName);
        if (soundDt.Clip == null) return;

        // 사용 가능한 AudioSource 찾기
        AudioSource availableSource = audioSources.Find(source => !source.isPlaying);

        // 만약 사용 가능한 AudioSource가 없다면 새로 생성
        if (availableSource == null)
        {
            availableSource = gameObject.AddComponent<AudioSource>();
            audioSources.Add(availableSource); // 리스트에 추가
        }

        // AudioSource 설정
        availableSource.clip = soundDt.Clip;
        availableSource.volume = soundDt.volume;
        availableSource.loop = soundDt.loop;

        // 특정 사운드가 재생 중으로 등록
        if (activeSounds.ContainsKey(soundName))
            activeSounds[soundName] = availableSource;
        else
            activeSounds.Add(soundName, availableSource);

        // 사운드 재생
        if (availableSource.loop)
        {
            availableSource.Play();
        }
        else
        {
            availableSource.PlayOneShot(availableSource.clip);
        }
    }

    /// <summary>
    /// 특정 사운드의 재생을 중지합니다.
    /// - activeSounds에서 제거하여 관리 해제
    /// </summary>
    public void SoundStop(string soundName)
    {
        if (activeSounds.ContainsKey(soundName))
        {
            activeSounds[soundName].Stop();
            activeSounds.Remove(soundName);
        }
    }

    /// <summary>
    /// 해당 사운드가 재생 중이 아니라면 재생합니다.
    /// - 중복 재생 방지용
    /// </summary>
    public void SoundPlayIfNotPlaying(string soundName)
    {
        if (IsPlaying(soundName)) return;

        SoundPlay(soundName);
    }

    /// <summary>
    /// 사운드를 다시 재생합니다.
    /// - 이미 재생 중이면 정지 후 재생
    /// </summary>
    public void SoundRePlay(string soundName)
    {
        // 기존에 등록된 사운드가 있는지 확인
        if (IsPlaying(soundName))
        {
            SoundStop(soundName);
        }

        // 사운드가 등록되어 있지 않다면 SoundPlay 호출
        SoundPlay(soundName);
    }

    public bool IsPlaying(string soundName)
    {
        return activeSounds.ContainsKey(soundName) && activeSounds[soundName].isPlaying;
    }
}
