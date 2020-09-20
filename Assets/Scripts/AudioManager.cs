using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    public static AudioManager AM;

    public AudioMixer masterMixer;
    [Header("Audio Clips")]
    [Space(5)]
    public List<AudioClip> buttonClips;
    public List<AudioClip> upgradeClips;
    public List<NamedClip> miscUIClips;
    public List<NamedClip> playerClips;
    public List<AudioClip> ambientStings;
    public List<AudioClip> musicClips;
    public AudioClip rigStart;
    public AudioClip rigStop;
    [Header("Audio Sources")]
    [Space(5)]
    public AudioSource UIAudio;
    public AudioSource PlayerAudio;
   // public AudioSource AmbientAudio;
    public AudioSource AmbientAudioSting;
    public AudioSource AmbientTown;
    public AudioSource RigStartStop;
    public AudioSource RigRunning;
    [Header("Snapshots")]
    [Space(5)]
    public AudioMixerSnapshot currentSnapShot;
    public AudioMixerSnapshot townExterior;
    public AudioMixerSnapshot townInterior;
    public AudioMixerSnapshot overworldSnapshot;
    public AudioMixerSnapshot pausedSnapshot;

    int secondsToWait;
    float startWaitTime;
    float currentTime;

    [System.Serializable]
    public struct NamedClip{
        public string clipName;
        public AudioClip clip;
    }

    void Awake(){
        AM = this;
    }

    // Start is called before the first frame update
    void Start()
    {   // Initializing Ambient Audio Controller
        secondsToWait = Random.Range(90, 180);
        startWaitTime = Time.time;
        
    }

    void FixedUpdate()
    {
        currentTime = Time.time;
        // Ambient Audio Controller
        if(Time.time - startWaitTime >= secondsToWait){
            PlayRandomAmbient();
            secondsToWait = Random.Range(90, 180);
            startWaitTime = Time.time;
        }
    }

// UI
    public void PlayMiscUIClip(string clipName){
        UIAudio.PlayOneShot(miscUIClips.Find(x => x.clipName == clipName).clip);
    }
    public void PlayRandomButton(){
        int clipToPlay = Random.Range(0, buttonClips.Count);
        UIAudio.PlayOneShot(buttonClips[clipToPlay]);
    }
    public void PlayRandomUpgrade(){
        int clipToPlay = Random.Range(0, upgradeClips.Count);
        UIAudio.PlayOneShot(upgradeClips[clipToPlay]);
    }
    public void OnHover(){
        UIAudio.PlayOneShot(miscUIClips.Find(x => x.clipName == "on hover").clip);
    }

// PLAYER
    public void ChangeZoom(){
        PlayerAudio.PlayOneShot(playerClips.Find(x => x.clipName == "change zoom").clip);
    }
    public void PlayPlayerClip(string clipName){
        PlayerAudio.PlayOneShot(playerClips.Find(x => x.clipName == clipName).clip);
    }
    public void FillFuel(){
        UIAudio.PlayOneShot(miscUIClips.Find(x => x.clipName == "fill fuel").clip, 1);
    }

// RIG
    public void PlayRigStart(){
        double startDuration = (double)RigStartStop.clip.samples / RigStartStop.clip.frequency;
        RigStartStop.PlayScheduled(AudioSettings.dspTime + 0.1);
        RigRunning.PlayScheduled(AudioSettings.dspTime + 0.1 + startDuration);
        //RigStartStop.PlayOneShot(rigStart);
    }
    public void PlayRigRunning(){
    }
    public void PlayRigStop(){
        RigStartStop.PlayOneShot(rigStop);
        RigRunning.Stop();
        Debug.Log("Playing Stop");
    }


// AMBIENT
    // Snapshot Transitions
    public void TransitionToTownExterior(){
        AmbientTown.Play();
        townExterior.TransitionTo(0.25f);
        currentSnapShot = townExterior;
    }
    public void TransitionToTownInterior(){
        townInterior.TransitionTo(0.25f);
        currentSnapShot = townInterior;
    }
    public void TransitionToOverworld(){
        AmbientTown.SetScheduledEndTime(10);
        overworldSnapshot.TransitionTo(10);
        currentSnapShot = overworldSnapshot;
    }

    bool paused = true;
    AudioMixerSnapshot storedSnapshot;
    public void TogglePausedSnapshot(){
        if(paused){
            overworldSnapshot.TransitionTo(0.5f);
            paused = false;
        }
        else{
            storedSnapshot = currentSnapShot;
            pausedSnapshot.TransitionTo(0.25f);
            paused = true;
        }
    }



    public void PlayRandomAmbient(){
        int clipToPlay = Random.Range(0, ambientStings.Count);
        // while (clipToPlay == clipToPlayLast){
        //     clipToPlay = Random.Range(0, ambientStings.Count);
        // }
        // int clipToPlayLast = clipToPlay;
        AmbientAudioSting.PlayOneShot(ambientStings[clipToPlay]);
        Debug.Log("Playing random sting" + ambientStings[clipToPlay].name);

        // if(playStyle == "oneshot"){
        //     AmbientAudio.PlayOneShot(clipToPlay);
        // }
        // else if(playStyle == "loop"){
        //     AmbientAudio.loop = true;
        //     AmbientAudio.clip = clipToPlay;
        //     AmbientAudio.Play(0);
        // }
        
    }

}
