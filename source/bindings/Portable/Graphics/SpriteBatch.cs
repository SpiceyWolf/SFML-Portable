using SFML.System;
using System;
using System.Collections.Generic;

namespace SFML.Graphics
{
    internal interface IBatch : IDisposable { void Display(RenderTarget context); }

    internal struct TextureBatch : IBatch
    {
        private RenderStates _state;
        private Texture _texture;
        private VertexArray _vertices;

        public TextureBatch(Texture texture, RenderStates state)
        {
            _texture = texture;
            _state = state;
            _vertices = new VertexArray(PrimitiveType.TriangleStrip);
        }

        public void Display(RenderTarget context)
        {
            _state.Texture = _texture;
            _vertices.Draw(context, _state);
        }
        public void Dispose() { _vertices.Dispose(); GC.SuppressFinalize(this); }

        public void Draw(Texture tex, IntRect texRect, Vector2f ori,
                         Vector2f pos, Vector2f sca, Color col, float rot)
        {
            var x = texRect.Left;
            var y = texRect.Top;
            var w = texRect.Width;
            var h = texRect.Height;

            var tl = new Vertex((new Vector2f(0, 0) - ori) * sca + pos, col, new Vector2f(x, y));
            var tr = new Vertex((new Vector2f(w, 0) - ori) * sca + pos, col, new Vector2f(x + w, y));
            var br = new Vertex((new Vector2f(w, h) - ori) * sca + pos, col, new Vector2f(x + w, y + h));
            var bl = new Vertex((new Vector2f(0, h) - ori) * sca + pos, col, new Vector2f(x, y + h));

            if (rot != 0f)
            {
                rot = rot * (float)Math.PI / 180f;
                var cRot = (float)Math.Cos(rot);
                var sRot = (float)Math.Sin(rot);

                var tp = tl.Position;
                tl.Position = new Vector2f(tp.X * cRot - tp.Y * sRot + pos.X,
                                           tp.X * sRot + tp.Y * cRot + pos.Y);

                tp = tr.Position;
                tl.Position = new Vector2f(tp.X * cRot - tp.Y * sRot + pos.X,
                                           tp.X * sRot + tp.Y * cRot + pos.Y);

                tp = bl.Position;
                tl.Position = new Vector2f(tp.X * cRot - tp.Y * sRot + pos.X,
                                           tp.X * sRot + tp.Y * cRot + pos.Y);

                tp = br.Position;
                tl.Position = new Vector2f(tp.X * cRot - tp.Y * sRot + pos.X,
                                           tp.X * sRot + tp.Y * cRot + pos.Y);
            }

            _vertices.Append(tl);
            _vertices.Append(tr);
            _vertices.Append(br);
            _vertices.Append(tl);
            _vertices.Append(bl);
            _vertices.Append(br);
        }

        public void Draw(Texture tex, Vertex[] vertices)
        {
            foreach (var v in vertices)
                _vertices.Append(v);
        }

        public bool IsValid(Texture texture, RenderStates state)
        {
            return texture == _texture &&
                   state.BlendMode == _state.BlendMode &&
                   state.Shader == _state.Shader;
        }
    }

    internal struct TextBatch : IBatch
    {
        private RenderStates _state;
        private Text _text;

        public TextBatch(Text text, RenderStates state)
        {
            _state = state;
            _text = text;
        }

        public void Display(RenderTarget context) { _text.Draw(context, _state); }
        public void Dispose() { _text.Dispose(); GC.SuppressFinalize(this); }
    }

    internal struct ShapeBatch : IBatch
    {
        private RenderStates _state;
        private Shape _shape;

        public ShapeBatch(Shape shape, RenderStates state)
        {
            _state = state;
            _shape = shape;
        }

        public void Display(RenderTarget context) { _shape.Draw(context, _state); }
        public void Dispose() { _shape.Dispose(); GC.SuppressFinalize(this); }
    }

    /// <summary>
    /// A render batching system for caching
    /// optimized render calls, improving draw 
    /// speeds and allowing multiple draw calls
    /// to be reusable.
    /// </summary>
    public struct SpriteBatch
    {
        #region Definitions
        private List<IBatch> _batches;
        private TextureBatch? _curBatch;
        public Vector2f RenderOffset { get; set; }
        #endregion

        #region Batch Handlers
        /// <summary> Resets current batch data. </summary>
        public void Reset(Vector2f? offset = null)
        {
            SafetyCheck();
            Flush();

            if (_batches.Count > 0)
                for (var i = 0; i < _batches.Count; i++)
                    _batches[i].Dispose();

            _batches.Clear();
            RenderOffset = offset ?? Vector2f.Zero;
        }

        /// <summary>
        /// Displays current batch data 
        /// to the target context.
        /// </summary>
        public void Display(RenderTarget context)
        {
            SafetyCheck();
            Flush();

            foreach (var batch in _batches)
                batch.Display(context);
        }

        private void SafetyCheck()
        {
            if (_batches == null)
                _batches = new List<IBatch>();
        }
        #endregion

        #region Draw Handlers
        /// <summary>
        /// Performs a fake clear by using a Rectangle, 
        /// thereby allowing Transparency and RenderState
        /// effects to be applied all while being cache-able.
        /// 
        /// Note: RenderTarget is used for aquiring screen bounds.
        /// </summary>
        public void Clear(RenderTarget context, Color color, RenderStates? state = null)
        {
            SafetyCheck();
            Flush();

            if (color == Color.Transparent) return;

            var shapeObj = new RectangleShape(context.Size.ToFloat());
            shapeObj.FillColor = color;

            _batches.Add(new ShapeBatch(shapeObj, state ?? RenderStates.Default));
        }

        /// <summary>
        /// Performs a fake clear by using a Rectangle, 
        /// thereby allowing Transparency and RenderState
        /// effects to be applied all while being cache-able.
        /// 
        /// Note: This overload takes into account the boundaries
        /// specified through the size parameter and RenderOffset.
        /// </summary>
        public void Clear(Vector2f size, Color color, RenderStates? state = null)
        {
            SafetyCheck();
            Flush();

            if (color == Color.Transparent) return;

            var shapeObj = new RectangleShape(size);
            shapeObj.Position = RenderOffset;
            shapeObj.FillColor = color;

            _batches.Add(new ShapeBatch(shapeObj, state ?? RenderStates.Default));
        }

        /// <summary> Draws a texture. </summary>
        public void Draw(Texture texture, IntRect? textureRect = null,
                         Vector2f origin = new Vector2f(),
                         Vector2f position = new Vector2f(),
                         Vector2f? scale = null, Color? color = null,
                         float rotation = 0f, RenderStates? state = null)
        {
            SafetyCheck();

            var curState = state ?? RenderStates.Default;
            if (!_curBatch.HasValue || !_curBatch.Value.IsValid(texture, curState))
            {
                Flush();
                _curBatch = new TextureBatch(texture, curState);
            }

            var rect = (!textureRect.HasValue ||
                        textureRect.Value.Width < 0 ||
                        textureRect.Value.Height < 0) ?
                        new IntRect(0, 0, (int)texture.Size.X, (int)texture.Size.Y) :
                        textureRect.Value;
            _curBatch.Value.Draw(texture, rect, origin, position + RenderOffset,
                                 scale ?? Vector2f.One, color ?? Color.White, rotation);
        }

        /// <summary> 
        /// Draws a texture using specified vertices.
        /// 
        /// Note: Vertices must be structured using
        /// PrimitiveType.TriangleStrip.
        /// </summary>
        public void Draw(Texture texture, Vertex[] vertices, RenderStates? state = null)
        {
            SafetyCheck();
            if (vertices.Length < 1) return;

            var curState = state ?? RenderStates.Default;
            if (!_curBatch.HasValue || !_curBatch.Value.IsValid(texture, curState))
            {
                Flush();
                _curBatch = new TextureBatch(texture, curState);
            }

            _curBatch.Value.Draw(texture, vertices);
        }

        /// <summary> Draws a string. </summary>
        public void Draw(string text = "", Font font = null,
                         Vector2f origin = new Vector2f(),
                         Vector2f position = new Vector2f(),
                         Color? fillColor = null,
                         Color? outlineColor = null,
                         uint size = 30, uint outlineThickness = 0,
                         RenderStates? state = null)
        {
            SafetyCheck();
            Flush();

            var textObj = new Text(text, font, size)
            {
                Origin = origin,
                Position = position + RenderOffset,
                FillColor = fillColor ?? Color.Black,
                OutlineColor = outlineColor ?? Color.Black,
                OutlineThickness = outlineThickness
            };

            _batches.Add(new TextBatch(textObj, state ?? RenderStates.Default));
        }

        /// <summary> Draws a user-defined text object. </summary>
        public void Draw(Text text, RenderStates? state = null)
        {
            SafetyCheck();
            Flush();

            var textObj = new Text(text);
            textObj.Position += RenderOffset;

            _batches.Add(new TextBatch(textObj, state ?? RenderStates.Default));
        }

        /// <summary> Draws a user-defined rectangle object. </summary>
        public void Draw(RectangleShape shape, RenderStates? state = null)
        {
            var shapeObj = new RectangleShape(shape);
            DrawShape(shapeObj, state ?? RenderStates.Default);
        }

        /// <summary> Draws a user-defined circle object. </summary>
        public void Draw(CircleShape shape, RenderStates? state = null)
        {
            var shapeObj = new CircleShape(shape);
            DrawShape(shapeObj, state ?? RenderStates.Default);
        }

        /// <summary> Draws a user-defined convex object. </summary>
        public void Draw(ConvexShape shape, RenderStates? state = null)
        {
            var shapeObj = new ConvexShape(shape);
            DrawShape(shapeObj, state ?? RenderStates.Default);
        }

        private void DrawShape(Shape shape, RenderStates state)
        {
            SafetyCheck();
            Flush();

            shape.Position += RenderOffset;
            _batches.Add(new ShapeBatch(shape, state));
        }

        private void Flush()
        {
            if (!_curBatch.HasValue) return;
            _batches.Add(_curBatch.Value);
            _curBatch = null;
            if (_curBatch.HasValue) return;
        }
        #endregion
    }
}