using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAudioClip : MonoBehaviour, IPoolable
{
    public float PitchChange;
    private float _initialPitch;
    public List<AudioClip> Clips;

    private AudioSource _source;

    private bool _played;

    public void ResetState()
    {
        gameObject.SetActive(true);
    }

    public void ReturnToPool()
    {

    }

    void Awake()
    {
        _source = GetComponent<AudioSource>();
        _initialPitch = _source.pitch;
    }

    void Update()
    {
        if (_source.isPlaying)
        {
            return;
        }
        else
        {
            if (_played)
            {
                _played = false;
                _source.Stop();
                gameObject.SetActive(false);
            }
            else
            {
                _source.clip = Clips[Random.Range(0, Clips.Count)];
                _source.pitch = _initialPitch + Random.Range(-PitchChange, PitchChange);
                _source.Play();
                _played = true;
            }
        }
    }
}