using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance; // 싱글톤 패턴
    public AudioSource bgmSource; // 배경음
    public AudioSource sfxSource; // 효과음
    public AudioClip[] sfxClips; // 여러 효과음을 저장할 배열

    void Awake()
    {
        // 싱글톤 설정 (SoundManager가 중복 생성되지 않도록 보장)
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 씬 변경 시에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 배경음 재생
    public void PlayBGM(AudioClip bgm, float volume = 0.5f)
    {
        if (bgmSource.clip == bgm) return; // 같은 BGM이면 중복 재생 안 함
        bgmSource.clip = bgm;
        bgmSource.volume = volume;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM(AudioClip bgm, float volume = 0.5f)
    {
        bgmSource.Stop();
    }

    // 효과음 재생
    public void PlaySFX(AudioClip sfx, float volume = 1f)
    {
        sfxSource.PlayOneShot(sfx, volume);
    }

    // 특정 효과음을 배열에서 찾아서 재생 (효과음 이름으로 재생)
    public void PlaySFX(string sfxName)
    {
        foreach (AudioClip clip in sfxClips)
        {
            if (clip.name == sfxName)
            {
                PlaySFX(clip);
                return;
            }
        }
        Debug.LogWarning("효과음 없음: " + sfxName);
    }
}
