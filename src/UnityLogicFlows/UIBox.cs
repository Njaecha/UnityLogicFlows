using System;
using UnityEngine;

namespace LogicFlows
{
    public abstract class LogicFlowBox
    {
        private Rect? _rect;
        public Rect rect
        {
            get => getRect();
            internal set => _rect = value;
        }
        // B C
        // A D
        internal Vector2 A { get => rect.position * LogicFlows.UIScale; }
        internal Vector2 B { get => new Vector2(rect.position.x, rect.position.y + rect.height) * LogicFlows.UIScale; }
        internal Vector2 C { get => (rect.position + rect.size) * LogicFlows.UIScale; }
        internal Vector2 D { get => new Vector2(rect.position.x + rect.width, rect.position.y) * LogicFlows.UIScale; }

        // mouse stuff
        protected Vector2? delta;

        public void setPosition(Vector2 pos)
        {
            rect = new Rect(pos, rect.size);
        }

        /// <summary>
        /// Overwrite this to add additional logic to the initial size and position of the node.
        /// </summary>
        /// <returns>The inital size and position of the node</returns>
        protected abstract Rect initRect();
        private Rect getRect()
        {
            if (_rect == null) _rect = initRect();
            return new Rect(_rect.Value.position * LogicFlows.UIScale, _rect.Value.size * LogicFlows.UIScale);
        }

        // helper methods 
        // translate screen coordinates to GL coordinates
        protected static Vector3 translate(Vector3 s)
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

            GL.Vertex(translate(start - offsetV));
            GL.Vertex(translate(start + offsetV));
            GL.Vertex(translate(end + offsetV));
            GL.Vertex(translate(end - offsetV));
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
        internal static Texture2D GetBlackTextureThatWorks()
        {
            if (_halfTransparent != null) return _halfTransparent;

            // Create a new Texture2D with the specified width and height
            Texture2D texture = new Texture2D(2, 2);

            // Set each pixel to half-transparent black
            Color32[] pixels = new Color32[2 * 2];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color32(0, 0, 0, 255); // 128 represents half-transparent alpha
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
