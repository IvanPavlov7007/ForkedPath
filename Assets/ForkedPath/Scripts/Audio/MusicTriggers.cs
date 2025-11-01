using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.ForkedPath.Scripts.Audio
{
    public class MusicTriggers : MonoBehaviour
    {
        public PlayerEnterTrigger musicStage1;
        public PlayerEnterTrigger musicStage2;
        public PlayerEnterTrigger musicStage3;

        private void OnEnable()
        {
            GameEvents.Instance.OnPlayerEnterTrigger += OnPlayerEnterTrigger;
        }

        private void OnDisable()
        {
            GameEvents.Instance.OnPlayerEnterTrigger -= OnPlayerEnterTrigger;
        }

        private IEnumerator Start()
        {
            yield return MusicController.Instance?.ResetMusic();
            MusicController.Instance?.ToggleBass(true);
        }

        private void OnPlayerEnterTrigger(PlayerEnterTrigger trigger)
        {
            if(trigger == musicStage1)
            {
                MusicController.Instance?.FadeTrack(MusicController.MelodyVolume, true, 1f);
            }
            else if (trigger == musicStage2)
            {
                MusicController.Instance?.FadeTrack(MusicController.DrumsVolume, true, 1f);
            }
            else if(trigger == musicStage3)
            {
                MusicController.Instance?.FadeTrack(MusicController.InstrumentsVolume, true, 1f);
            }
        }

    }
}