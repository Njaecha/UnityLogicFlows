using System.Reflection;

namespace LogicFlows
{
    public class LogicFlowOutput_Property : LogicFlowOutput
    {
        private readonly PropertyInfo property;
        private readonly object obj;


        public LogicFlowOutput_Property(PropertyInfo property, object obj, LogicFlowGraph parentGraph, bool cached = true, int? key = null) : base(parentGraph, cached, key)
        {
            if (!property.CanWrite) throw new NodeException($"Property {property.Name} must be writable");
            this.property = property;
            this.obj = obj;
            this.toolTipText = property.Name;
        }

        public override void InvokeOutput(bool value)
        {
            if (!enabled) return;
            property.SetValue(obj, value, null);
        }
    }
}
