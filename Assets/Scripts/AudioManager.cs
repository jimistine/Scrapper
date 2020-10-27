using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Yarn.Unity;


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
    public List<AudioClip> rigHitClips;
    public AudioClip rigStart;
    public AudioClip rigStop;
    public float rigHitVolumeModifier;
    [Range(0.25f, 5f)]
    public float hitModulationMin;
    [Range(0.25f, 5f)]
    public float hitModulationMax;
    [Header("Audio Sources")]
    [Space(5)]
    public AudioSource UIAudio;
    public AudioSource PlayerAudio;
    public AudioSource VoiceAudio;
   // public AudioSource AmbientAudio;
    public AudioSource AmbientAudioSting;
    public AudioSource AmbientTown;
    public AudioSource Music;
    public AudioSource RigStartStop;
    public AudioSource RigRunning;
    public AudioSource RigHit;
    [Header("Snapshots")]
    [Space(5)]
    public AudioMixerSnapshot currentSnapShot;
    public AudioMixerSnapshot townExterior;
    public AudioMixerSnapshot townInterior;
    public AudioMixerSnapshot overworldSnapshot;
    public AudioMixerSnapshot pausedSnapshot;
    [Header("Other")]
    [Space(5)]
    public float startRunningAdjustment;
    public float runningVolumeModifier;
    [Header("Voices")]
    [Space(5)]
    [Range(0.5f, 1.5f)]
    public float voiceModulationMin;
    [Range(0.5f, 1.5f)]
    public float voiceModulationMax;
    public int voiceClipRate;
    public AudioClip CH1PVoice;
    public AudioClip hasronVoice;
    public AudioClip chundrVoice;
    public AudioClip ogdenVoice;



    int secondsToWait;
    float startWaitTime;
    float currentTime;
    int secondsToWaitMusic;
    float startWaitTimeMusic;
    float currentTimeMusic;

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
        secondsToWaitMusic = Random.Range(180, 420);
        startWaitTimeMusic = Time.time;
        
    }

    void Update(){
        //masterMixer.SetFloat("RunningVolume", (PlayerManager.PM.GetComponent<ClickDrag>().currentSpeedActual));
        //masterMixer.SetFloat("RunningVolume", (PlayerManager.PM.GetComponent<ClickDrag>().currentSpeedActual * runningVolumeModifier) - 80);
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
        if(Time.time - startWaitTimeMusic >= secondsToWaitMusic && Director.Dir.ogdenVisited){
            PlayRandomSong();
            secondsToWait = Random.Range(180, 420);
            startWaitTimeMusic = Time.time;
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
    public void ToggleHeadlights(){
        PlayerAudio.PlayOneShot(playerClips.Find(x => x.clipName == "toggle headlights").clip, 1);
    }

    int voiceCounter;
    public void PlayVoiceClip(){
        if(voiceCounter % voiceClipRate == 0){
            VoiceAudio.pitch = Random.Range(voiceModulationMin, voiceModulationMax);
            VoiceAudio.PlayOneShot(DialogueManager.DM.characters.Find(x => x.characterName == ScrapDialogueUI.sDUI.speakerName).voiceClip, 1);
        }
        voiceCounter++;
    }

// RIG
    public void PlayRigStart(){
        //Debug.Log("rig start");
        double startDuration = (double)RigStartStop.clip.samples / RigStartStop.clip.frequency;
        RigStartStop.PlayScheduled(AudioSettings.dspTime);
        RigRunning.PlayScheduled(AudioSettings.dspTime + startDuration + startRunningAdjustment);
        //RigStartStop.PlayOneShot(rigStart);
    }
    public void PlayRigRunning(){
    }
    public void PlayRigStop(){
        RigStartStop.Stop();
        RigRunning.Stop();
        RigStartStop.PlayOneShot(rigStop);
        //Debug.Log("Playing Stop");
    }
    public void PlayRandomRigHit(){
        AudioClip clip = rigHitClips[Random.Range(0, rigHitClips.Count)];
        RigHit.pitch = Random.Range(hitModulationMin, hitModulationMax);
        RigHit.volume = PlayerManager.PM.GetComponent<ClickDrag>().currentSpeedActual * rigHitVolumeModifier;
        RigHit.PlayOneShot(clip);
    }


// AMBIENT
    // Snapshot Transitions
    public void TransitionToTownExterior(){
        if(AmbientTown.isPlaying == false){
            AmbientTown.Play();
        }
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
    public void PlayRandomSong(){
        int clipToPlayMusic = Random.Range(0, musicClips.Count);
        Music.PlayOneShot(musicClips[clipToPlayMusic]);
        Debug.Log("Playing track: " + musicClips[clipToPlayMusic].name);
    }

}
