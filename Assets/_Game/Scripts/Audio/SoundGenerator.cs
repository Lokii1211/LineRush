using System.Collections.Generic;
using UnityEngine;

namespace _Game.Audio
{
    /// <summary>
    /// Procedural sound generator for Line Rush by Viya Nexus.
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
                // ── New sounds for enhanced features ──
                case "combo_hit":
                    return GenerateComboHitSound();
                case "combo_break":
                    return GenerateComboBreakSound();
                case "hint_reveal":
                    return GenerateHintRevealSound();
                case "achievement_unlock":
                    return GenerateAchievementUnlockSound();
                case "daily_complete":
                    return GenerateDailyCompleteSound();
                case "perfect_clear":
                    return GeneratePerfectClearSound();
                default:
                    Debug.LogWarning($"[SoundGenerator] Unknown sound key: {key}");
                    return null;
            }
        }

        /// <summary>Line move: Short ascending sweep (whoosh effect)</summary>
        private AudioClip GenerateLineMoveSound()
        {
            float duration = 0.25f;
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float normalizedT = (float)i / samples;
                float freq = Mathf.Lerp(300f, 800f, normalizedT);
                float wave = Mathf.Sin(2f * Mathf.PI * freq * t);
                wave += 0.3f * Mathf.Sin(2f * Mathf.PI * freq * 2f * t);
                float envelope = 1f;
                if (normalizedT < 0.05f) envelope = normalizedT / 0.05f;
                else if (normalizedT > 0.7f) envelope = (1f - normalizedT) / 0.3f;
                data[i] = wave * envelope * 0.4f;
            }
            return CreateClip("line_move", data, duration);
        }

        /// <summary>Line collide: Low frequency buzz/crunch</summary>
        private AudioClip GenerateLineCollideSound()
        {
            float duration = 0.35f;
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float normalizedT = (float)i / samples;
                float wave = Mathf.Sin(2f * Mathf.PI * 80f * t);
                wave += 0.5f * Mathf.Sin(2f * Mathf.PI * 160f * t);
                wave += 0.3f * Mathf.Sin(2f * Mathf.PI * 55f * t);
                float noise = (Random.value * 2f - 1f) * 0.3f;
                wave = wave * 0.6f + noise * (1f - normalizedT);
                wave = Mathf.Clamp(wave * 1.5f, -1f, 1f);
                float envelope = Mathf.Exp(-normalizedT * 4f);
                data[i] = wave * envelope * 0.5f;
            }
            return CreateClip("line_collide", data, duration);
        }

        /// <summary>Line complete: Sparkle/ding — bright short chime</summary>
        private AudioClip GenerateLineCompleteSound()
        {
            float duration = 0.3f;
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float normalizedT = (float)i / samples;
                float wave = Mathf.Sin(2f * Mathf.PI * 1200f * t);
                wave += 0.5f * Mathf.Sin(2f * Mathf.PI * 1800f * t);
                wave += 0.25f * Mathf.Sin(2f * Mathf.PI * 2400f * t);
                float envelope = Mathf.Exp(-normalizedT * 6f);
                data[i] = wave * envelope * 0.3f;
            }
            return CreateClip("line_complete", data, duration);
        }

        /// <summary>Stage win: Ascending arpeggio chime (C-E-G-C sequence)</summary>
        private AudioClip GenerateStageWinSound()
        {
            float duration = 0.8f;
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];
            float[] notes = { 523f, 659f, 784f, 1047f };

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
                float noteEnvelope = noteT < 0.1f ? noteT / 0.1f : Mathf.Exp(-(noteT - 0.1f) * 2f);
                float overallEnvelope = Mathf.Lerp(0.5f, 1f, normalizedT);
                if (normalizedT > 0.85f) overallEnvelope *= (1f - normalizedT) / 0.15f;
                data[i] = wave * noteEnvelope * overallEnvelope * 0.35f;
            }
            return CreateClip("stage_win", data, duration);
        }

        /// <summary>Stage lose: Descending minor tone</summary>
        private AudioClip GenerateStageLoseSound()
        {
            float duration = 0.7f;
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float normalizedT = (float)i / samples;
                float freq = Mathf.Lerp(400f, 150f, normalizedT);
                float wave = Mathf.Sin(2f * Mathf.PI * freq * t);
                wave += 0.4f * Mathf.Sin(2f * Mathf.PI * freq * 1.2f * t);
                wave += 0.2f * Mathf.Sin(2f * Mathf.PI * freq * 0.5f * t);
                float envelope = Mathf.Exp(-normalizedT * 2f);
                data[i] = wave * envelope * 0.35f;
            }
            return CreateClip("stage_lose", data, duration);
        }

        /// <summary>Button tap: Short click/pop sound</summary>
        private AudioClip GenerateButtonTapSound()
        {
            float duration = 0.08f;
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float normalizedT = (float)i / samples;
                float wave = Mathf.Sin(2f * Mathf.PI * 2000f * t);
                wave += 0.5f * Mathf.Sin(2f * Mathf.PI * 4000f * t);
                float envelope = Mathf.Exp(-normalizedT * 15f);
                data[i] = wave * envelope * 0.3f;
            }
            return CreateClip("btn_tap", data, duration);
        }

        /// <summary>Button confirm: Two-tone confirmation beep</summary>
        private AudioClip GenerateButtonConfirmSound()
        {
            float duration = 0.2f;
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float normalizedT = (float)i / samples;
                float freq = normalizedT < 0.4f ? 600f : 900f;
                float wave = Mathf.Sin(2f * Mathf.PI * freq * t);
                wave += 0.3f * Mathf.Sin(2f * Mathf.PI * freq * 2f * t);
                float toneT = normalizedT < 0.4f ? normalizedT / 0.4f : (normalizedT - 0.4f) / 0.6f;
                float envelope = Mathf.Exp(-toneT * 4f);
                data[i] = wave * envelope * 0.3f;
            }
            return CreateClip("btn_confirm", data, duration);
        }

        // ============================================================
        //  NEW SOUNDS — Enhanced engagement features
        // ============================================================

        /// <summary>
        /// Combo hit: Ascending chime that gets higher per combo level.
        /// Bright, rewarding "ding" that escalates.
        /// </summary>
        private AudioClip GenerateComboHitSound()
        {
            float duration = 0.2f;
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float normalizedT = (float)i / samples;

                // High sparkle at 1500Hz with harmonics
                float freq = 1500f;
                float wave = Mathf.Sin(2f * Mathf.PI * freq * t);
                wave += 0.6f * Mathf.Sin(2f * Mathf.PI * freq * 1.5f * t); // Perfect fifth
                wave += 0.3f * Mathf.Sin(2f * Mathf.PI * freq * 2f * t);   // Octave

                // Quick attack, medium decay
                float envelope = 1f;
                if (normalizedT < 0.03f)
                    envelope = normalizedT / 0.03f;
                else
                    envelope = Mathf.Exp(-(normalizedT - 0.03f) * 5f);

                data[i] = wave * envelope * 0.35f;
            }
            return CreateClip("combo_hit", data, duration);
        }

        /// <summary>
        /// Combo break: Short descending buzz when combo is lost.
        /// Feels like a "whomp" — not harsh, but noticeable.
        /// </summary>
        private AudioClip GenerateComboBreakSound()
        {
            float duration = 0.3f;
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float normalizedT = (float)i / samples;

                // Descending from 500Hz to 200Hz
                float freq = Mathf.Lerp(500f, 200f, normalizedT);
                float wave = Mathf.Sin(2f * Mathf.PI * freq * t);
                wave += 0.3f * Mathf.Sin(2f * Mathf.PI * freq * 0.5f * t); // Sub octave

                // Add subtle noise
                float noise = (Random.value * 2f - 1f) * 0.15f * (1f - normalizedT);
                wave += noise;

                float envelope = Mathf.Exp(-normalizedT * 3f);
                data[i] = wave * envelope * 0.3f;
            }
            return CreateClip("combo_break", data, duration);
        }

        /// <summary>
        /// Hint reveal: Sparkle/twinkle effect — magical discovery feel.
        /// Three ascending bright tones.
        /// </summary>
        private AudioClip GenerateHintRevealSound()
        {
            float duration = 0.5f;
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];

            float[] tones = { 800f, 1200f, 1600f };
            float toneLength = duration / tones.Length;

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float normalizedT = (float)i / samples;

                int toneIndex = Mathf.Min((int)(normalizedT * tones.Length), tones.Length - 1);
                float toneT = (normalizedT * tones.Length) - toneIndex;
                float freq = tones[toneIndex];

                float wave = Mathf.Sin(2f * Mathf.PI * freq * t);
                wave += 0.5f * Mathf.Sin(2f * Mathf.PI * freq * 2f * t);
                wave += 0.25f * Mathf.Sin(2f * Mathf.PI * freq * 3f * t);

                // Shimmer effect — fast tremolo
                float shimmer = 1f + 0.2f * Mathf.Sin(2f * Mathf.PI * 20f * t);
                wave *= shimmer;

                float toneEnvelope = toneT < 0.05f ? toneT / 0.05f : Mathf.Exp(-(toneT - 0.05f) * 3f);
                float overallEnvelope = normalizedT > 0.85f ? (1f - normalizedT) / 0.15f : 1f;

                data[i] = wave * toneEnvelope * overallEnvelope * 0.25f;
            }
            return CreateClip("hint_reveal", data, duration);
        }

        /// <summary>
        /// Achievement unlock: Triumphant fanfare — brass-like C major chord.
        /// Bold, celebratory, and satisfying.
        /// </summary>
        private AudioClip GenerateAchievementUnlockSound()
        {
            float duration = 1.0f;
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];

            // C4=262, E4=330, G4=392, C5=523
            float[] chord = { 262f, 330f, 392f, 523f };

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float normalizedT = (float)i / samples;

                float wave = 0f;

                // Stagger chord entry
                for (int c = 0; c < chord.Length; c++)
                {
                    float startT = c * 0.08f;
                    if (t < startT) continue;

                    float localT = t - startT;
                    float freq = chord[c];

                    // Brass-like: fundamental + strong harmonics
                    float note = Mathf.Sin(2f * Mathf.PI * freq * localT);
                    note += 0.5f * Mathf.Sin(2f * Mathf.PI * freq * 2f * localT);
                    note += 0.3f * Mathf.Sin(2f * Mathf.PI * freq * 3f * localT);
                    note += 0.15f * Mathf.Sin(2f * Mathf.PI * freq * 4f * localT);

                    float noteEnv = Mathf.Exp(-(t - startT) * 1.5f);
                    wave += note * noteEnv;
                }

                // Overall envelope
                float envelope = 1f;
                if (normalizedT < 0.02f) envelope = normalizedT / 0.02f;
                if (normalizedT > 0.7f) envelope = (1f - normalizedT) / 0.3f;

                data[i] = wave * envelope * 0.15f;
            }
            return CreateClip("achievement_unlock", data, duration);
        }

        /// <summary>
        /// Daily challenge complete: Special victory jingle.
        /// Quick ascending 5-note scale with reverb tail.
        /// </summary>
        private AudioClip GenerateDailyCompleteSound()
        {
            float duration = 1.2f;
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];

            // Pentatonic scale: C5, D5, E5, G5, A5, C6
            float[] notes = { 523f, 587f, 659f, 784f, 880f, 1047f };
            float noteLen = 0.15f;

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float normalizedT = (float)i / samples;

                float wave = 0f;

                for (int n = 0; n < notes.Length; n++)
                {
                    float noteStart = n * noteLen;
                    if (t < noteStart) continue;

                    float localT = t - noteStart;
                    float freq = notes[n];

                    float note = Mathf.Sin(2f * Mathf.PI * freq * localT);
                    note += 0.4f * Mathf.Sin(2f * Mathf.PI * freq * 2f * localT);
                    note += 0.15f * Mathf.Sin(2f * Mathf.PI * freq * 3f * localT);

                    // Each note decays independently
                    float noteEnv = Mathf.Exp(-localT * 3f);
                    // Later notes are louder (crescendo)
                    float volume = Mathf.Lerp(0.5f, 1f, (float)n / notes.Length);

                    wave += note * noteEnv * volume;
                }

                // Final sustain tail
                if (normalizedT > 0.8f)
                    wave *= (1f - normalizedT) / 0.2f;

                data[i] = wave * 0.18f;
            }
            return CreateClip("daily_complete", data, duration);
        }

        /// <summary>
        /// Perfect clear: Bright, satisfying arpeggio with sparkle.
        /// Like getting a perfect score — very rewarding.
        /// </summary>
        private AudioClip GeneratePerfectClearSound()
        {
            float duration = 0.9f;
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];

            // C major arpeggio up then octave: C5, E5, G5, C6
            float[] notes = { 523f, 659f, 784f, 1047f };
            float noteLen = duration / (notes.Length + 1);

            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / SampleRate;
                float normalizedT = (float)i / samples;
                float wave = 0f;

                for (int n = 0; n < notes.Length; n++)
                {
                    float noteStart = n * noteLen;
                    if (t < noteStart) continue;

                    float localT = t - noteStart;
                    float freq = notes[n];

                    // Very bright: lots of high harmonics
                    float note = Mathf.Sin(2f * Mathf.PI * freq * localT);
                    note += 0.5f * Mathf.Sin(2f * Mathf.PI * freq * 2f * localT);
                    note += 0.3f * Mathf.Sin(2f * Mathf.PI * freq * 3f * localT);
                    note += 0.2f * Mathf.Sin(2f * Mathf.PI * freq * 4f * localT);

                    // Sparkle shimmer
                    float shimmer = 1f + 0.15f * Mathf.Sin(2f * Mathf.PI * 15f * localT);
                    note *= shimmer;

                    float noteEnv = Mathf.Exp(-localT * 2f);
                    wave += note * noteEnv;
                }

                // Final chord sustain
                if (normalizedT > 0.7f)
                {
                    float chordT = t;
                    float chord = 0f;
                    foreach (var f in notes)
                    {
                        chord += Mathf.Sin(2f * Mathf.PI * f * chordT) * 0.2f;
                    }
                    wave += chord * ((1f - normalizedT) / 0.3f);
                }

                float env = normalizedT > 0.85f ? (1f - normalizedT) / 0.15f : 1f;
                data[i] = wave * env * 0.15f;
            }
            return CreateClip("perfect_clear", data, duration);
        }

        private AudioClip CreateClip(string name, float[] data, float duration)
        {
            AudioClip clip = AudioClip.Create(name, data.Length, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }

        /// <summary>Clear the cache and release all generated clips.</summary>
        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}
