using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuantumMusicManager : MonoBehaviour
{

    private static QuantumMusicManager m_instance;

    //public QuantumMusicManager music;

    public static QuantumMusicManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<QuantumMusicManager>();
            }

            return m_instance;
        }
    }

    public AudioClip[] energeticMusic;
    public AudioClip[] chillMusic;
    private List<AudioSource> energeticPlayers;
    private List<AudioSource> chillPlayers;
    public GameObject MusicPlayer;
    public bool energetic = false;

    private int swapCounter = 0;

    private float energeticMixMultiplier = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        energeticPlayers = new List<AudioSource>();
        chillPlayers = new List<AudioSource>();

        foreach (AudioClip a in energeticMusic)
        {
            AudioSource source = Instantiate(MusicPlayer).GetComponent<AudioSource>();
            source.clip = a;
            //source.Play();
            //source.volume = 0;
            energeticPlayers.Add(source);
        }
        //energeticPlayers.Shuffle();
        foreach (AudioClip a in chillMusic)
        {
            AudioSource source = Instantiate(MusicPlayer).GetComponent<AudioSource>();
            source.clip = a;
            //source.volume = 0;
            chillPlayers.Add(source);
        }
        ResetMusic();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            SwapMusic();
    }

    // Update is called once per frame
    public void SwapMusic()
    {
        swapCounter++;
        if (swapCounter % 3 == 0)
        {
            StartCoroutine(SwapMusicWait());
        }
    }

    IEnumerator SwapMusicWait()
    {
        List<AudioSource> fadeout;
        if (energetic)
            fadeout = energeticPlayers;
        else
            fadeout = chillPlayers;

        if (fadeout == null)
        {
            yield break;
        }

        while (fadeout[0].volume > 0)
        {
            foreach (AudioSource source in fadeout)
            {
                source.volume = source.volume - 0.01f;
                //print(source.volume);
            }
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        energetic = !energetic;

        ResetMusic();

    }

    public void ResetMusic()
    {
        MixInstruments(1, 0, 0);

        if (energetic)
        {
            foreach (AudioSource source in energeticPlayers)
            {
                if (!source.isPlaying)
                {
                    source.Play();
                }
            }
            foreach (AudioSource source in chillPlayers)
            {
                if (source.isPlaying)
                    source.Stop();
            }
        }
        else
        {
            foreach (AudioSource source in chillPlayers)
            {
                if (!source.isPlaying)
                    source.Play();
            }
            foreach (AudioSource source in energeticPlayers)
            {
                if (source.isPlaying)
                    source.Stop();
            }
        }
    }

    public void EndMix()
    {
        if (energetic)
        {
            foreach (AudioSource source in energeticPlayers)
            {
                source.volume = 1 * energeticMixMultiplier;
            }
        }
    }

    public void MixInstruments(float left, float mid, float right)
    {
        if (energetic)
        {
            float largest = Mathf.Max(right, mid, left);

            if (largest != 0)
            {
                mid = Mathf.Clamp(mid / largest, 0, 1) * energeticMixMultiplier;
                right = Mathf.Clamp(right / largest, 0, 1) * energeticMixMultiplier;
                left = Mathf.Clamp(left / largest, 0, 1) * energeticMixMultiplier;
            }

            energeticPlayers[1].volume = mid;
            energeticPlayers[3].volume = mid;
            energeticPlayers[4].volume = 1 * energeticMixMultiplier;
            energeticPlayers[5].volume = left;
            energeticPlayers[0].volume = right;
            energeticPlayers[2].volume = right;
        }
        else
        {
            float largest = Mathf.Max(right, left);
            if (largest != 0)
            {
                right = Mathf.Clamp(right / largest, 0, 1);
                left = Mathf.Clamp(left / largest, 0, 1);
            }
            float clarinetQuieter = 0.6f;
            chillPlayers[0].volume = left;
            chillPlayers[1].volume = right;
            chillPlayers[2].volume = left;
            chillPlayers[3].volume = right;
            chillPlayers[4].volume = left * clarinetQuieter;
            chillPlayers[5].volume = right * clarinetQuieter;
        }

    }
}
