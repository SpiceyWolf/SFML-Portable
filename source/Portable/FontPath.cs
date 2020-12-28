namespace SFML.Graphics
{
    public static class FontPath
    {
        public static string Root()
        {
            if (Portable.Linux)
                return "";
            return "";
        }
    }
}