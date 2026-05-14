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
    [SerializeField] AudioClip damageSound; public void Damage(float volume = 0.3f) => PlaySound(damageSound, volume);
    [SerializeField] AudioClip healSound; public void Heal(float volume = 0.3f) => PlaySound(healSound, volume);
    [SerializeField] AudioClip menuSound; public void Menu(float volume = 0.3f) => PlaySound(menuSound, volume);
    [SerializeField] AudioClip deadSound; public void Dead(float volume = 0.3f) => PlaySound(deadSound, volume);
    [SerializeField] AudioClip changeSound; public void Change(float volume = 0.3f) => PlaySound(changeSound, volume);
    [SerializeField] AudioClip buffSound; public void Buff(float volume = 0.3f) => PlaySound(buffSound, volume);
    [SerializeField] AudioClip nerfSound; public void Nerf(float volume = 0.3f) => PlaySound(nerfSound, volume);
    [SerializeField] AudioClip blockSound; public void Blocked(float volume = 0.3f) => PlaySound(blockSound, volume);

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
    public void PlaySound(AudioClip audio, float volume = 0.3f)
    {
        audioPlayer.PlayOneShot(audio, volume);
    }
}
