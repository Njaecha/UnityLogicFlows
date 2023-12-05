using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace LogicFlows
{
    public class LogicFlowGraph : LogicFlowBox
    {
        private Dictionary<int, LogicFlowNode> nodes = new Dictionary<int, LogicFlowNode>();

        public static Color backgroundColor = Color.white;
        public static Color headerColor = Color.gray;
        public static Color selectionColor = LogicFlowNode.selectedColor;

        private bool draggingWindow = false;
        private bool hoverExpand = false;
        private bool draggingExpand = false;
        internal bool anyInputHovered = false;
        internal bool anyNodeHovered = false;
        internal int? dragConnectionSourceIndex = null;

        // selection
        internal Vector2? selectionStart;
        internal List<int> selectedNodes = new List<int>();

        public bool eatingInput { get; internal set; } = false;
        public bool mouseOver { get; internal set; } = false;

        public bool isLoading = false;

        public List<LogicFlowNode> getAllNodes()
        {
            return nodes.Values.ToList();
        }

        public LogicFlowGraph(Rect rect)
        {
            this.rect = rect;
        }

        public LogicFlowNode getNodeAt(int key)
        {
            if (!nodes.ContainsKey(key)) return null;
            return nodes[key];
        }

        /// <summary>
        /// Should only be called from the constructor of LogicFlowNode
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal int AddNode(LogicFlowNode node)
        {
            int key;
            
            key = new System.Random().Next(int.MinValue, 0);
            while (nodes.ContainsKey(key))
            {
                key = new System.Random().Next(int.MinValue, 0);
            }

            nodes.Add(key, node);
            return key;
        }

        internal int AddNode(int key, LogicFlowNode node)
        {
            nodes.Add(key, node);
            return key;
        }

        public void RemoveNode(int index)
        {
            nodes.Remove(index);
        }

        public void ForceUpdate()
        {
            foreach (LogicFlowNode node in nodes.Values)
            {
                if (node != null && node is LogicFlowOutput n)
                {
                    n.forceUpdate();
                }
            }
        }

        /// <summary>
        /// should be called per frame when the flow is active but not displaying its UI
        /// </summary>
        public void backgroundUpdate()
        {
            foreach (LogicFlowNode node in nodes.Values)
            {
                if (node != null && node is LogicFlowOutput)
                {
                    node.getValue(); //evaluate flow
                }
            }
        }

        /// <summary>
        /// should be called from within OnGUI whenever the flow is displaying its UI
        /// </summary>
        public void ongui()
        {
            foreach (LogicFlowNode node in nodes.Values)
            {
                if (node != null) node.ongui();
            }

            if (eatingInput) Input.ResetInputAxes();
        }

        /// <summary>
        /// should be called from withing Update() whenever the flow is displaying its UI
        /// </summary>
        public void update()
        {
            Rect headerRect = new Rect(B.x, B.y + 10, rect.width, 25);
            Rect eatInputArea = new Rect(A.x - 10, A.y - 10, rect.width + 20, rect.height + 50);

            bool mouseOverHeader = headerRect.Contains(Input.mousePosition);
            mouseOver = rect.Contains(Input.mousePosition);

            if (eatInputArea.Contains(Input.mousePosition))
            {
                eatingInput = true;
            }
            else eatingInput = false;

            anyNodeHovered = false;

            Event current = Event.current;

            if (mouseOverHeader
                && (current.type == EventType.MouseDown || current.type == EventType.MouseDrag)
                && current.button == 0)
            {
                if (!delta.HasValue) delta = (Vector2)Input.mousePosition - rect.position;
                draggingWindow = true;
            }

            if (draggingWindow)
            {
                if (!delta.HasValue) return;
                Vector2 m = (Vector2)Input.mousePosition - delta.Value;
                rect = new Rect(m, rect.size);
            }


            bool hoveringAny = false;
            foreach (LogicFlowNode node in nodes.Values.Reverse())
            {
                if (node == null) continue;
                node.update();
                if (node.mouseOver) anyNodeHovered = true;
                if (node.outputHovered && (current.type == EventType.MouseDown || current.type == EventType.MouseDrag) && !LogicFlowNode.draggingAnyNode && !selectionStart.HasValue)
                {
                    dragConnectionSourceIndex = node.index;
                }
                if (dragConnectionSourceIndex.HasValue && node.inputHoverIndex != -1 && node.index != dragConnectionSourceIndex && !getNodeAt(dragConnectionSourceIndex.Value).getInputTree().Contains(node.index))
                {
                    if (current.type == EventType.MouseUp)
                    {
                        node.setInput(dragConnectionSourceIndex.Value);
                    }
                    hoveringAny = true;
                }
            }
            if (hoveringAny) anyInputHovered = true;
            else anyInputHovered = false;

            if (current.modifiers != EventModifiers.Shift
                && current.button == 0
                && current.type == EventType.MouseDown
                && mouseOver
                && !mouseOverHeader
                && !draggingWindow
                && !dragConnectionSourceIndex.HasValue
                && !anyNodeHovered
                ) selectedNodes.Clear();

            //selection
            if (current.type == EventType.MouseDown)
            {
                foreach(LogicFlowNode node in nodes.Values)
                {
                    if (node.mouseOver && !selectedNodes.Contains(node.index))
                    {
                        if (current.modifiers != EventModifiers.Shift)
                        {
                            selectedNodes.Clear();
                        }
                        selectedNodes.Add(node.index);
                    }
                }
            }
            if (!LogicFlowNode.draggingAnyNode && current.button == 0 && !mouseOverHeader && !draggingWindow && !dragConnectionSourceIndex.HasValue)
            {
                if (!selectionStart.HasValue 
                    && (current.type == EventType.MouseDown || current.type == EventType.MouseDrag) 
                    && mouseOver)
                {
                    selectionStart = Input.mousePosition;
                }
                if (current.type == EventType.MouseUp && selectionStart.HasValue)
                {
                    if (current.modifiers != EventModifiers.Shift) selectedNodes.Clear();
                    Rect selectionRect = RectFromVectors(selectionStart.Value - A, (Vector2)Input.mousePosition - A);
                    foreach(LogicFlowNode node in nodes.Values.Reverse())
                    {
                        if (node == null) continue;
                        if (selectionRect.Contains(node.A) || selectionRect.Contains(node.B) || selectionRect.Contains(node.C) || selectionRect.Contains(node.D))
                        {
                            if (!selectedNodes.Contains(node.index)) selectedNodes.Add(node.index);
                        }
                    }
                }
            }


            // reset stuff on mouseUP
            if (current.type == EventType.MouseUp)
            {
                draggingWindow = false;
                dragConnectionSourceIndex = null;
                delta = null;
                draggingExpand = false;
                selectionStart = null;
            }

            if ((new Rect(D.x - 10, D.y - 10, 20, 20).Contains(Input.mousePosition)))
            {
                hoverExpand = true;
                if (current.type == EventType.MouseDrag && current.button == 0) draggingExpand = true;
            }
            else hoverExpand = false;

            if (draggingExpand)
            {
                rect = new Rect(new Vector2(B.x, Input.mousePosition.y), new Vector2(Input.mousePosition.x - B.x, B.y - Input.mousePosition.y));
            }
        }

        /// <summary>
        /// should be called from within OnPostRender whenever the flow is displaying its UI
        /// has to be enclosed by GL.PushMatrix() and GL.EndMatrix()
        /// </summary>
        public void draw()
        {
            // draw header
            GL.Begin(GL.QUADS);
            GL.Color(headerColor);
            GL.Vertex(translate(B + new Vector2(0, 10)));
            GL.Vertex(translate(B + new Vector2(0, 35)));
            GL.Vertex(translate(C + new Vector2(0, 35)));
            GL.Vertex(translate(C + new Vector2(0, 10)));
            GL.End();

            // draw background
            GL.Begin(GL.QUADS);
            GL.Color(backgroundColor);
            GL.Vertex(translate(A));
            GL.Vertex(translate(B));
            GL.Vertex(translate(C));
            GL.Vertex(translate(D));
            GL.End();

            foreach (LogicFlowNode node in nodes.Values)
            {
                if (node != null) node.draw();
            }

            if (dragConnectionSourceIndex.HasValue)
            {
                GL.Begin(GL.QUADS);
                GL.Color(anyInputHovered ? LogicFlowNode.connectorHoverColor : LogicFlowNode.lineColor);
                drawLineWithWidth(getNodeAt(dragConnectionSourceIndex.Value).outputScreenPos + A, Input.mousePosition, 2);
                GL.End();
            }

            if (selectionStart.HasValue)
            {
                Vector2 v1 = selectionStart.Value;
                Vector2 v2 = new Vector2(selectionStart.Value.x, Input.mousePosition.y);
                Vector2 v3 = Input.mousePosition;
                Vector2 v4 = new Vector2(Input.mousePosition.x, selectionStart.Value.y);
                GL.Begin(GL.QUADS);
                GL.Color(selectionColor);
                drawLineWithWidth(v1, v2, 3);
                drawLineWithWidth(v2, v3, 3);
                drawLineWithWidth(v3, v4, 3);
                drawLineWithWidth(v4, v1, 3);
                GL.End();
            }

            if (hoverExpand || draggingExpand)
            {
                GL.Begin(GL.TRIANGLE_STRIP);
                GL.Color(headerColor);
                GL.Vertex(translate(D + new Vector2(-10, 0)));
                GL.Vertex(translate(D + new Vector2(10, -10)));
                GL.Vertex(translate(D));
                GL.Vertex(translate(D + new Vector2(0, 10)));
                GL.End();
            }

        }

        protected override Rect initRect()
        {
            return new Rect(200, 200, 600, 300);
        }
    }
}
