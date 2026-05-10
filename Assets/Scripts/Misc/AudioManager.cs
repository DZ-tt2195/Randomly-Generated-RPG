using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using MyBox;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    [Foldout("Play audio", true)]
    public static AudioManager instance;
    AudioSource audioPlayer;
    public AudioMixer mixer;
    [Foldout("Sound effects", true)]
    [SerializeField] AudioClip damageSound; public void Damage(float volume) => PlaySound(damageSound, volume);
    [SerializeField] AudioClip healSound; public void Heal(float volume) => PlaySound(healSound, volume);
    [SerializeField] AudioClip missSound; public void Miss(float volume) => PlaySound(missSound, volume);
    [SerializeField] AudioClip shootSound; public void Shoot(float volume) => PlaySound(shootSound, volume);

    private void Awake()
    {
        if (instance == null)
        {
            audioPlayer = GetComponent<AudioSource>();
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public void PlaySound(AudioClip audio, float volume)
    {
        audioPlayer.PlayOneShot(audio, volume);
    }
}
