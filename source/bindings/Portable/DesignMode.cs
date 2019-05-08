using System.ComponentModel;
using System.Diagnostics;

namespace SFML.Gui
{
    /// <summary>
    /// Helper class to prevent code access outside the Visual Designer.
    /// May be used as well to stop the Designer after an Eto.Form
    /// InitializeComponent function to prevent errors while declaring
    /// other SFML content.
    /// </summary>
    public static class DesignMode
    {
        private static bool _initialized;
        private static bool _active;

        /// <summary>
        /// If running in Visual Designer, will return true.
        /// </summary>
        public static bool Active
        {
            get
            {
                if (!_initialized)
                {
                    var process = Process.GetCurrentProcess().ProcessName;
                    _active = LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
                              process == "devenv" || process == "XamarinStudio" || process == "MonoDevelop";
                    _initialized = true;
                }

                return _active;
            }
        }
    }
}