using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    private AudioClip backgroundMusic;
    private AudioClip bossMusic;
    private AudioClip[] skillSounds = new AudioClip[4];
    private AudioClip attackSound;         // 기존 기본 공격 사운드
    private AudioClip swordAttackSound;    // 새로 추가된 검 평타 사운드

    [Header("볼륨 조절")]
    [Range(0f, 1f)] public float bgmVolume = 0.7f;
    [Range(0f, 1f)] public float sfxVolume = 1.0f;

    void Awake()
    {
        // 싱글톤 처리
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // AudioSource 준비
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;

            // 클립들 불러오기 (Resources/Audio/)
            backgroundMusic = TryLoadClip("Audio/background_bgm");
            bossMusic       = TryLoadClip("Audio/boss_music");
            attackSound     = TryLoadClip("Audio/attack");
            for (int i = 0; i < 4; i++)
                skillSounds[i] = TryLoadClip($"Audio/skill{i+1}");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private AudioClip TryLoadClip(string path)
    {
        AudioClip clip = Resources.Load<AudioClip>(path);
        if (clip == null)
            Debug.LogWarning($"[AudioManager] {path} 오디오 파일을 찾지 못했습니다. (Assets/Resources/{path}.mp3 등 확인)");
        return clip;
    }

    // --------------------------
    // BGM 관련
    // --------------------------
    public void PlayBackgroundMusic()
    {
        PlayMusic(backgroundMusic);
    }

    public void PlayBossMusic()
    {
        PlayMusic(bossMusic);
    }

    public void StopBGM()
    {
        if (bgmSource.isPlaying)
            bgmSource.Stop();
    }

    private void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("[AudioManager] 재생할 BGM이 없습니다.");
            return;
        }
        if (bgmSource.clip == clip && bgmSource.isPlaying)
            return; // 이미 같은 곡이 재생 중이면 무시

        bgmSource.clip = clip;
        bgmSource.volume = bgmVolume;
        bgmSource.Play();
    }

    // --------------------------
    // 효과음 관련
    // --------------------------
    // 기본 공격 사운드 (기존)
    public void PlayAttackSound()
    {
        PlaySFX(attackSound);
    }

    // 스킬 효과음 (1~4번)
    public void PlaySkillSound(int skillNum)
    {
        if (skillNum < 1 || skillNum > 4)
        {
            Debug.LogWarning("[AudioManager] 잘못된 스킬 번호입니다 (1~4만 허용)");
            return;
        }
        PlaySFX(skillSounds[skillNum - 1]);
    }

    // 공용 SFX 재생 로직
    private void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip, sfxVolume);
        else
            Debug.LogWarning("[AudioManager] 효과음 AudioClip이 없습니다.");
    }
}