using System.Collections;
using System.Collections.Generic;
using jn;
using TMPro;
using UnityEngine;

public class RadioPlayManager : MonoBehaviour
{
    public AudioSource audioSource;
    public RadioPlayData radioPlayData;
    public TextMeshProUGUI subtitleTextField;

    public void OnPlayButton()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.Play();
        }
    }

   

    public void StartAudio()
    {
        audioSource.clip = radioPlayData.audio;
        audioSource.Play();
        StartCoroutine(WhileAudioIsPlaying());
    }

    void PauseAduio()
    {

    }

    IEnumerator WhileAudioIsPlaying()
    {
        int currentSubtitleIndex = 0;
        bool ended = false;
        while (!ended)
        {
            if(audioSource.time > radioPlayData.radioPlayScenes[currentSubtitleIndex+1].startTime)
            {
                currentSubtitleIndex++;
                if (currentSubtitleIndex > radioPlayData.radioPlayScenes.Length - 1) ended = true;
            }
            subtitleTextField.text = radioPlayData.radioPlayScenes[currentSubtitleIndex].displayText;
            yield return new WaitForEndOfFrame();
        }

        AudioEnded();
       
    }

    public void StopAudio()
    { 
        audioSource.Stop();
        StopAllCoroutines();
    }

    private void AudioEnded()
    {
        GetComponent<AdventureScreen>().ScreenCompleted();
    }
}
