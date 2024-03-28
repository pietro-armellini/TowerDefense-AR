using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	//List of all the game clips
    public List<AudioClip> clips = new List<AudioClip>();
    //the audio source
    public AudioSource source;

	void Start()
    {
        //play the background music
        //source.clip = clips[6];
        //source.Play();
    }
    
    //function for play a specific sound
    public void playClip(int id){
        AudioClip clip = clips[id];
        source.PlayOneShot(clip, 1);
    }

	//override function to play a specific sound but from a different audiosource from the main one
	//main audiosource is used for sound like: game start and round finished
	//other audiosources can be placed on the defender for playing the the arrow draw sound
	public void playClip(int id, AudioSource audioSource){
        AudioClip clip = clips[id];
        audioSource.PlayOneShot(clip, 1);
    }
}
