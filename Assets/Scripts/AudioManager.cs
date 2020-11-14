using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Yarn.Unity;


public class AudioManager : MonoBehaviour
{

    public static AudioManager AM;
    public DayNight DayNight;

    public AudioMixer masterMixer;
    [Header("Audio Clips")]
    [Space(5)]
    public List<AudioClip> buttonClips;
    public List<AudioClip> upgradeClips;
    public List<NamedClip> miscUIClips;
    public List<NamedClip> playerClips;
    public List<AudioClip> ambientStings;
    public List<AudioClip> ambientStingsNight;
    public List<AudioClip> musicClips;
    public List<AudioClip> rigHitClips;
    public AudioClip rigStart;
    public AudioClip rigStop;
    public AudioClip towRigStop;
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
    public AudioSource AmbientNight;
    public AudioSource Music;
    public AudioSource RigStartStop;
    public AudioSource RigRunning;
    public AudioSource RigHit;
    public AudioSource TowRigStartStop;
    public AudioSource TowRigRunning;
    [Header("Snapshots")]
    [Space(5)]
    public AudioMixerSnapshot currentSnapShot;
    public AudioMixerSnapshot townExterior;
    public AudioMixerSnapshot townInterior;
    public AudioMixerSnapshot overworldSnapshot;
    public AudioMixerSnapshot nightAmbientSnapshot;
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
    public float startWaitTimeMusic;
    public float currentTimeMusic;
    public float musicTimer;

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
        overworldSnapshot.TransitionTo(3);
        secondsToWait = Random.Range(90, 180);
        startWaitTime = Time.time;
        secondsToWaitMusic = 60; //420
        startWaitTimeMusic = Time.time;
        
    }

    void Update(){
        //masterMixer.SetFloat("RunningVolume", (PlayerManager.PM.GetComponent<ClickDrag>().currentSpeedActual));
        //masterMixer.SetFloat("RunningVolume", (PlayerManager.PM.GetComponent<ClickDrag>().currentSpeedActual * runningVolumeModifier) - 80);
    }

    public void InitTowTigAudio(){
        TowRigStartStop = GameObject.Find("TowRigParent").GetComponentsInChildren<AudioSource>()[0];
        TowRigRunning = GameObject.Find("TowRigParent").GetComponentsInChildren<AudioSource>()[1];
        OverworldManager.OM.towRig.SetActive(false);
    }
    void FixedUpdate()
    {
        currentTime = Time.time;
        // Ambient Audio Controller
        if(Time.time - startWaitTime >= secondsToWait){                 // Ambient Timer
            if(DayNight.isDay){
                PlayRandomAmbient();
            }
            else if(DayNight.isNight){
                PlayRandomAmbientNight();
            }
            secondsToWait = Random.Range(90, 180);
            startWaitTime = Time.time;
        }
        if ((Director.Dir.ogdenVisited || Director.Dir.chundrVisited) && UIManager.UIM.playerLocation == "overworld"){   // Musical Timer
            musicTimer += Time.deltaTime;
            if(musicTimer - startWaitTimeMusic >= secondsToWaitMusic){
                PlayRandomSong();
                secondsToWait = Random.Range(360, 720);
                startWaitTimeMusic = Time.time;
                Debug.Log("Playing random song"); 
            }
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

// RIGs
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
    public void PlayTowRigStart(){
        double startDuration = (double)TowRigStartStop.clip.samples / TowRigStartStop.clip.frequency;
        TowRigStartStop.PlayScheduled(AudioSettings.dspTime);
        TowRigRunning.PlayScheduled(AudioSettings.dspTime + startDuration);
    }
    public void PlayTowRigStop(){
        TowRigStartStop.Stop();
        TowRigRunning.Stop();
        TowRigStartStop.PlayOneShot(towRigStop);
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
        if(DayNight.isDay){
            overworldSnapshot.TransitionTo(10);
            currentSnapShot = overworldSnapshot;
        }
        else if(DayNight.isNight){
            nightAmbientSnapshot.TransitionTo(10);
            currentSnapShot = nightAmbientSnapshot;
        }
    }

    bool paused = false;
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
        AmbientAudioSting.PlayOneShot(ambientStings[clipToPlay]);
        Debug.Log("Playing random day sting: " + ambientStings[clipToPlay].name);
    }
    public void PlayRandomAmbientNight(){
        int clipToPlay = Random.Range(0, ambientStingsNight.Count);
        AmbientAudioSting.PlayOneShot(ambientStingsNight[clipToPlay]);
        Debug.Log("Playing random night sting: " + ambientStingsNight[clipToPlay].name);
    }
    public void DayNightTransitions(string snapshotToSet){
        StartCoroutine(DayNightTransitionsRoutine(snapshotToSet));
    }
    public IEnumerator DayNightTransitionsRoutine(string snapshotToSet){
        Debug.Log("Transitioning to " + snapshotToSet);
        if(snapshotToSet == "day"){
            overworldSnapshot.TransitionTo(DayNight.transitionLength);
            yield return new WaitForSeconds(DayNight.transitionLength + 2);
            AmbientNight.Stop();
        }
        if(snapshotToSet == "night"){
            AmbientNight.Play();
            nightAmbientSnapshot.TransitionTo(DayNight.transitionLength);
        }
    }


// MUSIC
    public void PlayRandomSong(){
        int clipToPlayMusic = Random.Range(0, musicClips.Count);
        Music.PlayOneShot(musicClips[clipToPlayMusic]);
        Debug.Log("Playing track: " + musicClips[clipToPlayMusic].name + "  ||  Next song in " + secondsToWaitMusic/60 + " minutes.");
    }

}
