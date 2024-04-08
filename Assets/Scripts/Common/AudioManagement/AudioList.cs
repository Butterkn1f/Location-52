using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.AudioManagement
{
    [CreateAssetMenu(menuName = "Custom app data containers/Audio List", fileName = "New Audio List", order = 20)]
    public class AudioList : ScriptableObject
    {
        public List<AudioTrack> Tracks;
    }

    public enum AudioID
    {
        NONE,
        LIKEY_DEMO_SONG,
    }
}
