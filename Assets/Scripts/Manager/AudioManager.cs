using UnityEngine.Audio;
using System;
using UnityEngine;

using DEMO.GamePlay;

namespace DEMO.Manager
{
    public class AudioManager : MonoBehaviour
    {
        public Sound[] sounds;

        public static AudioManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            DontDestroyOnLoad(gameObject);

            // Initialize audio
            foreach(Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;

                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
            }
        }

        private void Start()
        {
            Play("Theme");
        }

        public void Play(string audioName)
        {
            Sound s = Array.Find(sounds, sound => sound.name == audioName);
            s.source.Play();
        }
    }
}
