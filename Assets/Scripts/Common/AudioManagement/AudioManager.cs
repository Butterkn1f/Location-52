using Common.DesignPatterns;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace Common.AudioManagement
{
    /// <summary>
    /// This is the class that manages the audio for the game
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        // Create an object pool for the audio source  
        [SerializeField] private ObjectPooler _audioSource;

        // The default music and SFX volume
        private float MusicVol = 0.5f;
        private float SFXVol = 0.5f;

        // Database of all the audio tracks
        [SerializeField] private AudioList _currentAudioList;
        [SerializeField] private AudioMixerGroup _bgmMixerGroup;
        [SerializeField] private AudioMixerGroup _sfxMixerGroup;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void ResetGameReaction()
        {
            StopAllAudio();
            StoreAllInactiveAudioSources();
        }

        private void PauseGameReaction(bool isPaused)
        {
            if (isPaused)
            {
                PauseAllAudio();
            }
            else
            {
                ResumeAllAudio();
            }
        }

        public float Play2DAudio(AudioID audioID, float audioSoundMultiplier = 0.5f, float playAtTime = 0)
        {
            // Keep any inactive thigns
            StoreAllInactiveAudioSources();

            // Get the audio track
            GameObject audioGO = _audioSource.GetObject();
            audioGO.transform.position = this.transform.position;
            audioGO.transform.SetParent(this.transform);

            AudioTrack _audioTrackToPlay = _currentAudioList.Tracks.Where(track => track.ID == audioID).FirstOrDefault();

            audioGO.GetComponent<AudioSource>().spatialBlend = 0f;
            audioGO.GetComponent<AudioSource>().clip = _audioTrackToPlay.Clip;
            audioGO.GetComponent<AudioSource>().loop = _audioTrackToPlay.LoopTrack;

            if (_audioTrackToPlay.Type == AudioType.BGM)
            {
                audioGO.GetComponent<AudioSource>().volume = audioSoundMultiplier;
                audioGO.GetComponent<AudioSource>().outputAudioMixerGroup = _bgmMixerGroup;
            }
            else
            {
                audioGO.GetComponent<AudioSource>().volume = audioSoundMultiplier;
                audioGO.GetComponent<AudioSource>().outputAudioMixerGroup = _sfxMixerGroup;
            }

            audioGO.GetComponent<AudioSource>().Play();

            return _audioTrackToPlay.Clip.length;
        }

        public float Play3DAudio(Transform audioTransform, AudioID audioID, float audioSoundMultiplier = 0.5f, float playAtTime = 0)
        {
            // Keep any inactive thigns
            StoreAllInactiveAudioSources();

            GameObject audioGO = _audioSource.GetObject();
            audioGO.transform.position = audioTransform.position;
            audioGO.transform.SetParent(audioTransform);

            // Get the audio track
            AudioTrack _audioTrackToPlay = _currentAudioList.Tracks.Where(track => track.ID == audioID).FirstOrDefault();

            // Set 3D audio
            audioGO.GetComponent<AudioSource>().spatialBlend = 1f;
            audioGO.GetComponent<AudioSource>().clip = _audioTrackToPlay.Clip;
            audioGO.GetComponent<AudioSource>().loop = _audioTrackToPlay.LoopTrack;

            if (_audioTrackToPlay.Type == AudioType.BGM)
            {
                audioGO.GetComponent<AudioSource>().volume = audioSoundMultiplier;
                audioGO.GetComponent<AudioSource>().outputAudioMixerGroup = _bgmMixerGroup;
            }
            else
            {
                audioGO.GetComponent<AudioSource>().volume = audioSoundMultiplier;
                audioGO.GetComponent<AudioSource>().outputAudioMixerGroup = _sfxMixerGroup;
            }

            audioGO.GetComponent<AudioSource>().Play();

            return _audioTrackToPlay.Clip.length;
        }

        public void StoreAllInactiveAudioSources()
        {
            for (int i = 0; i < _audioSource.pooledObjects.Count(); i++)
            {
                if (_audioSource.pooledObjects[i] != null)
                {
                    if (!_audioSource.pooledObjects[i].GetComponent<AudioSource>().isPlaying)
                    {
                        _audioSource.pooledObjects[i].SetActive(false);
                    }
                }
                else
                {
                    _audioSource.pooledObjects.Remove(_audioSource.pooledObjects[i]);
                }
            }
        }

        public void StopAudioByID(AudioID audioID)
        {
            AudioTrack _audioTrack = _currentAudioList.Tracks.Where(track => track.ID == audioID).FirstOrDefault();

            for (int i = 0; i < _audioSource.pooledObjects.Count(); i++)
            {
                if (_audioSource.pooledObjects[i].GetComponent<AudioSource>().clip == _audioTrack.Clip)
                {
                    _audioSource.pooledObjects[i].GetComponent<AudioSource>().Stop();
                }
            }
        }

        public void PauseAllAudio()
        {
            StoreAllInactiveAudioSources();

            for (int i = 0; i < _audioSource.pooledObjects.Count(); i++)
            {
                if (_audioSource.pooledObjects[i] == null) { _audioSource.pooledObjects.Remove(_audioSource.pooledObjects[i]); continue; }
                _audioSource.pooledObjects[i].GetComponent<AudioSource>().Pause();
            }
        }

        public void ResumeAllAudio()
        {
            StoreAllInactiveAudioSources();

            for (int i = 0; i < _audioSource.pooledObjects.Count(); i++)
            {
                _audioSource.pooledObjects[i].GetComponent<AudioSource>().UnPause();
            }
        }

        public void StopAllAudio()
        {
            for (int i = 0; i < _audioSource.pooledObjects.Count(); i++)
            {
                if (_audioSource.pooledObjects[i] == null) { _audioSource.pooledObjects.Remove(_audioSource.pooledObjects[i]); continue; }
                _audioSource.pooledObjects[i].GetComponent<AudioSource>().Stop();
            }
        }
    }
}
