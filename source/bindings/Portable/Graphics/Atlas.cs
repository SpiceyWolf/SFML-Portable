using System;
using System.Collections.Generic;
using System.IO;

namespace SFML.Graphics
{
    public class Atlas : IDisposable
    {
        private Dictionary<string, Texture> _textures =
                            new Dictionary<string, Texture>();

        /// <summary>
        /// Returns the texture requested by name.
        /// Name is not case sensitive as all names
        /// are automatically lower-cased on load.
        /// 
        /// Will throw error if true and texture is
        /// not found.
        /// </summary>
        public Texture GetTexture(string name, bool throwException = true)
        {
            name = name.ToLower();

            if (_textures.ContainsKey(name))
                return _textures[name];

            if (!throwException) return null;
            else throw new InvalidOperationException("Key not found!");
        }

        /// <summary>
        /// Inserts a texture under specified name.
        /// 
        /// Will throw error if true and the specified
        /// name is already occupied.
        /// </summary>
        public void Insert(Texture texture, string name, bool throwException = true)
        {
            name = name.ToLower();

            if (_textures.ContainsKey(name))
            {
                if (!throwException) return;
                else throw new InvalidOperationException("Key already exists with that name!");
            }

            _textures.Add(name, texture);
        }

        /// <summary>
        /// Removes a texture under specified name if
        /// it exists in the atlas without disposing 
        /// the texture data.
        /// 
        /// Use Unload(name) to remove the texture from
        /// memory completely (with dispose).
        /// </summary>
        public void Remove(string name)
        {
            name = name.ToLower();

            if (!_textures.ContainsKey(name)) return;
            _textures.Remove(name);
        }

        /// <summary>
        /// Loads a texture into memory from specified path
        /// and added to Atlas under specified name.
        /// 
        /// Will throw error if true and the specified
        /// name is already occupied.
        /// </summary>
        public void Load(string path, string name, bool throwException = true)
        {
            name = name.ToLower();

            if (_textures.ContainsKey(name))
            {
                if (!throwException) return;
                throw new InvalidOperationException("Key already exists with that name!");
            }

            _textures.Add(name, new Texture(path));
        }

        /// <summary>
        /// Loads a texture into memory from specified path
        /// using Loader.AutoTexture(name) and added to Atlas
        /// under specified name.
        /// 
        /// Will throw error if true and the specified
        /// name is already occupied.
        /// </summary>
        public void LoadAnonymous(string path, string name, bool throwException = true)
        {
            name = name.ToLower();

            if (_textures.ContainsKey(name))
            {
                if (!throwException) return;
                throw new InvalidOperationException("Key already exists with that name!");
            }

            _textures.Add(name, Loader.AutoTexture(path));
        }

        /// <summary>
        /// Loads a texture stack into memory from specified path
        /// using Loader.AutoTextureStack(name) and added to Atlas
        /// under specified name pattern resembling name_X where
        /// X = the textures index as found in the directory.
        /// 
        /// Will throw error if true and the specified
        /// name is already occupied.
        /// </summary>
        public void LoadStack(string path, string name, bool throwException = true)
        {
            name = name.ToLower();

            var stack = Loader.AutoTextureStack(path);
            if (stack.Length < 1)
            {
                if (!throwException) return;
                throw new InvalidOperationException("Texture stack not found!");
            }

            for (var i = 0; i < stack.Length; i++)
            {
                if (_textures.ContainsKey(name + "_" + i))
                {
                    if (!throwException) return;
                    throw new InvalidOperationException("Key(s)? already exists with that name pattern!");
                }

                _textures.Add(name + "_" + i, stack[i]);
            }
        }

        /// <summary>
        /// Loads a texture collection into memory from specified 
        /// path with compatible extensions retaining the file names
        /// without extension and added to Atlas.
        /// 
        /// Will throw error if true and the specified
        /// name is already occupied.
        /// </summary>
        public void LoadDirectory(string path, bool throwException = true)
        {
            var cd = path + "/";
            if (!Directory.Exists(cd))
            {
                if (!throwException) return;
                throw new Exception("Directory '" + path + "' not found!");
            }

            var exts = new[] { ".bmp", ".dds", ".jpg", ".png", ".tga", ".psd" };
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
                if (_textures.ContainsKey(name))
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
                    _textures.Add(name, new Texture(file));
                    fCount++;
                }
            }

            if (fCount < 1)
            {
                if (!throwException) return;
                throw new Exception("File(s) in '" + path + "' not found under any valid extension!");
            }
        }

        /// <summary> 
        /// Unloads a texture from memory under 
        /// specified name using a Dispose() call. 
        /// </summary>
        public void Unload(string name)
        {
            if (!_textures.ContainsKey(name)) return;

            _textures[name].Dispose();
            _textures.Remove(name);
        }

        /// <summary>
        /// Unloads all textures from memory
        /// using Dispose() calls. 
        /// </summary>
        public void UnloadAll()
        {
            if (_textures.Count < 1) return;

            foreach (var tex in _textures.Values)
                tex.Dispose();

            _textures.Clear();
        }

        /// <summary>
        /// Unloads all textures from memory then cleans up this object.
        /// </summary>
        public void Dispose()
        {
            UnloadAll(); _textures = null;
            GC.SuppressFinalize(this);
        }
    }
}