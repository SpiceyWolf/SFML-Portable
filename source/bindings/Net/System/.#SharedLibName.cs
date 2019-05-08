namespace SFML.System
{
    public static class CSFML
    {
#if !asdNET40
        public const string audio = "csfml-audio-2";
        public const string graphics = "csfml-graphics-2";
        public const string system = "csfml-system-2";
        public const string window = "csfml-window-2";
#else
        public const string audio = "csfml-audio";
        public const string graphics = "csfml-graphics";
        public const string system = "csfml-system";
        public const string window = "csfml-window";
#endif
    }
}
