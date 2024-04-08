using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.AudioManagement
{
    [System.Serializable]
    public class AudioTrack
    {
        // For easy identification
        public string Header;
        public AudioID ID;
        public Common.AudioManagement.AudioType Type;
        public AudioClip Clip;

        public bool LoopTrack = false;
    }

    public enum AudioType
    {
        BGM,
        SFX
    }
}
