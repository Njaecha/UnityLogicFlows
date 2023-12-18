using System;
using UnityEngine;

namespace LogicFlows
{
    public abstract class LogicFlowBox
    {
        protected static float UIScale { get => ((float)Screen.height / 2160f) * UIScaleModifier; }
        protected static float UIScaleModifier = 1f;

        private Rect? _rect;
        /// <summary>
        /// Rect with UIScaling enabled
        /// </summary>
        internal Rect rect
        {
            get => getRect();
        }
        // B C
        // A D
        internal Vector2 A { get => rect.position; }
        internal Vector2 B { get => new Vector2(rect.position.x, rect.position.y + rect.height); }
        internal Vector2 C { get => (rect.position + rect.size); }
        internal Vector2 D { get => new Vector2(rect.position.x + rect.width, rect.position.y); }

        public Vector2 positionUI { get => rect.position; }
        public Vector2 sizeUI { get => rect.size; }

        // mouse stuff
        protected Vector2? delta;

        /// <summary>
        /// Sets positon of the rect taking UIScaling into account
        /// </summary>
        /// <param name="pos"></param>
        internal void setPositionUI(Vector2 pos)
        {
            if (!_rect.HasValue) return;
            _rect = new Rect(pos / UIScale, _rect.Value.size);
        }

        /// <summary>
        /// Sets size of the rect taking UIScaling into account
        /// </summary>
        /// <param name="size"></param>
        internal void setSizeUI(Vector2 size)
        {
            if (_rect.HasValue)
            {
                _rect = new Rect(_rect.Value.position, size / UIScale);
            }
        }
        /// <summary>
        /// Sets the Backing rect directly (without UIScaling)
        /// </summary>
        /// <param name="rect"></param>
        internal void setBackingRect(Rect rect)
        {
            _rect = rect;
        }

        /// <summary>
        /// Sets the position of the rect. x: 0-3840 | y: 0-2160
        /// </summary>
        /// <param name="pos"></param>
        public void setPosition(Vector2 pos)
        {
            if (!_rect.HasValue) _rect = initRect();
            _rect = new Rect(pos, _rect.Value.size);
        }

        /// <summary>
        /// Sets the size of the rect. x: 0-3840 | y: 0-2160
        /// </summary>
        /// <param name="size"></param>
        public void setSize(Vector2 size)
        {
            if (!_rect.HasValue) _rect = initRect();
            _rect = new Rect(_rect.Value.position, size);
        }

        public Vector2 getSize()
        {
            if (!_rect.HasValue) _rect = initRect();
            return _rect.Value.size;
        }

        public Vector2 getPosition()
        {
            if (!_rect.HasValue) _rect = initRect();
            return _rect.Value.position;
        }

        /// <summary>
        /// Overwrite this to add additional logic to the initial size and position of the node.
        /// </summary>
        /// <returns>The inital size and position of the node</returns>
        protected abstract Rect initRect();
        private Rect getRect()
        {
            if (_rect == null) _rect = initRect();
            return new Rect(_rect.Value.position * UIScale, _rect.Value.size * UIScale);
        }

        // helper methods 
        // translate screen coordinates to GL coordinates
        protected static Vector3 translateToGL(Vector3 s)
        {
            return translateToGL((Vector2)s);
        }

        protected static Vector3 translateToGL(Vector2 screenCoord)
        {
            return new Vector3(translateXToGL(screenCoord.x), translateYToGL(screenCoord.y), 0);
        }

        protected static float translateXToGL(float screenX)
        {
            return screenX / Screen.width;
        }

        protected static float translateYToGL(float screenY)
        {
            return screenY / Screen.height;
        }

        protected void GL_DrawLineWithWidth(Vector3 start, Vector3 end, float lineWidth)
        {
            Vector3 line = (end - start);
            Vector3 offsetV = (new Vector3(-line.y, line.x, 0)).normalized * lineWidth / 2;

            GL.Vertex(translateToGL(start - offsetV));
            GL.Vertex(translateToGL(start + offsetV));
            GL.Vertex(translateToGL(end + offsetV));
            GL.Vertex(translateToGL(end - offsetV));
        }

        protected static Vector2 translateToIMGUI(Vector2 screenPos)
        {
            return new Vector2(screenPos.x, Screen.height - screenPos.y);
        }

        public static Rect RectFromVectors(Vector2 v1, Vector2 v2)
        {
            float x = v1.x <= v2.x ? v1.x : v2.x;
            float xSize = Math.Abs(v1.x - v2.x);
            float y = v1.y <= v2.y ? v1.y : v2.y;
            float ySize = Math.Abs(v1.y - v2.y);
            return new Rect(x, y, xSize, ySize);
        }

        static Texture2D _halfTransparent = null;
        internal static Texture2D GetBackground()
        {
            if (_halfTransparent != null) return _halfTransparent;

            // Create a new Texture2D with the specified width and height
            Texture2D texture = new Texture2D(2, 2);

            // Set each pixel to half-transparent black
            Color32[] pixels = new Color32[2 * 2];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color32(200, 200, 200, 255); // 128 represents half-transparent alpha
            }

            // Set the pixels to the texture
            texture.SetPixels32(pixels);

            // Apply changes to the texture
            texture.Apply();

            _halfTransparent = texture;
            return texture;
        }
    }
}
