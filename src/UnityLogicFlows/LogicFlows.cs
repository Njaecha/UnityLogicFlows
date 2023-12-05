using System;
using System.Collections.Generic;
using UnityEngine;

namespace LogicFlows
{
    public class LogicFlows : MonoBehaviour
    {
        public const string version = "0.0.1";

        private Dictionary<string, LogicFlowGraph> graphs = new Dictionary<string, LogicFlowGraph>();

        private Material mat = new Material(Shader.Find("Hidden/Internal-Colored"));

        bool active = false;

        public LogicFlows()
        {

        }

        void OnGUI()
        {
            foreach (LogicFlowGraph graph in graphs.Values)
            {
                graph.ongui();
            }
        }

        void Update()
        {
            if (!active) return;

            foreach (LogicFlowGraph graph in graphs.Values)
            {
                graph.update();
            }
        }

        void OnPostRender()
        {
            if (!active) return;

            // init GL
            GL.PushMatrix();
            mat.SetPass(0);
            GL.LoadOrtho();

            foreach (LogicFlowGraph graph in graphs.Values)
            {
                graph.draw();
            }

            // end GL
            GL.PopMatrix();
        }

        public LogicFlowGraph getGraph(string identifier)
        {
            return graphs[identifier];
        }

        public LogicFlowGraph AddGraph(string identifier)
        {
            return AddGraph(identifier, new Rect(200, 200, 600, 300));
        }

        public LogicFlowGraph AddGraph(string identifier, Rect rect)
        {
            LogicFlowGraph g = new LogicFlowGraph(rect);
            graphs.Add(identifier, g);
            return g;
        }
    }
}
