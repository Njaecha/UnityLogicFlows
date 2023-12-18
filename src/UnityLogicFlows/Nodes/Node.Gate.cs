using UnityEngine;


namespace LogicFlows
{
    public abstract class LogicFlowGate : LogicFlowNode
    {
        protected LogicFlowGate(int?[] inputs, LogicFlowGraph parentGraph, int? key = null) : base(inputs, parentGraph, key)
        {

        }

        protected abstract void clone();

        protected new void handleBody()
        {
            base.handleBody();
            Event current = Event.current;
            if (current.keyCode == KeyCode.D && current.modifiers == EventModifiers.Control && current.type == EventType.KeyDown && parentGraph.selectedNodes.Contains(index))
            {
                clone();
            }
        }

        protected override Rect initRect()
        {
            if (LogicFlows.SmallUI) return new Rect(25, 25, 30, 20);
            return new Rect(50, 50, 60, 40);
        }

        internal override void update()
        {
            handleBody();
            handleOutput();
            handleInputs();
        }

        internal override void draw()
        {
            drawBody();
            drawOutput();
            drawInputs();
            drawSymbol();
        }

        protected override bool inputsValid()
        {
            bool valid = true;
            for (int i = 0; i < inputs.Length; i++)
            {
                if (inputAt(i) == null) valid = false;
            }
            return valid;
        }
    }
}
