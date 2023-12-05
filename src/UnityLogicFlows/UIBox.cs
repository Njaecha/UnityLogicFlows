using System;
using UnityEngine;

namespace LogicFlows
{
    public abstract class LogicFlowBox
    {
        private Rect? _rect;
        public Rect rect
        {
            get => checkRect();
            internal set => _rect = value;
        }
        // B C
        // A D
        internal Vector2 A { get => rect.position; }
        internal Vector2 B { get => new Vector2(rect.position.x, rect.position.y + rect.height); }
        internal Vector2 C { get => rect.position + rect.size; }
        internal Vector2 D { get => new Vector2(rect.position.x + rect.width, rect.position.y); }

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
        private Rect checkRect()
        {
            if (_rect == null) _rect = initRect();
            return _rect.Value;
        }

        // helper methods 
        // translate screen coordinates to GL coordinates
        protected static Vector3 translate(Vector3 s)
        {
            return translate((Vector2)s);
        }

        protected static Vector3 translate(Vector2 screenCoord)
        {
            return new Vector3(translateX(screenCoord.x), translateY(screenCoord.y), 0);
        }

        protected static float translateX(float screenX)
        {
            return screenX / Screen.width;
        }

        protected static float translateY(float screenY)
        {
            return screenY / Screen.height;
        }

        protected void drawLineWithWidth(Vector3 start, Vector3 end, float lineWidth)
        {
            Vector3 line = (end - start);
            Vector3 offsetV = (new Vector3(-line.y, line.x, 0)).normalized * lineWidth / 2;

            GL.Vertex(translate(start - offsetV));
            GL.Vertex(translate(start + offsetV));
            GL.Vertex(translate(end + offsetV));
            GL.Vertex(translate(end - offsetV));
        }

        protected Vector2 imguiPos(Vector2 screenPos)
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
    }
}
