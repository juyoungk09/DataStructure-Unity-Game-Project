// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Audio;
// using System;

// public class AudioManager : MonoBehaviour
// {
//     public static AudioManager Instance { get; private set; }

//     [Header("Audio Sources")]
//     [SerializeField] private AudioSource _musicSource;
//     [SerializeField] private AudioSource _sfxSource;
//     [SerializeField] private AudioSource _uiSource;

//     [Header("Audio Mixer")]
//     [SerializeField] private AudioMixer _audioMixer;

//     [Header("Volume Parameters")]
//     [SerializeField] private string _masterVolumeParam = "MasterVolume";
//     [SerializeField] private string _musicVolumeParam = "MusicVolume";
//     [SerializeField] private string _sfxVolumeParam = "SFXVolume";

//     [Header("Fade Settings")]
//     [SerializeField] private float _musicFadeDuration = 1f;

//     private Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
//     private Coroutine _musicFadeCoroutine;

//     private void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             DontDestroyOnLoad(gameObject);
//             InitializeAudioSources();
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }

//     private void InitializeAudioSources()
//     {
//         if (_musicSource == null)
//         {
//             _musicSource = gameObject.AddComponent<AudioSource>();
//             _musicSource.outputAudioMixerGroup = _audioMixer?.FindMatchingGroups("Music")[0];
//             _musicSource.loop = true;
//         }

//         if (_sfxSource == null)
//         {
//             _sfxSource = gameObject.AddComponent<AudioSource>();
//             _sfxSource.outputAudioMixerGroup = _audioMixer?.FindMatchingGroups("SFX")[0];
//         }

//         if (_uiSource == null)
//         {
//             _uiSource = gameObject.AddComponent<AudioSource>();
//             _uiSource.outputAudioMixerGroup = _audioMixer?.FindMatchingGroups("UI")[0];
//         }
//     }

//     #region Music Controls
//     public void PlayMusic(AudioClip musicClip, bool fade = true)
//     {
//         if (musicClip == null) return;

//         if (_musicSource.isPlaying && _musicSource.clip == musicClip) return;

//         if (_musicFadeCoroutine != null)
//         {
//             StopCoroutine(_musicFadeCoroutine);
//         }

//         if (fade)
//         {
//             _musicFadeCoroutine = StartCoroutine(FadeMusic(musicClip, _musicFadeDuration));
//         }
//         else
//         {
//             _musicSource.Stop();
//             _musicSource.clip = musicClip;
//             _musicSource.Play();
//         }
//     }

//     public void StopMusic(bool fade = true)
//     {
//         if (fade)
//         {
//             if (_musicFadeCoroutine != null)
//             {
//                 StopCoroutine(_musicFadeCoroutine);
//             }
//             _musicFadeCoroutine = StartCoroutine(FadeOutMusic(_musicFadeDuration));
//         }
//         else
//         {
//             _musicSource.Stop();
//         }
//     }

//     public void PauseMusic()
//     {
//         _musicSource.Pause();
//     }

//     public void ResumeMusic()
//     {
//         _musicSource.UnPause();
//     }
//     #endregion

//     #region SFX Controls
//     public void PlaySFX(AudioClip sfxClip, float volumeScale = 1f)
//     {
//         if (sfxClip == null) return;
//         _sfxSource.PlayOneShot(sfxClip, volumeScale);
//     }

//     public void PlaySFX(string clipName, float volumeScale = 1f)
//     {
//         if (_audioClips.TryGetValue(clipName, out AudioClip clip))
//         {
//             _sfxSource.PlayOneShot(clip, volumeScale);
//         }
//         else
//         {
//             Debug.LogWarning($"SFX clip {clipName} not found!");
//         }
//     }
//     #endregion

//     #region UI Sound Controls
//     public void PlayUISound(AudioClip uiClip, float volumeScale = 1f)
//     {
//         if (uiClip == null) return;
//         _uiSource.PlayOneShot(uiClip, volumeScale);
//     }
//     #endregion

//     #region Volume Controls
//     public void SetMasterVolume(float volume)
//     {
//         SetVolume(_masterVolumeParam, volume);
//     }

//     public void SetMusicVolume(float volume)
//     {
//         SetVolume(_musicVolumeParam, volume);
//     }

//     public void SetSFXVolume(float volume)
//     {
//         SetVolume(_sfxVolumeParam, volume);
//     }

//     private void SetVolume(string parameter, float value)
//     {
//         if (_audioMixer != null)
//         {
//             _audioMixer.SetFloat(parameter, Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20);
//         }
//     }
//     #endregion

//     #region Helper Methods
//     private IEnumerator FadeMusic(AudioClip newClip, float fadeDuration)
//     {
//         // Fade out current music
//         yield return FadeOutMusic(fadeDuration / 2);

//         // Change clip and fade in new music
//         _musicSource.Stop();
//         _musicSource.clip = newClip;
//         _musicSource.Play();
        
//         yield return FadeInMusic(fadeDuration / 2);
//     }

//     private IEnumerator FadeOutMusic(float duration)
//     {
//         float startVolume = _musicSource.volume;
//         float time = 0;

//         while (time < duration)
//         {
//             time += Time.deltaTime;
//             _musicSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
//             yield return null;
//         }

//         _musicSource.Stop();
//         _musicSource.volume = startVolume;
//     }

//     private IEnumerator FadeInMusic(float duration)
//     {
//         float startVolume = 0f;
//         _musicSource.volume = 0f;
//         float time = 0;

//         while (time < duration)
//         {
//             time += Time.deltaTime;
//             _musicSource.volume = Mathf.Lerp(startVolume, 1f, time / duration);
//             yield return null;
//         }
//     }

//     public void LoadAudioClips(SerializableDictionary<string, AudioClip> clipsToLoad)
//     {
//         _audioClips.Clear();
//         foreach (var pair in clipsToLoad)
//         {
//             _audioClips[pair.Key] = pair.Value;
//         }
//     }
//     #endregion
// }
