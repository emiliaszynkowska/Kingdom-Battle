using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    // Music
    public AudioClip startMusic;
    public AudioClip prologueMusic;
    public AudioClip castleMusic;
    public AudioClip dungeonMusic;
    public AudioClip bossMusic;
    public AudioClip caveAmbience;
    // UI
    public AudioClip clickButton;
    public AudioClip changeItem;
    public AudioClip buyItem;
    public AudioClip speech;
    public AudioClip error;
    public AudioClip play;
    // Effects
    public AudioClip block;
    public AudioClip swordAttack;
    public AudioClip spinAttack;
    public AudioClip groundPound;
    public AudioClip magicAttack;
    public AudioClip magicSpell;
    public AudioClip complete;
    public AudioClip powerup;
    public AudioClip lifeup;
    public AudioClip damage;
    public AudioClip die;
    public AudioClip win;
    public AudioClip jump;
    public AudioClip coin;
    public AudioClip item;
    public AudioClip open;
    public AudioClip monster;

    public void PlayMusic(AudioClip music)
    {
        audioSource.clip = music;
        audioSource.Play();
    }

    public void PlaySound(AudioClip sound)
    {
        audioSource.PlayOneShot(sound);
    }
    
}
