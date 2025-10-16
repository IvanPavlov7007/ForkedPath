using UnityEngine;
using Pixelplacement;
public class AudioManager : Singleton<AudioManager>
{
    //TODO: subscribe on scene load? Or should only AudioContoller be persistent?
    private void OnEnable()
    {
        GameEvents.Instance.OnFX += OnFX;
    }

    private void OnDisable()
    {
        GameEvents.Instance.OnFX -= OnFX;
    }

    void OnFX(FXEventData data)
    {
        if (data == null) return;
        if (data.sound != null)
        {
            AudioController.Instance.PlaySound3D(data.sound.name, data.position, 1f);
        }
    }
}