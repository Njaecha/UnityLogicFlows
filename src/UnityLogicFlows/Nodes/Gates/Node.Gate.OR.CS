using UnityEngine;

namespace LogicFlows
{
    public class LogicFlowNode_OR : LogicFlowGate
    {
        public LogicFlowNode_OR(LogicFlowGraph parentGraph, int? key = null) : base(new int?[2], parentGraph, key)
        {
            this.toolTipText = "OR";
        }

        public override void drawSymbol()
        {
            float y = rect.height - rect.height / 5;
            Vector2 p1 = A + new Vector2(rect.width / 2 - y / 1.5f, (rect.height - y) / 2);

            GL.Begin(GL.TRIANGLE_STRIP);
            GL.Color(enabled ? getValue() ? trueColor : falseColor : disabledColor);
            GL.Vertex(translate(p1));
            GL.Vertex(translate(p1 + new Vector2(y, 0)));
            GL.Vertex(translate(p1 + new Vector2(y / 6, y / 2)));
            GL.Vertex(translate(p1 + new Vector2(y * 1.5f, y / 2)));
            GL.Vertex(translate(p1 + new Vector2(0, y)));
            GL.Vertex(translate(p1 + new Vector2(y, y)));
            GL.End();
        }

        protected override void clone()
        {
            var newNode = new LogicFlowNode_OR(this.parentGraph) { label = this.label, toolTipText = this.toolTipText };
            newNode.setPosition(this.rect.position + new Vector2(20, 20));
        }

        public override bool getValue()
        {
            if (!enabled || inputAt(0) == null || inputAt(1) == null) return false;
            return inputAt(0).getValue() || inputAt(1).getValue();
        }
    }
}
