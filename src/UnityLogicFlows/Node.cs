using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace LogicFlows
{
    public abstract class LogicFlowNode : LogicFlowBox
    {
        public readonly int index;
        protected readonly int?[] inputs;
        public int inputAmount { get => inputs.Length; }
        protected readonly LogicFlowGraph parentGraph;

        public LogicFlowNode inputAt(int i)
        {
            if (!(inputs.Length > i)) return null;
            if (inputs[i].HasValue)
            {
                LogicFlowNode n = parentGraph.getNodeAt(inputs[i].Value);
                if (n == null && !parentGraph.isLoading) inputs[i] = null;
                return n;
            }
            return null;
        }

        public LogicFlowNode(int?[] inputs, LogicFlowGraph parentGraph, int? key = null)
        {
            this.parentGraph = parentGraph;
            this.inputs = inputs;
            if (key.HasValue) index = parentGraph.AddNode(key.Value, this);
            else index = parentGraph.AddNode(this);
        }

        public static Color nodeColor = Color.Lerp(Color.white, Color.gray, 0.5f);
        public static Color borderColor = Color.black;
        public static Color invalidColor = Color.red;
        public static Color outputColor = Color.gray;
        public static Color inputColor = Color.gray;
        public static Color connectorHoverColor = Color.magenta;
        public static Color lineColor = Color.blue;
        public static Color symbolColor = Color.black;
        public static Color trueColor = Color.green;
        public static Color falseColor = Color.red;
        public static Color selectedColor = Color.Lerp(nodeColor, Color.yellow, 0.5f);
        public static Color disabledColor = Color.Lerp(Color.gray, Color.black, 0.4f);

        public string toolTipText;
        public string label;

        public bool mouseOver { get; private set; }
        private bool draggingNode = false;
        internal static bool draggingAnyNode = false;

        public bool deletable = true;
        public EventHandler<NodeDeletedEventArgs> nodeDeletedEvent;

        public bool outputHovered { get; private set; } = false;
        public int? inputHovered { get; private set; } = null;

        protected bool hasOutput = true;

        internal Vector2 outputScreenPos { get => D + (C - D) / 2 + new Vector2(parentGraph.getUIScale() * 10, 0); }

        public bool enabled { get; set; } = true;

        public abstract bool getValue();

        public abstract void drawSymbol();

        internal abstract void update();


        /// <summary>
        /// Set an input for this note
        /// </summary>
        /// <param name="sourceIndex">nodeIndex of the connected node</param>
        /// <param name="i">input index</param>
        public void SetInput(int sourceIndex, int? i = null)
        {
            if (!parentGraph.isLoading && parentGraph.getNodeAt(sourceIndex) == null) return;
            if (!i.HasValue)
            {
                if (inputHovered.HasValue) i = inputHovered.Value;
                else return;
            }
            inputs[i.Value] = sourceIndex;
        }

        internal void drawLabel()
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.box);
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.fontSize = (int)(14f * parentGraph.getUIScale());
            labelStyle.normal.textColor = Color.black;
            labelStyle.padding.top = labelStyle.padding.bottom = labelStyle.padding.left = labelStyle.padding.right = 0;
            labelStyle.normal.background = GetBackground();

            if (!string.IsNullOrEmpty(label)) GUI.Label(new Rect(parentGraph.A.x + B.x, Screen.height - (parentGraph.A.y + B.y) - (parentGraph.getUIScale() * 25), C.x - B.x, parentGraph.getUIScale() * 20), label, labelStyle);
        }

        internal void drawTooltip()
        {
            GUIStyle tooltipStyle = new GUIStyle(GUI.skin.box);
            tooltipStyle.alignment = TextAnchor.MiddleCenter;
            tooltipStyle.normal.textColor = Color.yellow;

            if (mouseOver && !string.IsNullOrEmpty(toolTipText))
            {
                Vector2 mouse = translateToIMGUI(Input.mousePosition);
                GUI.Label(new Rect(mouse.x, mouse.y - 30, toolTipText.Length * 8 + 16, 20), toolTipText, tooltipStyle);
            }
        }

        protected void handleBody()
        {
            Rect rect = new Rect(parentGraph.A + base.rect.position, base.rect.size);
            mouseOver = rect.Contains(Input.mousePosition);
            Rect nodeEatInputRect = new Rect(parentGraph.A + base.rect.position + new Vector2(-15, -15), base.rect.size + new Vector2(30, 30));
            if (nodeEatInputRect.Contains(Input.mousePosition)) parentGraph.eatingInput = true;

            Event current = Event.current;

            if (current.button == 0 && current.type == EventType.MouseDown && !delta.HasValue) delta = (Vector2)Input.mousePosition - rect.position;

            if (mouseOver
                && (current.type == EventType.MouseDrag || current.type == EventType.MouseDown)
                && current.button == 0
                && !draggingAnyNode
                && !parentGraph.dragConnectionSourceIndex.HasValue
                && !parentGraph.selectionStart.HasValue)
            {
                draggingAnyNode = true;
                draggingNode = true;
            }

            if (current.type == EventType.MouseUp)
            {
                draggingNode = false;
                draggingAnyNode = false;
                delta = null;
            }

            if (draggingNode || (draggingAnyNode && parentGraph.selectedNodes.Contains(index)))
            {
                if (!delta.HasValue) return;
                Vector2 m = ((Vector2)Input.mousePosition - parentGraph.A) - delta.Value;
                setPositionUI(m);
            }

            if (deletable && parentGraph.selectedNodes.Contains(index) && current.keyCode == parentGraph.KeyNodeDelete && current.type == EventType.KeyDown)
            {
                nodeDeletedEvent?.Invoke(this, new NodeDeletedEventArgs() { index = index, parentGraph = parentGraph });
                parentGraph.RemoveNode(index);
            }

            if (current.keyCode == parentGraph.KeyNodeDisable && current.modifiers == EventModifiers.Alt && current.type == EventType.KeyDown && parentGraph.selectedNodes.Contains(index))
            {
                enabled = !enabled;
            }

        }

        protected void handleInputs()
        {
            Event current = Event.current;
            // Inputs
            bool hoveringAnyInput = false;
            for (int i = 0; i < inputs.Length; i++)
            {
                Rect inputHoverRect = new Rect(parentGraph.A + getInputPoint(i) + new Vector2(parentGraph.getUIScale() * -15, parentGraph.getUIScale() * -10), new Vector2(parentGraph.getUIScale() * 15, parentGraph.getUIScale() * 20));
                if (inputHoverRect.Contains(Input.mousePosition))
                {
                    inputHovered = i;
                    hoveringAnyInput = true;
                    if (current.type == EventType.MouseDown && current.button == 1 && inputAt(i) != null)
                    {
                        inputs[i] = null; // remove input connection by right clicking it
                    }
                    if (current.type == EventType.MouseDrag 
                        && current.button == 0 
                        && inputAt(i) != null 
                        && !parentGraph.dragConnectionSourceIndex.HasValue 
                        && !draggingAnyNode 
                        && !parentGraph.selectionStart.HasValue)
                    {
                        parentGraph.dragConnectionSourceIndex = inputAt(i).index;
                        inputs[i] = null;
                    }
                }
            }
            if (!hoveringAnyInput) inputHovered = null;
        }

        protected void handleOutput()
        {
            // Output
            Rect outputRect = new Rect(parentGraph.A + D + (C - D) / 2 + new Vector2(0, parentGraph.getUIScale() * -10), new Vector2(parentGraph.getUIScale() * 15, parentGraph.getUIScale() * 20));
            if (outputRect.Contains(Input.mousePosition))
            {
                outputHovered = true;
            }
            else outputHovered = false;
        }

        /// <summary>
        /// must be run from within OnPostRender()
        /// </summary>
        internal abstract void draw();

        protected void drawBody()
        {
            // draw node border
            GL.Begin(GL.QUADS);
            GL.Color(enabled ? inputsValid() ? borderColor : invalidColor : disabledColor);
            GL.Vertex(translateToGL(A + new Vector2(-2, -2) * parentGraph.getUIScale()));
            GL.Vertex(translateToGL(B + new Vector2(-2, 2) * parentGraph.getUIScale()));
            GL.Vertex(translateToGL(C + new Vector2(2, 2) * parentGraph.getUIScale()));
            GL.Vertex(translateToGL(D + new Vector2(2, -2) * parentGraph.getUIScale()));
            GL.End();

            // draw node background
            GL.Begin(GL.QUADS);
            GL.Color(parentGraph.selectedNodes.Contains(index) ? selectedColor : nodeColor);
            GL.Vertex(translateToGL(A));
            GL.Vertex(translateToGL(B));
            GL.Vertex(translateToGL(C));
            GL.Vertex(translateToGL(D));
            GL.End();
        }

        protected void drawOutput()
        {
            // draw output
            GL.Begin(GL.TRIANGLES);
            GL.Color(outputHovered ? connectorHoverColor : outputColor);
            GL.Vertex(translateToGL((D + (C - D) / 2) + new Vector2(0, -10) * parentGraph.getUIScale()));
            GL.Vertex(translateToGL((D + (C - D) / 2) + new Vector2(0, 10) * parentGraph.getUIScale()));
            GL.Vertex(translateToGL((D + (C - D) / 2) + new Vector2(15, 0) * parentGraph.getUIScale()));
            GL.End();
        }

        protected void drawInputs()
        {
            for (int i = 0; i < inputs.Length; i++)
            {
                // draw slot
                GL.Begin(GL.TRIANGLES);
                GL.Color(inputHovered.HasValue && inputHovered.Value == i && parentGraph.anyInputHovered ? connectorHoverColor : inputColor);
                GL.Vertex(translateToGL(getInputPoint(i) + new Vector2(-15, 0) * parentGraph.getUIScale()));
                GL.Vertex(translateToGL(getInputPoint(i) + new Vector2(0, 10) * parentGraph.getUIScale()));
                GL.Vertex(translateToGL(getInputPoint(i) + new Vector2(0, -10) * parentGraph.getUIScale()));
                GL.End();

                // draw line
                if (inputAt(i) != null)
                {
                    LogicFlowNode input = inputAt(i);
                    GL.Begin(GL.QUADS);
                    GL.Color(input.getValue() ? trueColor : falseColor);
                    GL_DrawLineWithWidth(input.outputScreenPos + parentGraph.A, getInputPoint(i) + new Vector2(parentGraph.getUIScale() * -10, 0) + parentGraph.A, 2);
                    GL.End();
                }
            }
        }

        private Vector2 getInputPoint(int index)
        {
            return A + ((B - A) / (inputs.Length * 2) * (index * 2 + 1));
        }


        protected abstract bool inputsValid();

        new protected Vector3 translateToGL(Vector3 s)
        {
            return LogicFlowBox.translateToGL(s + (Vector3)parentGraph.A);
        }

        internal void getInputTreeNodes(ref List<int> indexCollection)
        {
            foreach(int? index in inputs)
            {
                if (index.HasValue)
                {
                    indexCollection.Add(index.Value);
                    parentGraph.getNodeAt(index.Value).getInputTreeNodes(ref indexCollection);
                }
            }
        }

        /// <summary>
        /// Gets a "Tree" of all relevant downstream nodes. Does not include its called on.
        /// </summary>
        /// <returns></returns>
        public List<int> getInputTree()
        {
            List<int> l = new List<int>();
            getInputTreeNodes(ref l);
            return l;
        }

        public List<int> getNodeNetwork()
        {
            List<int> coreTree = this.getInputTree().Where(i => !(parentGraph.getNodeAt(i) is  LogicFlowInput)).ToList();
            coreTree.Add(index);
            List<int> otherNodes = parentGraph.getAllNodes().Where(n => !(n is LogicFlowInput)).Select(n => n.index).Where(i => !coreTree.Contains(i)).ToList();
            List<int> network = new List<int>(coreTree);
            foreach(int index in otherNodes)
            {
                if (!network.Contains(index))
                {
                    List<int> otherTree = parentGraph.getNodeAt(index).getInputTree();
                    if (network.Intersect(otherTree).ToList().Count > 0)
                    {
                        network.Add(index); // this node must be part of the network because its tree and the root tree intersect
                    }
                }
            }
            return network;
        }
    }

    public class NodeDeletedEventArgs : EventArgs
    {
        public int index { get; set; }
        public LogicFlowGraph parentGraph { get; set; }
    }
}
