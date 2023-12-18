using UnityEngine;

namespace LogicFlows
{
    public abstract class LogicFlowInput : LogicFlowNode
    {
        protected LogicFlowInput(LogicFlowGraph parentGraph, int? key = null) : base(new int?[0], parentGraph, key)
        {

        }

        protected override Rect initRect()
        {
            if (LogicFlows.SmallUI) return new Rect(25, 25, 35, 10);
            return new Rect(50, 50, 70, 20);
        }

        internal override void update()
        {
            handleBody();
            handleOutput();
        }

        internal override void draw()
        {
            drawBody();
            drawOutput();
            drawSymbol();
        }

        public override void drawSymbol()
        {
            GL.Begin(GL.QUADS);
            GL.Color(enabled ? getValue() ? trueColor : falseColor : disabledColor);
            GL.Vertex(translate(rect.center + new Vector2(-10, -10) * (LogicFlows.SmallUI ? 0.5f : 1f)));
            GL.Vertex(translate(rect.center + new Vector2(-10, 10) * (LogicFlows.SmallUI ? 0.5f : 1f)));
            GL.Vertex(translate(rect.center + new Vector2(10, 10) * (LogicFlows.SmallUI ? 0.5f : 1f)));
            GL.Vertex(translate(rect.center + new Vector2(10, -10) * (LogicFlows.SmallUI ? 0.5f : 1f)));
            GL.End();
        }
    }
}
