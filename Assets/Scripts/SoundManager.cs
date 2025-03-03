using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class SoundManager : MonoBehaviour
{
    public enum SoundType
    {
        StackPlace
    }

    [Serializable]
    public class SoundDefinition
    {
        [HideInInspector] public string Name;
        public SoundType Type;
        public List<AudioClip> Clips;
        public AudioMixerGroup Mixer;
    }

    [SerializeField, BoxGroup("Settings")] private float defaultPitch = 1f;
    [SerializeField, BoxGroup("Settings")] private float maxPitch = 2f;
    [SerializeField, BoxGroup("Settings")] private float pitchIncrease = .1f;

    [SerializeField, Foldout("Setup")] private AudioSource audioSource;

    [SerializeField] private List<SoundDefinition> allSounds;

    private Dictionary<SoundType, SoundDefinition> soundDictionary;

    private float currentPitch;

    [Inject]
    private void Construct()
    {
        soundDictionary = new Dictionary<SoundType, SoundDefinition>();
        foreach (var sound in allSounds)
        {
            soundDictionary.TryAdd(sound.Type, sound);
        }

        currentPitch = defaultPitch;
    }

    public void PlaySound(SoundType type, bool isPerfect)
    {
        if (soundDictionary.TryGetValue(type, out var soundDef))
        {
            var clip = soundDef.Clips[UnityEngine.Random.Range(0, soundDef.Clips.Count)];
            audioSource.outputAudioMixerGroup = soundDef.Mixer;

            currentPitch = isPerfect ? Mathf.Min(currentPitch + pitchIncrease, maxPitch) : defaultPitch;

            audioSource.pitch = currentPitch;
            audioSource.PlayOneShot(clip);
        }
    }
}