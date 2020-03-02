using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public List<AudioSource> audioList = new List<AudioSource>();

    void Start()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        foreach (AudioSource source in sources)
        {
            audioList.Add(source);
        }
    }
}
