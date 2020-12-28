using System;
using System.Collections.Generic;
using System.IO;

namespace SFML.Audio
{
    public class SoundManager : IDisposable
    {
        private struct SfxInstance
        {
            public Sound Sfx;
            private float _volume;
            public void SetVolume(float volume) { Sfx.Volume = 100f * _volume * volume; }

            public SfxInstance(Sound sfx, float volume, float masterVolume)
            {
                Sfx = sfx;
                _volume = volume > 1.0F ? 1.0F :
                    (volume < 0F ? 0F : volume);
                SetVolume(masterVolume);
            }
        }

        #region Definitions
        private Dictionary<int, SfxInstance> _player = new Dictionary<int, SfxInstance>();
        private Dictionary<string, string> _sounds = new Dictionary<string, string>();
        private float _volume = 1.0f;

        public float Volume
        {
            get => _volume;
            set
            {
                var volume = value > 1.0F ? 1.0F : (value < 0F ? 0F : value);
                _volume = volume;

                if (_player.Count > 0)
                    lock (_player)
                        for (var i = 0; i < _player.Count; i++)
                            _player[i].SetVolume(volume);
            }
        }
        #endregion

        #region Construction
        public SoundBuffer GetSound(string name, bool throwException = true)
        {
            name = name.ToLower();

            if (_sounds.ContainsKey(name))
                return new SoundBuffer(_sounds[name]);

            if (!throwException) return null;
            throw new InvalidOperationException("Key not found!");
        }
        public void Insert(string name, string path, bool throwException = true)
        {
            name = name.ToLower();

            if (_sounds.ContainsKey(name))
            {
                if (!throwException) return;
                throw new InvalidOperationException("Key already exists with that name!");
            }

            _sounds.Add(name, path);
        }
        public void Remove(string name)
        {
            name = name.ToLower();

            if (!_sounds.ContainsKey(name)) return;
            _sounds.Remove(name);
        }
        public void RemoveAll() { _sounds.Clear(); }

        public void Load(string name, string path, bool throwException = true)
        {
            name = name.ToLower();

            if (_sounds.ContainsKey(name))
            {
                if (!throwException) return;
                throw new InvalidOperationException("Key already exists with that name!");
            }
            if (!File.Exists(path))
            {
                if (!throwException) return;
                throw new InvalidOperationException("File does not exist at " + path + "!");
            }

            _sounds.Add(name, path);
        }
        public void LoadAnonymous(string name, string path, bool throwException = true)
        {
            name = name.ToLower();

            if (_sounds.ContainsKey(name))
            {
                if (!throwException) return;
                throw new InvalidOperationException("Key already exists with that name!");
            }

            var exts = new[] { ".ogg", ".wav", ".flac", ".aiff", ".au",
                ".raw", ".paf", ".svx", ".nist", ".voc", ".ircam", ".w64",
                ".mat4", ".mat5", ".pvf", ".htk", ".sds", ".avr",".sd2",
                ".caf", ".wve", ".mpc2k", ".rf64"};
            var file = "";
            foreach (var ext in exts)
                if (File.Exists(path + ext))
                {
                    file = path + ext;
                    break;
                }

            if (file.Trim().Length > 0)
                _sounds.Add(name, file);
        }
        public void LoadDirectory(string path, bool throwException = true)
        {
            var cd = path + "/";
            if (!Directory.Exists(cd))
            {
                if (!throwException) return;
                throw new Exception("Directory '" + path + "' not found!");
            }

            var exts = new[] { ".ogg", ".wav", ".flac", ".aiff", ".au",
                ".raw", ".paf", ".svx", ".nist", ".voc", ".ircam", ".w64",
                ".mat4", ".mat5", ".pvf", ".htk", ".sds", ".avr",".sd2",
                ".caf", ".wve", ".mpc2k", ".rf64"};
            var fCount = 0;
            var fi = new DirectoryInfo(cd).GetFiles("*");
            foreach (var cf in fi)
            {
                var nameLen = cf.Name.Length - 1;
                while (nameLen > 0)
                {
                    if (cf.Name[nameLen] == '.')
                    {
                        nameLen--;
                        break;
                    }
                    nameLen--;
                }
                if (nameLen < 1) continue;

                var name = cf.Name.Substring(0, nameLen).ToLower();
                if (_sounds.ContainsKey(name))
                {
                    if (!throwException) continue;
                    throw new InvalidOperationException("Duplicate Key entry attempt!");
                }

                var file = "";
                foreach (var ext in exts)
                    if (File.Exists(path + ext))
                    {
                        file = path + ext;
                        break;
                    }

                if (file.Trim().Length > 0)
                {
                    _sounds.Add(name, file);
                    fCount++;
                }
            }

            if (fCount < 1)
            {
                if (!throwException) return;
                throw new Exception("File(s) in '" + path + "' not found under any valid extension!");
            }
        }

        public void Dispose()
        {
            RemoveAll();
            _player = null;
            _sounds = null;
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Player
        public void PlaySound(string name, bool loop = false, float relativeVolume = 1.0f)
        {
            if (!_sounds.ContainsKey(name)) return;

            ClearDeadSounds();

            var volume = relativeVolume > 1.0F ? 1.0F :
                (relativeVolume < 0F ? 0F : relativeVolume);

            var sfx = new Sound(new SoundBuffer(_sounds[name]));
            var inst = new SfxInstance(sfx, volume, _volume);
            sfx.Loop = loop;
            sfx.Play();
            _player.Add(_player.Count, inst);
        }

        public void Stop()
        {
            if (_player.Count < 1) return;

            for (var i = 0; i < _player.Count; i++)
            {
                _player[i].Sfx.Stop();
                _player[i].Sfx.Dispose();
            }

            _player.Clear();
        }

        private void ClearDeadSounds()
        {
            var count = _player.Count - 1;
            if (count < 0) return;

            for (var i = count; i >= 0; i--)
            {
                if (_player[i].Sfx == null)
                    _player.Remove(i);
                else if (_player[i].Sfx.Status == SoundStatus.Stopped)
                {
                    _player[i].Sfx.Stop();
                    _player[i].Sfx.Dispose();
                    _player.Remove(i);
                }
            }
        }
        #endregion
    }
}