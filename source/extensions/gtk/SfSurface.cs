using Cairo;
using SFML.Graphics;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;

using Color = SFML.Graphics.Color;
using Designer = SFML.Gui.DesignMode;
using GtkColor = Cairo.Color;
using MemoryStream = System.IO.MemoryStream;

namespace SFML.Gui
{
    [ToolboxItem(true)]
    public class SfSurface : Gtk.Image, IDisposable
    {
        private Color _clearColor;
        private RenderWindow _handle;
        private View _view;

        /// <summary>
        /// Specifies a custom view. If null,
        /// will use the default view.
        /// </summary>
        public View View;

        /// <summary>
        /// Returns the internal RenderWindow.
        /// </summary>
        public RenderWindow SfHandle { get { return _handle; } }

        /// <summary>
        /// Changes the color when in designer,
        /// and changes the clear color when in runtime.
        /// </summary>
        public GtkColor Color
        {
            get => new GtkColor(_clearColor.R / 255, _clearColor.G / 255, _clearColor.B / 255);
            set => _clearColor = new Color((byte)(255 * value.R),
                                           (byte)(255 * value.G),
                                           (byte)(255 * value.B));
        }

        /// <summary>
        /// Changes the color when in designer,
        /// and changes the clear color when in runtime.
        /// </summary>
        public double ColorBlue
        {
            get { return _clearColor.B / 255; }
            set { _clearColor.B = (byte)(255 * value); }
        }

        /// <summary>
        /// Changes the color when in designer,
        /// and changes the clear color when in runtime.
        /// </summary>
        public double ColorGreen
        {
            get { return _clearColor.G / 255; }
            set { _clearColor.G = (byte)(255 * value); }
        }

        /// <summary>
        /// Changes the color when in designer,
        /// and changes the clear color when in runtime.
        /// </summary>
        public double ColorRed
        {
            get { return _clearColor.R / 255; }
            set { _clearColor.R = (byte)(255 * value); }
        }

        public new Gdk.Pixbuf Pixbuf
        {
            get => null;
            set { return; }
        }

        public SfSurface()
        {
            if (Designer.Active)
            {
                using (var stream = new MemoryStream())
                {
                    Properties.Resources.Logo.Save(stream,
                        global::System.Drawing.Imaging.ImageFormat.Png);
                    stream.Position = 0;
                    base.Pixbuf = new Gdk.Pixbuf(stream);
                }
                return;
            }

            // Make sure handle can be detected.
            Gtk.Application.Invoke(delegate
            {
                if (Portable.Windows)
                {
                    try { _handle = new RenderWindow(gdk_win32_drawable_get_handle(Window.Handle)); }
                    catch { throw new Exception("Incorrect or missing installation of GTK#."); }
                }
                else if (Portable.Linux)
                {
                    try { _handle = new RenderWindow(gdk_x11_drawable_get_xid(Window.Handle)); }
                    catch { throw new Exception("Incompatible version of Linux or missing installation of gtk."); }
                }
                else if (Portable.Mac)
                {
                    try { _handle = new RenderWindow(gdk_quartz_window_get_nswindow(Window.Handle)); }
                    catch { throw new Exception("Incompatible version of Mac or missing installation of gtk."); }
                }
                else
                    throw new Exception("Operating system could not be detected. Possible error due " +
                                        "to not using CSFML.Activate() at start of program or OS incompatible!");

                GLib.Idle.Add(new GLib.IdleHandler(delegate { QueueDraw(); return true; }));
            });

            // Declare properties
            _clearColor = new Color(50, 160, 160);
            _view = new View(new FloatRect(0, 0, AllocatedWidth, AllocatedHeight));

            // Set events
            Render += OnRender;
            SizeAllocated += (o, args) =>
            {
                _view = new View(new FloatRect(0, 0,
                                    args.Allocation.Width,
                                    args.Allocation.Height));
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (_view != null)
            {
                _view.Dispose();
                _view = null;
            }

            if (_handle != null)
            {
                _handle.Dispose();
                _handle = null;
            }
            
            if (View != null)
            {
                View.Dispose();
                View = null;
            }

            base.Dispose(disposing);
        }

        protected override bool OnDrawn(Context cr)
        {
            if (Designer.Active)
            {
                var w = AllocatedWidth;
                var h = AllocatedHeight;

                cr.SetSourceColor(Color);
                cr.LineWidth = 0;
                cr.Rectangle(0, 0, w, h);
                cr.Fill();

                var img = base.Pixbuf;
                var iw = img.Width;
                var ih = img.Height;
                var x = w / 2 - iw / 2;
                var y = h / 2 - ih / 2;

                var sf = new ImageSurface(base.Pixbuf.Handle, 
                    Format.Argb32, iw, ih, img.Rowstride);
                cr.SetSourceRGB(1, 1, 1);
                cr.SetSourceSurface(sf, x, y);
                cr.Rectangle(x, y, iw, ih);
                cr.Fill();
            }
            else
            {
                _handle.Clear(_clearColor);
                _handle.SetView(View ?? _view);
                Render?.Invoke(this, EventArgs.Empty);
                _handle.Display();
            }

            return true;
        }

        #region Draw Calls
        /// <summary> Draws input to the surface. </summary>
        public void Draw(Drawable drawable)
        {
            _handle.Draw(drawable);
        }

        /// <summary> Draws input to the surface. </summary>
        public void Draw(Drawable drawable, RenderStates states)
        {
            _handle.Draw(drawable, states);
        }

        /// <summary> Draws input to the surface. </summary>
        public void Draw(Vertex[] vertices, PrimitiveType type)
        {
            _handle.Draw(vertices, type);
        }

        /// <summary> Draws input to the surface. </summary>
        public void Draw(Vertex[] vertices, PrimitiveType type, RenderStates states)
        {
            _handle.Draw(vertices, type, states);
        }

        /// <summary> Draws input to the surface. </summary>
        public void Draw(Vertex[] vertices, uint start, uint count, PrimitiveType type)
        {
            _handle.Draw(vertices, start, count, type);
        }

        /// <summary> Draws input to the surface. </summary>
        public void Draw(Vertex[] vertices, uint start, uint count, PrimitiveType type, RenderStates states)
        {
            _handle.Draw(vertices, start, count, type, states);
        }
        #endregion

        public event EventHandler Render;
        public virtual void OnRender(object sender, EventArgs e) { }

        [SuppressUnmanagedCodeSecurity, DllImport("libgdk-win32-2.0-0.dll")]
        public static extern IntPtr gdk_win32_drawable_get_handle(IntPtr handle);

        [SuppressUnmanagedCodeSecurity, DllImport("gdk-x11-2.0")]
        static extern IntPtr gdk_x11_drawable_get_xid(IntPtr handle);

        [SuppressUnmanagedCodeSecurity, DllImport("libgdk-quartz-2.0.0.dylib")]
        static extern IntPtr gdk_quartz_window_get_nswindow(IntPtr handle);
    }
}
