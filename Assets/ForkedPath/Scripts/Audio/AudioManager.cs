using UnityEngine;
using Pixelplacement;
public class AudioManager : Singleton<AudioManager>
{
    //TODO: subscribe on scene load? Or should only AudioContoller be persistent?
    private void OnEnable() => GameEvents.Instance.OnFX += HandleFX;
    private void OnDisable() => GameEvents.Instance.OnFX -= HandleFX;

    void HandleFX(FXEventData data)
    {
        var entry = FXResolver.GetSerializedAudio(data);
        PlayEntrySoundWithContextRules(entry, data);

    }

    static void PlayEntrySoundWithContextRules(SerializedAudio entry, FXEventData data)
    {
        if (entry == null) return;
        // Delegate selection and parameter application to AudioController
        AudioController.Instance.PlaySound3D(entry, data.position, entry.volume);
    }
}