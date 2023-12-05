using System.Reflection;

namespace LogicFlows
{
    public class LogicFlowOutput_Field : LogicFlowOutput
    {
        private readonly FieldInfo field;
        private readonly object obj;

        public LogicFlowOutput_Field(FieldInfo field, object obj, LogicFlowGraph parentGraph, bool cached = true, int? key = null) : base(parentGraph, cached, key)
        {
            this.field = field;
            this.obj = obj;
            this.toolTipText = field.Name;
        }

        public override void InvokeOutput(bool value)
        {
            if (!enabled) return;
            field.SetValue(obj, value);
        }
    }
}
