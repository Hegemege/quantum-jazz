using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuantumMusicManager : MonoBehaviour
{

    public AudioClip[] music;
    private List<AudioSource> players;
    public GameObject MusicPlayer;

    // Start is called before the first frame update
    void Start()
    {
        players = new List<AudioSource>();

        foreach(AudioClip a in music){
            AudioSource source = Instantiate(MusicPlayer).GetComponent<AudioSource>();
            source.clip = a;
            source.Play();
            //source.volume = 0;
            players.Add(source);
        }   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MixInstruments(float right, float mid, float left){
        float largest = Mathf.Max(right,mid,left);

        mid = mid/largest;
        right = right/largest;
        left = left/largest;

        players[0].volume = mid;
        players[1].volume = mid;
        players[2].volume = mid;
        players[3].volume = left;
        players[4].volume = left;
        players[5].volume = right;
        players[6].volume = right;

    }
}
