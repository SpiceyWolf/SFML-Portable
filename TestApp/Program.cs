using System;
using SFML;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using TGUI;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            SFML.Portable.Activate();

            var win = new RenderWindow(VideoMode.DesktopMode, "Test SFML");
            win.Size = new Vector2u(600, 400);
            Gui gui = new Gui(win);
            
            EditBox editBoxUsername = new EditBox();
            editBoxUsername.Position = new Vector2f(600 / 6, 400 / 6);
            editBoxUsername.Size = new Vector2f(600 * 2/3, 400 / 8);
            editBoxUsername.DefaultText = "Username";
            gui.Add(editBoxUsername);

            while (win.IsOpen)
            {
                win.DispatchEvents();
                win.Clear(Color.Blue);
                gui.Draw();
                win.Display();
            }

            win.Close();
            win.Dispose();
            
            Console.WriteLine("Hello World!");
        }
    }
}