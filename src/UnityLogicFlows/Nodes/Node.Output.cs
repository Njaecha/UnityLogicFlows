using UnityEngine;

namespace LogicFlows
{
    public abstract class LogicFlowOutput : LogicFlowNode
    {
        public readonly bool cached;
        private bool? _value;

        protected LogicFlowOutput(LogicFlowGraph parentGraph, bool cached = true, int? key = null) : base(new int?[1], parentGraph, key)
        {
            this.cached = cached;
        }

        protected override Rect initRect()
        {
            return new Rect(50, 50, 70, 20);
        }

        internal override void update()
        {
            handleBody();
            handleInputs();
        }

        internal override void draw()
        {
            drawBody();
            drawInputs();
            drawSymbol();
        }

        protected override bool inputsValid()
        {
            return inputs[0] != null && parentGraph.getNodeAt(inputs[0].Value) != null;
        }

        public override void drawSymbol()
        {
            GL.Begin(GL.QUADS);
            GL.Color(enabled ? getValue() ? trueColor : falseColor : disabledColor);
            GL.Vertex(translateToGL(A + new Vector2(5, 5) * parentGraph.getUIScale()));
            GL.Vertex(translateToGL(B + new Vector2(5, -5) * parentGraph.getUIScale()));
            GL.Vertex(translateToGL(C + new Vector2(-5, -5) * parentGraph.getUIScale()));
            GL.Vertex(translateToGL(D + new Vector2(-5, 5) * parentGraph.getUIScale()));
            GL.End();
        }

        public abstract void InvokeOutput(bool value);

        public void forceUpdate()
        {
            bool? s = inputAt(0)?.getValue();
            if (s.HasValue) InvokeOutput(s.Value);
        }

        public override bool getValue()
        {
            if (inputAt(0) == null) return false;
            bool value = inputAt(0).getValue();
            if (!cached)
            {
                InvokeOutput(value);
                return value;
            }
            if (!_value.HasValue) _value = value;
            if (_value.Value != value)
            {
                InvokeOutput(value);
            }
            _value = value;
            return value;
        }
    }
}
