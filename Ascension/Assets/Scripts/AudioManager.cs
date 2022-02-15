using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager InstanceAudioManager { get; private set; }

    public AudioSource[] audioSources;
    public List<AudioClip> audioClips;
    public AudioClip[] SFXClips;

    private AudioSource audioBackground;
    private AudioSource audioSFX;
    private AudioClip clip;
    public Scrollbar soundScrollbar;

    public int audioClipNumber;
    public int audioNumber;
    private bool newInstance = false;
    private float clipTime;
    public bool pause { get; set; }
    public float volume { get; set; }

    /// <summary>
    /// Called before the first frame.
    /// Gets the information from the other AudioManager Instance if it exists,
    /// starts the first clip if not
    /// </summary>
    void Start()
    {
        // If an Instance of AudioManager already exists
        if (InstanceAudioManager != null)
        {
            // Gets the information of the former AudioManager
            clipTime = InstanceAudioManager.audioBackground.time;
            clip = InstanceAudioManager.audioBackground.clip;
            volume = InstanceAudioManager.audioBackground.volume;
            pause = InstanceAudioManager.pause;
            audioClipNumber = InstanceAudioManager.audioClipNumber;

            // Destroy the former AudioManager
            Destroy(InstanceAudioManager.gameObject);

            newInstance = true;
        }
        // Prevent the Instance to be destroyed on load
        InstanceAudioManager = this;
        DontDestroyOnLoad(gameObject);

        // Gets the audio sources child and prepare it
        audioSources = GetComponentsInChildren<AudioSource>();

        audioBackground = audioSources[0];
        audioBackground.clip = audioClips[audioClipNumber];
        audioNumber = audioClips.Count;

        audioSFX = audioSources[1];

        // If it's the first Instance, play the first clip
        if (!newInstance)
        {
            audioBackground.Play();
            audioClipNumber = 0;
            volume = audioBackground.volume;
        }
        // If not, put the information of the former AudioManager into the new one
        else
        {
            audioBackground.time = clipTime;
            audioBackground.clip = clip;
            audioBackground.volume = volume;
            soundScrollbar.value = volume;
            audioBackground.Play();
            if (pause) audioBackground.Pause();
        }
    }

    /// <summary>
    /// Start the currently chosen clip at the beginning
    /// </summary>
    private void StartClip()
    {
        audioBackground.clip = audioClips[audioClipNumber];
        audioBackground.time = 0f;
        audioBackground.Play();
        pause = false;
    }

    /// <summary>
    /// Play the next clip on the clip list
    /// </summary>
    public void NextClip()
    {
        if (audioClipNumber < audioNumber-1) audioClipNumber++;
        else audioClipNumber = 0;
        StartClip();
    }

    /// <summary>
    /// Play the previous clip on the clip list
    /// </summary>
    public void PreviousClip()
    {
        if (audioClipNumber > 0) audioClipNumber--;
        else audioClipNumber = audioNumber - 1;
        StartClip();
    }

    /// <summary>
    /// Pause the clip currently playing
    /// </summary>
   //public void Pause()
   //{
   //    audioBackground.Pause();
   //    pause = true;
   //}

    /// <summary>
    /// Unpause the clip currently playing
    /// </summary>
   //public void UnPause()
   //{
   //    audioBackground.UnPause();
   //    pause = false;
   //}

    /// <summary>
    /// Called every 0.02s
    /// If no clip is playing and the clip ins't on pause, play the next clip
    /// </summary>
    private void FixedUpdate()
    {
        if (!audioBackground.isPlaying && !pause) NextClip();
    }


    //public void ActuScrollbar()
    //{
    //    volume = audioBackground.volume;
    //    soundScrollbar.value = volume;
    //}

    /// <summary>
    /// Play the UI's touch sound
    /// </summary>
    public void PlayUITouch()
    {
        audioSFX.clip = SFXClips[0];
        audioSFX.time = 0f;
        audioSFX.volume = 0.25f;
        audioSFX.Play();
    }

    /// <summary>
    /// Play the select tile sound
    /// </summary>
    public void PlaySelectTile()
    {
        if (audioSFX.clip != SFXClips[1]) audioSFX.clip = SFXClips[1];
        audioSFX.time = 0.1f;
        audioSFX.volume = 0.5f;
        audioSFX.Play();
    }

    /// <summary>
    /// Play the earthquake sound
    /// </summary>
    public void PlayEarthquake()
    {
        audioSFX.clip = SFXClips[2];
        audioSFX.time = 0f;
        audioSFX.volume = 0.5f;
        audioSFX.Play();
        //StartCoroutine(WaitToStop(1.5f));
    }

    IEnumerator WaitToStop(float t)
    {
        yield return new WaitForSeconds(t);
        audioSFX.Stop();
    }
}
