using System;

namespace LogicFlows
{
    public class LogicFlowOutput_Action : LogicFlowOutput
    {
        private readonly Action<bool> setter;

        public LogicFlowOutput_Action(Action<bool> setter, LogicFlowGraph parentGraph, bool cached = true, int? key = null) : base(parentGraph, cached, key)
        {
            this.setter = setter;
            this.toolTipText = setter.Method.Name;
        }

        public override void InvokeOutput(bool value)
        {
            if (!enabled) return;
            setter.Invoke(value);
        }
    }
}
