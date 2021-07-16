using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace EnglishKids.SortingTransport
{
    public enum Audio
    {
        None,
        Conveyer,
        Steam,
        Car,
        Bike,
        Tractor,
        Helicopter,
        CorrectAnswer,
        WrongAnswer,
        UploadYellowPaint,
        RobotWork,
        Coin
    }

    public enum Speach
    {
        SortByColor,
        Car,
        Bike,
        Tractor,
        Helicopter,
        Yellow,
        Green,
        Black,
        Blue,
        Brown,
        Gray,
        Orange,
        Pink,
        Purple,
        Red,
        White
    }

    public class AudioManager : MonoSingleton<AudioManager>
    {
        [System.Serializable]
        private class BaseTrack
        {

        }

        [System.Serializable]
        private class AudioTrack
        {
            public Audio kind;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 1f;
            public int priority;
        }

        [System.Serializable]
        private class SpeachTrack
        {
            public Speach kind;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 1f;            
        }

        //==================================================
        // Fields
        //==================================================

        [Space]
        [Range(0f, 1f)] [SerializeField] private float _musicVolume = 1f;
        [Range(0f, 1f)] [SerializeField] private float _soundVolume = 1f;
        [Range(0f, 1f)] [SerializeField] private float _speachVolume = 1f;

        [Header("Audio Sources Settings")]        
        [SerializeField] private GameObject _sourcePanel;
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _speachSource;
        [SerializeField] private int _maxSoundSourceCount;

        [Header("Music List")]
        [SerializeField] private AudioTrack[] _musics;

        [Header("Audio List")]
        [SerializeField] private AudioTrack[] _sounds;

        [Header("Speach List")]
        [SerializeField] private SpeachTrack[] _speaches;

        private List<AudioSource> _soundSources;
        private Queue<SpeachTrack> _speachOrder;

        //==================================================
        // Methods
        //==================================================

        protected override void Init()
        {            
            base.Init();
                        
            _soundSources = new List<AudioSource>();
            _speachOrder = new Queue<SpeachTrack>();

            for (int i = 0; i < _maxSoundSourceCount; i++)
            {
                AudioSource source = _sourcePanel.AddComponent<AudioSource>();
                _soundSources.Add(source);
            }

            StartCoroutine(PlayingMusicProcess());
            StartCoroutine(PlayingSpeachProcess());
        }

        private IEnumerator PlayingMusicProcess()
        {
            const int MIN_COUNT_FOR_CHECKING_LAST_TRACK = 2;
            bool checkLastTrack = _musics.Length >= MIN_COUNT_FOR_CHECKING_LAST_TRACK;      // Don't repeat the same track in a row.

            List<AudioTrack> actualMusics = new List<AudioTrack>(_musics);
            AudioTrack track = null;
            AudioTrack lastTrack = null;

            bool canPlay = _musics.Length > 0;

            while (canPlay)
            {
                int index = Random.Range(0, actualMusics.Count);
                track = actualMusics[index];

                if (checkLastTrack)
                {
                    actualMusics.RemoveAt(index);

                    if (lastTrack != null)
                        actualMusics.Add(lastTrack);

                    lastTrack = track;
                }
                                
                _musicSource.clip = track.clip;
                _musicSource.volume = _musicVolume * track.volume;
                _musicSource.Play();

                yield return new WaitForSeconds(track.clip.length);
            }
        }

        public void StopSound(Audio kind)
        {
            AudioTrack track = FindTrack(_sounds, kind);
            foreach (var item in _soundSources)
            {
                if (item.isPlaying && item.clip.name.Equals(track.clip.name))
                    item.Stop();
            }
        }

        public void PlaySingleSound(Audio kind)
        {
            AudioTrack track = FindTrack(_sounds, kind);

            foreach (AudioSource item in _soundSources)
            {
                if (item.isPlaying && item.clip.name.Equals(track.clip.name))
                {
                    return;
                }
            }

            PlaySound(kind);
        }

        public void PlaySound(Audio kind)
        {
            AudioTrack target = FindTrack(_sounds, kind);

            if (target == null)
            {
                Debug.LogError("Counldn't find sound");
                return;
            }

            AudioSource source = null;
            AudioTrack sourceTrack = null;

            // Find AudioSource with min priority track. If not playing - it's min.
            foreach (AudioSource item in _soundSources)
            {
                if (source == null)
                {
                    source = item;

                    if (source.isPlaying)
                    {
                        sourceTrack = FindTrack(_sounds, source.clip.name);
                        continue;
                    }
                    else
                        break;
                }

                if (!item.isPlaying)
                {
                    source = item;
                    break;
                }

                AudioTrack itemTrack = FindTrack(_sounds, item.clip.name);
                if (sourceTrack.priority > itemTrack.priority)
                {
                    source = item;
                    sourceTrack = itemTrack;
                }
            }

            // Play sound;
            if (source != null && (!source.isPlaying || sourceTrack.priority < target.priority))
            {
                source.Stop();
                source.clip = target.clip;
                source.volume = _soundVolume * target.volume;
                source.Play();
            }
        }

        public void PlaySpeach(params Speach[] speachSounds)
        {
            _speachOrder.Clear();

            for (int i = 0; i < speachSounds.Length; i++)
                _speachOrder.Enqueue(FindTrack(_speaches, speachSounds[i]));
        }

        public void StopSpeach()
        {
            _speachOrder.Clear();
            _speachSource.Stop();
        }

        private IEnumerator PlayingSpeachProcess()
        {
            while (true)
            {
                if (_speachSource.isPlaying)
                {
                    yield return new WaitForEndOfFrame();                    
                }
                else
                {
                    if (_speachOrder.Count > 0)
                    {
                        SpeachTrack track = _speachOrder.Dequeue();

                        _speachSource.clip = track.clip;
                        _speachSource.volume = _speachVolume * track.volume;
                        _speachSource.Play();

                        float duration = track.clip.length;

                        yield return new WaitForEndOfFrame();
                    }
                    else
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
        }
                
        private AudioTrack FindTrack(AudioTrack[] list, Audio kind)
        {
            foreach (AudioTrack item in list)
            {
                if (item.kind == kind)
                    return item;
            }

            return null;
        }

        private AudioTrack FindTrack(AudioTrack[] list, string name)
        {
            foreach (AudioTrack item in list)
            {
                if (item.clip.name == name)
                    return item;
            }

            return null;
        }

        private SpeachTrack FindTrack(SpeachTrack[] list, Speach kind)
        {
            foreach (SpeachTrack item in list)
            {
                if (item.kind == kind)
                    return item;
            }

            return null;
        }

        private SpeachTrack FindTrack(SpeachTrack[] list, string name)
        {
            foreach (SpeachTrack item in list)
            {
                if (item.clip.name == name)
                    return item;
            }

            return null;
        }
    }
}