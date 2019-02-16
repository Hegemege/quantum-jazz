using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuantumMusicManager : MonoBehaviour
{

    public AudioClip[] energeticMusic;
    public AudioClip[] chillMusic;
    private List<AudioSource> energeticPlayers;
    private List<AudioSource> chillPlayers;
    public GameObject MusicPlayer;
    public bool energetic = false;

    // Start is called before the first frame update
    void Start()
    {
        energeticPlayers = new List<AudioSource>();
        chillPlayers = new List<AudioSource>();

        foreach(AudioClip a in energeticMusic){
            AudioSource source = Instantiate(MusicPlayer).GetComponent<AudioSource>();
            source.clip = a;
            //source.Play();
            //source.volume = 0;
            energeticPlayers.Add(source);
        }   
        foreach(AudioClip a in chillMusic){
            AudioSource source = Instantiate(MusicPlayer).GetComponent<AudioSource>();
            source.clip = a;
            //source.volume = 0;
            chillPlayers.Add(source);
        }   
        ResetMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetMusic(){
        MixInstruments(1, 0, 0);
        if(energetic){
            foreach(AudioSource source in energeticPlayers){
                if(!source.isPlaying){
                    source.Play();
                }
            }
            foreach(AudioSource source in chillPlayers){
                if(source.isPlaying)
                    source.Stop();
            }
        } else {
            foreach(AudioSource source in chillPlayers){
                if(!source.isPlaying)
                    source.Play();
            }
            foreach(AudioSource source in energeticPlayers){
                if(source.isPlaying)
                    source.Stop();
            }
        }
    }

    public void EndMix(){
        if(energetic){
            foreach(AudioSource source in energeticPlayers){
                source.volume = 1;
            }
        }
    }

    public void MixInstruments(float right, float mid, float left){
        if(energetic){
            float largest = Mathf.Max(right,mid,left);

            if(largest != 0){
                mid = Mathf.Clamp(mid/largest,0,1);
                right = Mathf.Clamp(right/largest,0,1);
                left = Mathf.Clamp(left/largest,0,1);
            }

            energeticPlayers[0].volume = mid;
            energeticPlayers[1].volume = mid;
            energeticPlayers[2].volume = mid;
            energeticPlayers[3].volume = left;
            energeticPlayers[4].volume = left;
            energeticPlayers[5].volume = right;
            energeticPlayers[6].volume = right;
        } else {
            float largest = Mathf.Max(right,left);
            if(largest != 0){
                right = Mathf.Clamp(right/largest,0,1);
                left = Mathf.Clamp(left/largest,0,1);
            }

            chillPlayers[0].volume = left;
            chillPlayers[1].volume = right;
            chillPlayers[2].volume = left;
            chillPlayers[3].volume = right;
        }
        


    }
}
