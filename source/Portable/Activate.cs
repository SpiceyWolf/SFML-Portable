using System;
using System.Diagnostics;
using System.IO;
using SFML.Gui;
#if NET40
using System.Windows.Forms;
#endif

namespace SFML
{
    /// <summary>
    /// This class is created to internalize the dependancy libraries
    /// and allow for unpacking needed files at application startup.
    /// </summary>
    public static class Portable
    {
        /// <summary>
        /// Call this before anything else in your app to have unpack
        /// dependancy files. Also internalizes Platform name so user
        /// app may have platform specific calls.
        /// </summary>
        public static void Activate()
        {
            if (_initialized) return;

            if (IntPtr.Size == 8) _isBit64 = true;
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Unix:
                    if (Directory.Exists("/Applications") &&
                        Directory.Exists("/System") &&
                        Directory.Exists("/Users") &&
                        Directory.Exists("/Volumes"))
                        _isMac = true;
                    else
                        _isLinux = true;
                    break;
                case PlatformID.MacOSX:
                    _isMac = true;
                    break;
                case PlatformID.Win32Windows:
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.WinCE:
                    _isWindows = true;
                    break;
            }

            CheckInstall();
        }
        private static bool _initialized;

        /// <summary> If running on Windows OS, will return true. </summary>
        public static bool Windows { get { Activate(); return _isWindows; } }
        private static bool _isWindows;

        /// <summary> If running on Mac OS, will return true. </summary>
        public static bool Mac { get { Activate(); return _isMac; } }
        private static bool _isMac;

        /// <summary> If running on Linux OS, will return true. </summary>
        public static bool Linux { get { Activate(); return _isLinux; } }
        private static bool _isLinux;

        private static bool _isBit64;
        /// <summary> If running on 32-bit/x86 system, will return true. </summary>
        public static bool X86 { get { Activate(); return !_isBit64; } }
        /// <summary> If running on 64-bit/x64 system, will return true. </summary>
        public static bool X64 { get { Activate(); return _isBit64; } }

        private static void CheckInstall()
        {
#if NET40
            if (_isWindows)
            {
                var path = "C:/Program Files/SFML_2.5.0/";
                path += _isBit64 ? "x64/" : "x86/";
                if (!File.Exists(path + "csfml-audio-2.dll") ||
                    !File.Exists(path + "csfml-graphics-2.dll") ||
                    !File.Exists(path + "csfml-system-2.dll") ||
                    !File.Exists(path + "csfml-window-2.dll")) goto InstallMsg;

                Environment.SetEnvironmentVariable("Path",
                Environment.GetEnvironmentVariable("Path") + ";" +
                Path.GetFullPath(path) + "; ");
            }
            else if (_isMac)
            {
                var path = "SOMEPATHHERE/libcsfml-";
                if (!File.Exists(path + "audio.2.5") ||
                    !File.Exists(path + "graphics.so.2.5") ||
                    !File.Exists(path + "system.so.2.5") ||
                    !File.Exists(path + "window.so.2.5"))
                {
                    Alert("This platform is not supported yet!");
                    Environment.Exit(0);
                }
            }
            else if (_isLinux)
            {
                var path = "/usr/local/lib/sfml_2.5.0/libcsfml-";
                if (!File.Exists(path + "audio.so.2.5") ||
                    !File.Exists(path + "graphics.so.2.5") ||
                    !File.Exists(path + "system.so.2.5") ||
                    !File.Exists(path + "window.so.2.5")) goto InstallMsg;
            }
            else
            {
                Alert("Your operating system was not supported.");
                Environment.Exit(0);
            }
#endif

            // Successful Activation
            _initialized = true;                        
            return;
#if NET40
            InstallMsg:
            InvalidInstall();
            Environment.Exit(0);
#endif
        }

#if NET40
        private static void InvalidInstall()
        {
            Alert("SFML-Portable could not locate SFML installation.\n\n" +
                  "You will now be redirected to SFML installation for your system.");

            var url = "https://github.com/SpiceyWolf/SFML-Portable" +
                      "/raw/master/installer/Binaries/";

            if (_isWindows) url += "(Windows)%20SFML-2.5.0.exe";
            else if (_isMac) Environment.Exit(0); //url += "";
            else if (_isLinux) url += "(Linux)%20sfml-2.5.0.deb";

            Process.Start(url);
        }

        private static void Alert(string message)
        {
            MessageBox.Show(message);
        }
#endif
    }
}