using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioClipList
{
    Match, Bomb, Swap
}

public class MatchThreeAudio : MonoBehaviour
{
    [Header("Sound Effects")]
    public List<AudioClip> clips = new List<AudioClip>();
    public int channels = 10;
    public float volume = 1.0f;
    public Vector2 pitchRange = new Vector2(1.0f, 1.0f);

    private List<AudioSource> Sources = new List<AudioSource>();
    private int index = 0;

    private void Awake()
    {
        for (int i = 0; i < channels; i++)
        {
            AudioSource AS = gameObject.AddComponent<AudioSource>();

            AS.loop = false;
            AS.playOnAwake = false;
            AS.volume = volume;

            Sources.Add(AS);
        }
    }

    private void OnEnable()
    {
        MatchThreeEvents.AddScore += PlayMatchAudio;
        MatchThreeEvents.BombTriggered += PlayBombAudio;
    }

    private void OnDisable()
    {
        MatchThreeEvents.AddScore -= PlayMatchAudio;
        MatchThreeEvents.BombTriggered -= PlayBombAudio;
    }


    /// Functions ///

    private void PlayMatchAudio(int _)
    {
        PlayAudio(AudioClipList.Match);
    }

    private void PlayBombAudio()
    {
        PlayAudio(AudioClipList.Bomb);
    }

    private void PlayAudio(AudioClipList clip)
    {
        if (clips == null || (int)clip >= clips.Count) return;

        AudioSource AS = GetAudioSource();

        AS.Stop();
        AS.pitch = Random.Range(pitchRange.x, pitchRange.y);

        AS.clip = clips[(int)clip];
        AS.Play();
    }

    private AudioSource GetAudioSource()
    {
        if (++index >= Sources.Count) index = 0;

        return Sources[index];
    }

}
