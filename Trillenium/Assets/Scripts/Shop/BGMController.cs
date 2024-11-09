using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour
{
    #region Variables
    private AudioSource source; // The game object where the audio is played from.

    [SerializeField] private AudioClip clipIntro; // Audio clip of the intro part of the track.
    [SerializeField] private AudioClip clipLoop; // // Audio clip of the looped part of the track.
    #endregion

    void Start()
    {
        source = this.GetComponent<AudioSource>();

        source.clip = clipIntro; // Initialize with intro,
        source.loop = false; // Turn loop off.
        source.Play(); // Begin playback.

        // Start the coroutine to check if the audio playback is active.
        StartCoroutine(CheckPlaybackTime());
    }

    // Checks playback and will set looped version of track upon reaching the playback limit.
    private System.Collections.IEnumerator CheckPlaybackTime()
    {
        // Ensures that the source is always playing.
        if (!source.isPlaying)
        {
            source.Play();
        }

        // If the playback time hasn't reached the loop point (I set it to slightly before so that it would work better), then skip the bottom snippet of code.
        while (source.time < clipIntro.length - 0.05f)
        {
            yield return null; // This is where we essentially leave the coroutine for this frame, which skips the bottom snippet.
        }

        // This part of the coroutine hasn't been skipped over, meaning we've reached our playback time limit.
        // Change audio clip to the looped version of the track, enable looping, and enable playback.
        source.clip = clipLoop;
        source.loop = true;
        source.Play();
    }
}
