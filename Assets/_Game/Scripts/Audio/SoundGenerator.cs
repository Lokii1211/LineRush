using System.Collections.Generic;
using UnityEngine;

namespace _Game.Audio
{
    /// <summary>
    /// Procedural sound generator for LineRush.
    /// Creates unique audio clips at runtime using waveform synthesis.
    /// All sounds are completely original — no copyright concerns.
    /// </summary>
    public class SoundGenerator : MonoBehaviour
    {
        private const int SampleRate = 44100;
        private readonly Dictionary<string, AudioClip> _cache = new Dictionary<string, AudioClip>();

        /// <summary>
        /// Get or create a procedural sound by key.
        /// Results are cached so each sound is only generated once.
        /// </summary>
        public AudioClip GetSound(string key)
        {
            if (_cache.TryGetValue(key, out AudioClip clip))
                return clip;

            clip = GenerateSound(key);
            if (clip != null)
                _cache[key] = clip;

            return clip;
        }

        private AudioClip GenerateSound(string key)
        {
            switch (key)
            {
                case "line_move":
                    return GenerateLineMoveSound();
                case "line_collide":
                    return GenerateLineCollideSound();
                case "line_complete":
                    return GenerateLineCompleteSound();
                case "stage_win":
                    return GenerateStageWinSound();
                case "stage_lose":
                    return GenerateStageLoseSound();
                case "btn_tap":
                    return GenerateButtonTapSound();
                case "btn_confirm":
                    return GenerateButtonConfirmSound();
                default:
                    Debug.LogWarning($"[SoundGenerator] Unknown sound key: {key}");
                    return null;
            }
        }

        /// <summary>
        /// Line move: Short ascending sweep (whoosh effect)
        /// </summary>
        private AudioClip GenerateLineMoveSound()
        {
            float duration = 0.25f;
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float normalizedT = (float)i / samples;
                
                // Ascending frequency sweep from 300Hz to 800Hz
                float freq = Mathf.Lerp(300f, 800f, normalizedT);
                float wave = Mathf.Sin(2f * Mathf.PI * freq * t);
                
                // Add a subtle harmonic
                wave += 0.3f * Mathf.Sin(2f * Mathf.PI * freq * 2f * t);
                
                // Envelope: quick attack, sustained, quick release
                float envelope = 1f;
                if (normalizedT < 0.05f)
                    envelope = normalizedT / 0.05f;
                else if (normalizedT > 0.7f)
                    envelope = (1f - normalizedT) / 0.3f;
                
                data[i] = wave * envelope * 0.4f;
            }

            return CreateClip("line_move", data, duration);
        }

        /// <summary>
        /// Line collide: Low frequency buzz/crunch
        /// </summary>
        private AudioClip GenerateLineCollideSound()
        {
            float duration = 0.35f;
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float normalizedT = (float)i / samples;
                
                // Low buzz at 80Hz with distortion
                float wave = Mathf.Sin(2f * Mathf.PI * 80f * t);
                wave += 0.5f * Mathf.Sin(2f * Mathf.PI * 160f * t);
                wave += 0.3f * Mathf.Sin(2f * Mathf.PI * 55f * t);
                
                // Add noise crunch
                float noise = (Random.value * 2f - 1f) * 0.3f;
                wave = wave * 0.6f + noise * (1f - normalizedT);
                
                // Clip for distortion effect
                wave = Mathf.Clamp(wave * 1.5f, -1f, 1f);
                
                // Decay envelope
                float envelope = Mathf.Exp(-normalizedT * 4f);
                
                data[i] = wave * envelope * 0.5f;
            }

            return CreateClip("line_collide", data, duration);
        }

        /// <summary>
        /// Line complete: Sparkle/ding — bright short chime
        /// </summary>
        private AudioClip GenerateLineCompleteSound()
        {
            float duration = 0.3f;
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float normalizedT = (float)i / samples;
                
                // High pitched chime at 1200Hz
                float wave = Mathf.Sin(2f * Mathf.PI * 1200f * t);
                wave += 0.5f * Mathf.Sin(2f * Mathf.PI * 1800f * t);
                wave += 0.25f * Mathf.Sin(2f * Mathf.PI * 2400f * t);
                
                // Quick decay
                float envelope = Mathf.Exp(-normalizedT * 6f);
                
                data[i] = wave * envelope * 0.3f;
            }

            return CreateClip("line_complete", data, duration);
        }

        /// <summary>
        /// Stage win: Ascending arpeggio chime (C-E-G-C sequence)
        /// </summary>
        private AudioClip GenerateStageWinSound()
        {
            float duration = 0.8f;
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];

            // C5=523, E5=659, G5=784, C6=1047
            float[] notes = { 523f, 659f, 784f, 1047f };
            float noteLength = duration / notes.Length;

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float normalizedT = (float)i / samples;
                
                int noteIndex = Mathf.Min((int)(normalizedT * notes.Length), notes.Length - 1);
                float noteT = (normalizedT * notes.Length) - noteIndex;
                float freq = notes[noteIndex];
                
                float wave = Mathf.Sin(2f * Mathf.PI * freq * t);
                wave += 0.4f * Mathf.Sin(2f * Mathf.PI * freq * 2f * t);
                wave += 0.2f * Mathf.Sin(2f * Mathf.PI * freq * 3f * t);
                
                // Per-note envelope with sustain
                float noteEnvelope = 1f;
                if (noteT < 0.1f)
                    noteEnvelope = noteT / 0.1f;
                else
                    noteEnvelope = Mathf.Exp(-(noteT - 0.1f) * 2f);
                
                // Overall crescendo
                float overallEnvelope = Mathf.Lerp(0.5f, 1f, normalizedT);
                
                // Final decay
                if (normalizedT > 0.85f)
                    overallEnvelope *= (1f - normalizedT) / 0.15f;
                
                data[i] = wave * noteEnvelope * overallEnvelope * 0.35f;
            }

            return CreateClip("stage_win", data, duration);
        }

        /// <summary>
        /// Stage lose: Descending minor tone (sad trombone style)
        /// </summary>
        private AudioClip GenerateStageLoseSound()
        {
            float duration = 0.7f;
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float normalizedT = (float)i / samples;
                
                // Descending from 400Hz to 150Hz
                float freq = Mathf.Lerp(400f, 150f, normalizedT);
                
                // Minor feel with flat third
                float wave = Mathf.Sin(2f * Mathf.PI * freq * t);
                wave += 0.4f * Mathf.Sin(2f * Mathf.PI * freq * 1.2f * t); // Minor third
                wave += 0.2f * Mathf.Sin(2f * Mathf.PI * freq * 0.5f * t); // Sub bass
                
                // Slow decay
                float envelope = Mathf.Exp(-normalizedT * 2f);
                
                data[i] = wave * envelope * 0.35f;
            }

            return CreateClip("stage_lose", data, duration);
        }

        /// <summary>
        /// Button tap: Short click/pop sound
        /// </summary>
        private AudioClip GenerateButtonTapSound()
        {
            float duration = 0.08f;
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float normalizedT = (float)i / samples;
                
                // High frequency click
                float wave = Mathf.Sin(2f * Mathf.PI * 2000f * t);
                wave += 0.5f * Mathf.Sin(2f * Mathf.PI * 4000f * t);
                
                // Very fast decay
                float envelope = Mathf.Exp(-normalizedT * 15f);
                
                data[i] = wave * envelope * 0.3f;
            }

            return CreateClip("btn_tap", data, duration);
        }

        /// <summary>
        /// Button confirm: Two-tone confirmation beep
        /// </summary>
        private AudioClip GenerateButtonConfirmSound()
        {
            float duration = 0.2f;
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float normalizedT = (float)i / samples;
                
                // Two-tone: low then high
                float freq = normalizedT < 0.4f ? 600f : 900f;
                float wave = Mathf.Sin(2f * Mathf.PI * freq * t);
                wave += 0.3f * Mathf.Sin(2f * Mathf.PI * freq * 2f * t);
                
                // Quick envelope per tone
                float toneT = normalizedT < 0.4f ? normalizedT / 0.4f : (normalizedT - 0.4f) / 0.6f;
                float envelope = Mathf.Exp(-toneT * 4f);
                
                data[i] = wave * envelope * 0.3f;
            }

            return CreateClip("btn_confirm", data, duration);
        }

        private AudioClip CreateClip(string name, float[] data, float duration)
        {
            AudioClip clip = AudioClip.Create(name, data.Length, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        /// <summary>
        /// Clear the cache and release all generated clips.
        /// </summary>
        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}
