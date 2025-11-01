using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using Pixelplacement;

public class MusicController : Singleton<MusicController>
{

    [Header("Audio Sources")]
    public AudioSource bass;
    public AudioSource drums;
    public AudioSource instruments;
    public AudioSource melody;

    [Header("Mixer")]
    public AudioMixer mixer;

    private bool isFading = false;

    public static readonly string BassVolume = "BassVolume";
    public static readonly string DrumsVolume = "DrumsVolume";
    public static readonly string InstrumentsVolume = "InstrumentsVolume";
    public static readonly string MelodyVolume = "MelodyVolume";

    public void PlayAllSynced()
    {
        double startTime = AudioSettings.dspTime + 0.1;

        bass.PlayScheduled(startTime);
        drums.PlayScheduled(startTime);
        instruments.PlayScheduled(startTime);
        melody.PlayScheduled(startTime);
    }

    public void StopAll()
    {
        bass.Stop();
        drums.Stop();
        instruments.Stop();
        melody.Stop();
    }

    // Toggle example
    public void ToggleBass(bool on) => mixer.SetFloat(BassVolume, on ? 0f : -80f);
    public void ToggleDrums(bool on) => mixer.SetFloat(DrumsVolume, on ? 0f : -80f);
    public void ToggleInstruments(bool on) => mixer.SetFloat(InstrumentsVolume, on ? 0f : -80f);
    public void ToggleMelody(bool on) => mixer.SetFloat(MelodyVolume, on ? 0f : -80f);

    // Smooth fade (shared coroutine)
    private IEnumerator FadeMixerGroup(string param, float target, float duration)
    {
        mixer.GetFloat(param, out float current);
        float start = current;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float newValue = Mathf.Lerp(start, target, time / duration);
            mixer.SetFloat(param, newValue);
            yield return null;
        }
        mixer.SetFloat(param, target);
    }

    public void FadeTrack(string param, bool fadeIn, float duration = 1f)
    {
        if (isFading) return;
        StartCoroutine(FadeMixerGroup(param, fadeIn ? 0f : -80f, duration));
    }

    // 🎯 Reset all tracks in sync, with fade out / in
    public Coroutine ResetMusic(float fadeOutTime = 1f, float fadeInTime = 1f)
    {
        if (isFading) return null;
        return StartCoroutine(ResetSequence(fadeOutTime, fadeInTime));
    }


    public Coroutine FadeOutAndStop(float time)
    {
        return StartCoroutine(FadeOutAndStopSequence(time));
    }

    IEnumerator FadeOutAndStopSequence(float time)
    {
        // Fade out all groups
        yield return StartCoroutine(FadeMixerGroup(BassVolume, -80f, time));
        yield return StartCoroutine(FadeMixerGroup(DrumsVolume, -80f, time));
        yield return StartCoroutine(FadeMixerGroup(InstrumentsVolume, -80f, time));
        yield return StartCoroutine(FadeMixerGroup(MelodyVolume, -80f, time));

        // Stop and restart all sources at the same DSP time
        StopAll();
    }

    private IEnumerator ResetSequence(float fadeOutTime, float fadeInTime)
    {
        isFading = true;
        yield return StartCoroutine(FadeOutAndStopSequence(fadeOutTime));
        PlayAllSynced();

        // Fade back in
        //yield return new WaitForSecondsRealtime(0.2f);
        //yield return StartCoroutine(FadeMixerGroup(BassVolume, 0f, fadeInTime));
        //yield return StartCoroutine(FadeMixerGroup(DrumsVolume, 0f, fadeInTime));
        //yield return StartCoroutine(FadeMixerGroup(InstrumentsVolume, 0f, fadeInTime));
        //yield return StartCoroutine(FadeMixerGroup(MelodyVolume, 0f, fadeInTime));

        isFading = false;
    }
}
