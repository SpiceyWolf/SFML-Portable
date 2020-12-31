using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

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
        public static bool Windows
        {
            get
            {
                Activate();
                return _isWindows;
            }
        }

        private static bool _isWindows;

        /// <summary> If running on Mac OS, will return true. </summary>
        public static bool Mac
        {
            get
            {
                Activate();
                return _isMac;
            }
        }

        private static bool _isMac;

        /// <summary> If running on Linux OS, will return true. </summary>
        public static bool Linux
        {
            get
            {
                Activate();
                return _isLinux;
            }
        }

        private static bool _isLinux;

        private static bool _isBit64;

        /// <summary> If running on 32-bit/x86 system, will return true. </summary>
        public static bool X86
        {
            get
            {
                Activate();
                return !_isBit64;
            }
        }

        /// <summary> If running on 64-bit/x64 system, will return true. </summary>
        public static bool X64
        {
            get
            {
                Activate();
                return _isBit64;
            }
        }

        private static void CheckInstall()
        {
            if (_isWindows)
            {
                var path = "C:/Program Files/SFML_Portable";
                if (!Directory.Exists(path)) InstallCSFML();

                path += $"/{(_isBit64 ? "x64/" : "x86/")}";
                Environment.SetEnvironmentVariable("Path",
                    Environment.GetEnvironmentVariable("Path") + ";" +
                    Path.GetFullPath(path) + "; ");
            }
            else if (_isMac)
            {
                var path = "/Library/Frameworks/sfml_portable";
                if (!Directory.Exists(path)) InstallCSFML();
            }
            else if (_isLinux)
            {
                var path = "/usr/lib/sfml_portable";
                if (!Directory.Exists(path)) InstallCSFML();
            }
            else throw new Exception("Your operating system was not supported.");

            // Successful Activation
            _initialized = true;
        }

        private static void InstallCSFML()
        {
            // File Name
            var fn = "csfml_";
            if (_isWindows) fn += "windows.exe";
            else if (_isMac) fn += "macosx.tar.gz";
            else if (_isLinux) fn += "linux.tar.gz";

            var file = $"{Environment.CurrentDirectory}/{fn}";
            var url = "https://github.com/SpiceyWolf/SFML-Portable" +
                      $"/raw/master/installer/{fn}";

            // Download
            DownloadFile(file, url);

            // Install
            if (_isWindows)
            {
                var p = Process.Start(file);
                p.WaitForInputIdle();
                p.WaitForExit();

                var path = "C:/Program Files/SFML_Portable";
                if (!Directory.Exists(path))
                    throw new Exception("SFML-Portable failed to install.");
            }
            else if (_isMac)
            {
                var path = "/Library/Frameworks/sfml_portable";
                
                InvokeCommand("", $"tar -xzf ./{fn}");
                InvokeCommand("To complete the SFML_Portable required installation you must authorize sudo-",
                    $"sudo cp -r ./sfml_portable {path}");
                
                if (!Directory.Exists(path))
                    throw new Exception("SFML-Portable failed to install.");
            }
            else if (_isLinux)
            {
                var path = "/usr/lib/sfml_portable";
                
                InvokeCommand("", $"tar -xzf ./{fn}");
                InvokeCommand("To complete the SFML_Portable required installation you must authorize sudo-",
                    $"sudo cp -r ./sfml_portable {path}");
                
                if (!Directory.Exists(path))
                    throw new Exception("SFML-Portable failed to install.");
                
                InvokeCommand("", "sudo ldconfig /usr/lib/sfml_portable");
            }

            // Cleanup
            File.Delete(file);
        }

        private static void InvokeCommand(string notif, string command)
        {
            if (notif != "") Console.WriteLine(notif);

            var proc = new Process
            {
                StartInfo =
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{command}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                }
            };

            proc.Start();
            proc.WaitForExit();
        }

        private static void DownloadFile(string file, string url)
        {
            if (!File.Exists(file))
                try
                {
                    var request = WebRequest.Create(url);
                    using (var response = request.GetResponse())
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            var fSize = response.ContentLength;
                            const int bSize = 1024 * 1024;
                            var buf = new byte[bSize];

                            using (var f = new FileStream(file, FileMode.OpenOrCreate))
                            {
                                var len = stream.Read(buf, 0, bSize);

                                while (len > 0)
                                {
                                    f.Write(buf, 0, len);

                                    var progress = (int) (((float) f.Length / fSize) * 100);
                                    Console.Write(
                                        $"\rDownloading CSFML Installer: {progress}%");

                                    len = stream.Read(buf, 0, bSize);
                                    Thread.Sleep(1);
                                }

                                Console.WriteLine();
                            }
                        }
                    }
                }
                catch (Exception err)
                {
                    throw new Exception("Failed to fetch required csfml installer files!" +
                                        "\nReason: " + err);
                }
        }
    }
}