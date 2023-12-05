using System.Reflection;

namespace LogicFlows
{
    public class LogicFlowInput_Property : LogicFlowInput
    {
        private readonly PropertyInfo propertyInfo;
        private readonly object obj;

        public LogicFlowInput_Property(PropertyInfo propertyInfo, object obj, LogicFlowGraph parentGraph, int? key = null) : base(parentGraph, key)
        {
            if (!propertyInfo.CanRead) throw new NodeException($"Property {propertyInfo.Name} must be readable");
            this.propertyInfo = propertyInfo;
            this.toolTipText = propertyInfo.Name;
            this.obj = obj;
        }

        public override bool getValue()
        {
            if (!enabled) return false;
            object v = propertyInfo.GetValue(obj, null);
            return v is bool ? (bool)v : false;
        }

        protected override bool inputsValid()
        {
            return true;
        }
    }
}
