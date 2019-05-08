using SFML.Graphics;
using SFML.System;
using Eto.Drawing;
using Eto.Forms;

using Color = SFML.Graphics.Color;
using Designer = SFML.Gui.DesignMode;
using EtoColor = Eto.Drawing.Color;
using System;

namespace SFML.Gui
{
    /// <summary>
    /// Eto based SFML Render Surface
    /// </summary>
    public class SfSurface : Eto.Forms.Drawable
    {
        private UITimer _autoDraw = new UITimer();
        private Color _clearColor;
        private RenderWindow _handle;
        private Bitmap _img;
        private View _view;

        /// <summary>
        /// Specifies a custom view. If null,
        /// will use the default view.
        /// </summary>
        public View View;

        /// <summary>
        /// If true, will automatically redraw
        /// on the specified interval.
        /// </summary>
        public bool AutoDraw
        {
            get => _autoDraw.Started;
            set
            {
                if (value && !_autoDraw.Started)
                    _autoDraw.Stop();
                else if (!value && _autoDraw.Started)
                    _autoDraw.Start();
            }
        }

        /// <summary>
        /// Specifies the interval in
        /// seconds to redraw by.
        /// </summary>
        public double AutoDrawInterval
        {
            get => _autoDraw.Interval;
            set => _autoDraw.Interval = value;
        }

        /// <summary>
        /// Returns the internal RenderWindow.
        /// </summary>
        public RenderWindow SfHandle { get { return _handle; } }

        /// <summary>
        /// Changes the color when in designer,
        /// and changes the clear color when in runtime.
        /// </summary>
        public new EtoColor BackgroundColor
        {
            get => base.BackgroundColor;
            set
            {
                base.BackgroundColor = value;
                _clearColor = new Color(
                    (byte)value.Rb,
                    (byte)value.Gb,
                    (byte)value.Bb,
                    (byte)value.Ab);
            }
        }

        public SfSurface()
        {
            Size = new Size(100, 100);

            if (Designer.Active)
            {
                using(var stream = new global::System.IO.MemoryStream())
                {
                    Gui.Properties.Resources.Logo.Save(stream, 
                        global::System.Drawing.Imaging.ImageFormat.Png);
                    stream.Position = 0;
                    _img = new Bitmap(stream);
                }
                
                return;
            }

            // Make sure hardware rendering is supported otherwise SFML doesnt work.
            if (SupportsCreateGraphics)
                _handle = new RenderWindow(NativeHandle);

            // Declare properties
            _autoDraw.Interval = 0.1f;
            _autoDraw.Start();

            var color = base.BackgroundColor;
            _clearColor = new Color((byte)color.Rb, (byte)color.Gb, (byte)color.Bb, (byte)color.A);
            _view = new View(new FloatRect(0, 0, Width, Height));

            // Set events
            _autoDraw.Elapsed += (sender, e) => { InvokeRender(); };
            Render += OnRender;
            SizeChanged += (sender, e) =>
            {
                _handle.Size = new Vector2u((uint)Width, (uint)Height);
                _view = new View(new FloatRect(0, 0, Width, Height));
            };
        }
        
        protected override void Dispose(bool disposing)
        {
            if (_autoDraw != null)
            {
                _autoDraw.Dispose();
                _autoDraw = null;
            }

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

            if (_img != null)
            {
                _img.Dispose();
                _img = null;
            }

            if (View != null)
            {
                View.Dispose();
                View = null;
            }

            base.Dispose(disposing);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!Designer.Active) return;

            var x = Width / 2 - _img.Width / 2;
            var y = Height / 2 - _img.Height / 2;

            base.OnPaint(e);
            e.Graphics.DrawImage(_img, x, y);
        }

        /// <summary>
        /// This function may be used to invoke a render if wishing
        /// to use Event based rendering without using the AutoDraw.
        /// </summary>
        public void InvokeRender()
        {
            _handle.Clear(_clearColor);
            _handle.SetView(View ?? _view);
            Render?.Invoke(this, EventArgs.Empty);
            _handle.Display();
        }

        #region Draw Calls
        /// <summary> Draws input to the surface. </summary>
        public void Draw(Graphics.Drawable drawable)
        {
            _handle.Draw(drawable);
        }

        /// <summary> Draws input to the surface. </summary>
        public void Draw(Graphics.Drawable drawable, RenderStates states)
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
    }
}