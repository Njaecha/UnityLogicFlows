using UnityEngine;

namespace LogicFlows
{
    public class LogicFlowNode_NOT : LogicFlowGate
    {
        protected override Rect initRect()
        {
            if (LogicFlows.SmallUI) return new Rect(25, 25, 20, 10);
            return new Rect(50, 50, 40, 20);
        }

        public LogicFlowNode_NOT(LogicFlowGraph parentGraph, int? key = null) : base(new int?[1], parentGraph, key)
        {
            this.toolTipText = "NOT";
        }

        public override void drawSymbol()
        {
            float y = rect.height - rect.height / 5;
            Vector2 p1 = A + new Vector2(rect.width / 2 - y / 1.5f, (rect.height - y) / 2);

            GL.Begin(GL.TRIANGLES);
            GL.Color(enabled ? getValue() ? trueColor : falseColor : disabledColor);
            GL.Vertex(translate(p1));
            GL.Vertex(translate(p1 + new Vector2(y * 1.5f, y / 2)));
            GL.Vertex(translate(p1 + new Vector2(0, y)));
            GL.End();
        }

        protected override void clone()
        {
            var newNode = new LogicFlowNode_NOT(this.parentGraph) { label = this.label, toolTipText = this.toolTipText };
            newNode.setPosition(this.rect.position + new Vector2(20, 20));
        }

        public override bool getValue()
        {
            if (!enabled || inputAt(0) == null) return false;
            return !inputAt(0).getValue();
        }
    }
}
