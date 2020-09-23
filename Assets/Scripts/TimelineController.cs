using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineController : MonoBehaviour
{
    public List <PlayableDirector> playableDirectors;
    public List <TimelineAsset> timelines;
    public TimelineAsset dialogueTimeline;
    public List <string> openingLines;
    public TextSwitcherClip currentClip;
    public TextSwitcherTrack currentTrack;
    public TrackAsset hasronDialogue;


    void Start(){

        hasronDialogue = dialogueTimeline.GetOutputTrack(0);
    }

    public void PlayAll(){
        foreach (PlayableDirector playableDirector in playableDirectors){
            playableDirector.Play();
        }
    }

    public void PlayFromTimelines(int index){
        TimelineAsset selectedTimeline;
        selectedTimeline = timelines[index];
        playableDirectors[0].Play(selectedTimeline);
    }

    public void FeedDialogue(){
        // if (playableDirectors[0].playableAsset.outputs.ToString() == "Hasron"
    }
}


