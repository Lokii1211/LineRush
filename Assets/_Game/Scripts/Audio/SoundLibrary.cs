using System.Collections.Generic;
using UnityEngine;
using SerapKeremGameKit._Audio;

namespace _Game.Audio
{
    /// <summary>
    /// Sound library for LineRush.
    /// Generates all procedural sounds at startup and registers them with AudioManager.
    /// Attach this to the same GameObject as AudioManager (or a child).
    /// </summary>
    public class SoundLibrary : MonoBehaviour
    {
        [Header("Sound Generator")]
        [SerializeField] private SoundGenerator _soundGenerator;

        [Header("Sound Keys")]
        [SerializeField] private string[] _soundKeys = new string[]
        {
            "line_move",
            "line_collide",
            "line_complete",
            "stage_win",
            "stage_lose",
            "btn_tap",
            "btn_confirm"
        };

        [Header("Settings")]
        [SerializeField] private float _defaultVolume = 0.7f;
        [SerializeField] private float _defaultPitch = 1f;

        private bool _isInitialized = false;

        private void Awake()
        {
            if (_soundGenerator == null)
            {
                _soundGenerator = GetComponent<SoundGenerator>();
                if (_soundGenerator == null)
                {
                    _soundGenerator = gameObject.AddComponent<SoundGenerator>();
                }
            }
        }

        private void Start()
        {
            Initialize();
        }

        /// <summary>
        /// Generate all sounds and register them with AudioManager.
        /// Safe to call multiple times — will only initialize once.
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized) return;

            if (!AudioManager.IsInitialized)
            {
                Debug.LogWarning("[SoundLibrary] AudioManager not initialized yet. Sounds will be registered when available.");
                return;
            }

            RegisterAllSounds();
            _isInitialized = true;
        }

        private void RegisterAllSounds()
        {
            AudioManager audioManager = AudioManager.Instance;
            if (audioManager == null) return;

            // Access the audio list via reflection or directly add AudioData entries
            // Since AudioManager uses a serialized list, we inject via its public API
            // by creating AudioData entries and adding them to the internal registry
            
            foreach (string key in _soundKeys)
            {
                AudioClip clip = _soundGenerator.GetSound(key);
                if (clip == null)
                {
                    Debug.LogWarning($"[SoundLibrary] Failed to generate sound for key: {key}");
                    continue;
                }

                AudioData data = new AudioData();
                data.Key = key;
                data.Clip = clip;
                data.Volume = GetVolumeForKey(key);
                data.Pitch = _defaultPitch;
                data.Loop = false;
                data.Spatial = false;

                // Register with AudioManager's internal registry
                audioManager.RegisterAudioData(data);
            }

            Debug.Log($"[SoundLibrary] Registered {_soundKeys.Length} procedural sounds.");
        }

        private float GetVolumeForKey(string key)
        {
            switch (key)
            {
                case "line_move":
                    return 0.5f;
                case "line_collide":
                    return 0.7f;
                case "line_complete":
                    return 0.6f;
                case "stage_win":
                    return 0.8f;
                case "stage_lose":
                    return 0.7f;
                case "btn_tap":
                    return 0.4f;
                case "btn_confirm":
                    return 0.5f;
                default:
                    return _defaultVolume;
            }
        }
    }
}
