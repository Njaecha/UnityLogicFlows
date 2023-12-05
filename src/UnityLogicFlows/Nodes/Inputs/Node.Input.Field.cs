using System.Reflection;

namespace LogicFlows
{
    public class LogicFlowInput_Field : LogicFlowInput
    {
        private readonly FieldInfo fieldInfo;
        private readonly object obj;

        public LogicFlowInput_Field(FieldInfo fieldInfo, object obj, LogicFlowGraph parentGraph, int? key = null) : base(parentGraph, key)
        {
            this.fieldInfo = fieldInfo;
            this.toolTipText = fieldInfo.Name;
            this.obj = obj;
        }

        public override bool getValue()
        {
            if (!enabled) return false;
            object v = fieldInfo.GetValue(obj);
            return v is bool ? (bool)v : false;
        }

        protected override bool inputsValid()
        {
            return true;
        }
    }
}
