using System;

namespace LogicFlows
{
    public class LogicFlowInput_Func : LogicFlowInput
    {
        private readonly Func<bool> getter;

        public LogicFlowInput_Func(Func<bool> getter, LogicFlowGraph parentGraph, int? key = null) : base(parentGraph, key)
        {
            this.getter = getter;
            this.toolTipText = getter.Method.Name;
        }

        public override bool getValue()
        {
            if (!enabled) return false;
            return getter.Invoke();
        }

        protected override bool inputsValid()
        {
            return true;
        }
    }
}
