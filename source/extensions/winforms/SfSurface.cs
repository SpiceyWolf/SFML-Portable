using SFML.Graphics;
using SFML.System;
using System.Windows.Forms;

using WinControl = System.Windows.Forms.Control;
using Color = SFML.Graphics.Color;
using Designer = SFML.Gui.DesignMode;
using View = SFML.Graphics.View;
using SystemColor = System.Drawing.Color;
using System;

namespace SFML.Gui
{
    /// <summary>
    /// Winforms based SFML Render Surface
    /// </summary>
    public class SfSurface : WinControl
    {
        private Timer _autoDraw = new Timer();
        private Color _clearColor;
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
        [global::System.ComponentModel.DefaultValue(true)]
        public bool AutoDraw
        {
            get => _autoDraw.Enabled;
            set => _autoDraw.Enabled = value;
        }

        /// <summary>
        /// Specifies the interval in
        /// milliseconds to redraw by.
        /// </summary>
        public int AutoDrawInterval
        {
            get => _autoDraw.Interval;
            set => _autoDraw.Interval = value;
        }

        /// <summary>
        /// Returns the internal RenderWindow.
        /// </summary>
        public RenderWindow SfHandle { get; private set; }

        /// <summary>
        /// Changes the color when in designer,
        /// and changes the clear color when in runtime.
        /// </summary>
        public override SystemColor BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                _clearColor = new Color(value.R, value.G, value.B, value.A);
            }
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            if (Designer.Active)
            {
                BackgroundImage = Properties.Resources.Logo;
                BackgroundImageLayout = ImageLayout.Center;
                return;
            }

            // Declare properties
            _autoDraw.Enabled = true;

            _clearColor = new Color(base.BackColor.R, base.BackColor.G, base.BackColor.B, base.BackColor.A);

            SfHandle = new RenderWindow(Handle);
            _view = new View(new FloatRect(0, 0, Width, Height));

            // Set events
            _autoDraw.Tick += (sender, e) => { InvokeRender(); };
            Render += OnRender;
            SizeChanged += (sender, e) =>
            {
                SfHandle.Size = new Vector2u((uint)Width, (uint)Height);
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

            if (SfHandle != null)
            {
                SfHandle.Dispose();
                SfHandle = null;
            }

            if (BackgroundImage != null)
            {
                BackgroundImage.Dispose();
                BackgroundImage = null;
            }

            if (View != null)
            {
                View.Dispose();
                View = null;
            }

            base.Dispose(disposing);
        }

        protected sealed override void OnPaintBackground(PaintEventArgs e)
        {
            if (Designer.Active)
                base.OnPaintBackground(e);
        }

        protected sealed override void OnPaint(PaintEventArgs e) { }

        /// <summary>
        /// This function may be used to invoke a render if wishing
        /// to use Event based rendering without using the AutoDraw.
        /// </summary>
        public void InvokeRender()
        {
            SfHandle.Clear(_clearColor);
            SfHandle.SetView(View ?? _view);
            Render?.Invoke(this, EventArgs.Empty);
            SfHandle.Display();
        }

        #region Draw Calls
        /// <summary> Draws input to the surface. </summary>
        public void Draw(Graphics.Drawable drawable)
        {
            SfHandle.Draw(drawable);
        }

        /// <summary> Draws input to the surface. </summary>
        public void Draw(Graphics.Drawable drawable, RenderStates states)
        {
            SfHandle.Draw(drawable, states);
        }

        /// <summary> Draws input to the surface. </summary>
        public void Draw(Vertex[] vertices, PrimitiveType type)
        {
            SfHandle.Draw(vertices, type);
        }

        /// <summary> Draws input to the surface. </summary>
        public void Draw(Vertex[] vertices, PrimitiveType type, RenderStates states)
        {
            SfHandle.Draw(vertices, type, states);
        }

        /// <summary> Draws input to the surface. </summary>
        public void Draw(Vertex[] vertices, uint start, uint count, PrimitiveType type)
        {
            SfHandle.Draw(vertices, start, count, type);
        }

        /// <summary> Draws input to the surface. </summary>
        public void Draw(Vertex[] vertices, uint start, uint count, PrimitiveType type, RenderStates states)
        {
            SfHandle.Draw(vertices, start, count, type, states);
        }
        #endregion
        
        public event EventHandler Render;
        public virtual void OnRender(object sender, EventArgs e) { }
    }
}