using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager AM;

    [Header("Audio Clips")]
    [Space(5)]
    public List<AudioClip> buttonClips;
    public List<AudioClip> upgradeClips;
    public List<NamedClip> miscUIClips;
    public List<NamedClip> playerClips;
    public List<AudioClip> ambientClips;
    public List<AudioClip> musicClips;
    [Header("Audio Sources")]
    [Space(5)]
    public AudioSource UIAudio;
    public AudioSource PlayerAudio;

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
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayRandomButton(){
        int clipToPlay = Random.Range(0, buttonClips.Count);
        UIAudio.PlayOneShot(buttonClips[clipToPlay]);
    }
    public void PlayRandomUpgrade(){
        int clipToPlay = Random.Range(0, upgradeClips.Count);
        UIAudio.PlayOneShot(upgradeClips[clipToPlay]);
    }
    public void ChangeZoom(){
        PlayerAudio.PlayOneShot(playerClips.Find(x => x.clipName == "change zoom").clip);
    }
    public void PlayPlayerClip(string clipName){
        PlayerAudio.PlayOneShot(playerClips.Find(x => x.clipName == clipName).clip);
    }
    public void OnHover(){
        UIAudio.PlayOneShot(miscUIClips.Find(x => x.clipName == "on hover").clip);
    }
    public void FillFuel(){
        UIAudio.PlayOneShot(miscUIClips.Find(x => x.clipName == "fill fuel").clip, 1);
    }

    public void PlayMiscUIClip(string clipName){
        UIAudio.PlayOneShot(miscUIClips.Find(x => x.clipName == clipName).clip);
    }
}
