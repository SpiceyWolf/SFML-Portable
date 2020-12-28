using SFML.Audio;
using SFML.Graphics;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using SFML.System;

namespace SFML
{
    public static class Loader
    {
        /// <summary>
        /// Given a file without an extension, this
        /// will return a Music object found under
        /// any of the audio formats compatible with
        /// SFML.
        /// 
        /// Will throw error if true and file is 
        /// not found under any extension.
        /// </summary>
        public static Music AutoMusic(string path, bool throwException = true)
        {
            var exts = new[] { ".ogg", ".wav", ".flac", ".aiff", ".au",
                ".raw", ".paf", ".svx", ".nist", ".voc", ".ircam", ".w64",
                ".mat4", ".mat5", ".pvf", ".htk", ".sds", ".avr",".sd2",
                ".caf", ".wve", ".mpc2k", ".rf64"};

            foreach (var ext in exts)
                if (File.Exists(path + ext)) return new Music(path + ext);

            if (!throwException) return null;
            throw new Exception("File not found under any valid extension!");
        }

        /// <summary>
        /// Given a file without an extension, this
        /// will return a SoundBuffer found under
        /// any of the audio formats compatible with
        /// SFML.
        /// 
        /// Will throw error if true and file is 
        /// not found under any extension.
        /// </summary>
        public static SoundBuffer AutoSound(string path, bool throwException = true)
        {
            var exts = new[] { ".ogg", ".wav", ".flac", ".aiff", ".au",
                ".raw", ".paf", ".svx", ".nist", ".voc", ".ircam", ".w64",
                ".mat4", ".mat5", ".pvf", ".htk", ".sds", ".avr",".sd2",
                ".caf", ".wve", ".mpc2k", ".rf64"};

            foreach (var ext in exts)
                if (File.Exists(path + ext)) return new SoundBuffer(path + ext);

            if (!throwException) return null;
            else throw new Exception("File not found under any valid extension!");
        }

        /// <summary>
        /// Given a file without an extension, this
        /// will return a Texture found under
        /// any of the image formats compatible with
        /// SFML.
        /// 
        /// Will throw error if true and file is 
        /// not found under any extension.
        /// </summary>
        public static Texture AutoTexture(string path, bool throwException = true)
        {
            var exts = new[] { ".bmp", ".dds", ".jpg", ".png", ".tga", ".psd" };

            foreach (var ext in exts)
                if (File.Exists(path + ext)) return new Texture(path + ext);

            if (!throwException) return null;
            throw new Exception("File not found under any valid extension!");
        }

        /// <summary>
        /// Given a directory path, this will return
        /// an array of Textures found under any of
        /// the image formats compatible with SFML.
        /// 
        /// Note: This function detects files named
        /// after the index it would be in memory -
        /// ex: 1.png.
        /// 
        /// Note2: Index starts at 0, and will detect
        /// the highest index file in the directory
        /// as well as fill any empty spaces between
        /// 0 and the high-index with an empty
        /// (no-null) Texture.
        /// 
        /// Will throw error if true and :
        /// - directory is not found, or
        /// - files are not found under any extension.
        /// </summary>
        public static Texture[] AutoTextureStack(string path, bool throwException = true)
        {
            var exts = new[] { ".bmp", ".dds", ".jpg", ".png", ".tga", ".psd" };

            var cd = path + "/";
            if (!Directory.Exists(cd))
            {
                if (!throwException) return null;
                throw new Exception("Directory '" + path + "' not found!");
            }

            var fCount = -1;
            var fIndex = -1;

            var fi = new DirectoryInfo(cd).GetFiles("*");
            foreach (var cf in fi)
            {
                foreach (var ext in exts)
                {
                    if (cf.Name.EndsWith(ext))
                    {
                        // Parse name.
                        int.TryParse(cf.Name.Substring(0, cf.Name.Length - ext.Length),
                            out fIndex);

                        // Make sure a 0 return name is valid.
                        if (fIndex == 0 && !cf.Name.StartsWith("0"))
                            continue;

                        // Raise count if higher than -1.
                        if (fIndex > fCount)
                            fCount = fIndex;

                        break;
                    }
                }
            }

            if (fCount < 0)
            {
                if (!throwException) return null;
                throw new Exception("File(s) in '" + path + "' not found under any valid extension!");
            }

            var tex = new Texture[fCount + 1];
            var texFound = false;

            for (var i = 0; i <= fCount; i++)
            {
                texFound = false;

                foreach (var ext in exts)
                {
                    if (File.Exists(cd + i + ext))
                    {
                        tex[i] = new Texture(cd + i + ext);
                        texFound = true;
                        break;
                    }
                }

                // Give a blank texture to prevent null error.
                if (!texFound)
                    tex[i] = new Texture(1, 1);
            }

            return tex;
        }

        /// <summary>
        /// Given a file without an extension, this
        /// will return a Sprite found under
        /// any of the image formats compatible with
        /// SFML.
        /// 
        /// Will throw error if true and file is 
        /// not found under any extension.
        /// </summary>
        public static Sprite AutoSprite(string path, bool throwException = true)
        {
            return new Sprite(AutoTexture(path, throwException));
        }

        /// <summary>
        /// Given a directory path, this will return
        /// an array of Sprites found under any of
        /// the image formats compatible with SFML.
        /// 
        /// Note: This function detects files named
        /// after the index it would be in memory -
        /// ex: 1.png.
        /// 
        /// Note2: Index starts at 0, and will detect
        /// the highest index file in the directory
        /// as well as fill any empty spaces between
        /// 0 and the high-index with an empty
        /// (no-null) Sprite.
        /// 
        /// Will throw error if true and :
        /// - directory is not found, or
        /// - files are not found under any extension.
        /// </summary>
        public static Sprite[] AutoSpriteStack(string path, bool throwException = true)
        {
            var tex = AutoTextureStack(path, throwException);
            int len = tex.Length;

            var spr = new Sprite[len];
            for (var i = 0; i < len; i++)
                spr[i] = new Sprite(tex[i]);

            return spr;
        }

        /// <summary>
        /// Given a file name without an extension,
        /// this will return a Font found under the
        /// systems global font folder under ttf format.
        /// 
        /// Note: Linux systems requires the installation
        /// of MS Core Fonts package for universal fonts.
        /// 
        /// Will throw error if true and file is not found.
        /// </summary>
        public static Font AutoFont(string name, bool throwException = true)
        {
            if (Portable.Linux)
            {
                var path = "/usr/share/fonts/truetype/msttcorefonts/";
                if (!Directory.Exists(path))
                {
                    if (!throwException) return new Font("");
                    throw new Exception("MS Core Fonts not installed!");
                }

                path += name + ".ttf";
                if (!File.Exists(path))
                {
                    if (!throwException) return new Font("");
                    throw new Exception("Font not found on the system.");
                }

                return new Font(path);
            }

            return new Font(Environment.GetFolderPath(
                            Environment.SpecialFolder.Fonts) +
                            "/" + name + ".ttf");
        }
    }
}