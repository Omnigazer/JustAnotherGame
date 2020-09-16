﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Facebook.Yoga;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.HUDStates;
using Omniplatformer.Objects;
using Omniplatformer.Scenes.Subsystems;
using Omniplatformer.Services;
using Omniplatformer.Utility.Extensions;

namespace Omniplatformer.Views
{
    public abstract class ViewControl : IUpdatable, IEnumerable<ViewControl>
    {
        public YogaNode Node;

        public Point Position => new Point((int)Node.LayoutX + Parent?.OffsetH ?? 0, (int)Node.LayoutY + Parent?.OffsetV ?? 0);
        public Point GlobalPosition => Parent != null ? Parent.GlobalPosition + Position : Position;

        // public int Width { get => (int)Node.Width.Value; set => Node.Width = value; }
        // public int Height { get => (int)Node.Height.Value; set => Node.Height = value; }
        public int Width { get => (int)Node.LayoutWidth - 2 * BorderThickness; set => Node.Width = value; }

        public int Height { get => (int)Node.LayoutHeight - 2 * BorderThickness; set => Node.Height = value; }
        public int RelativeWidth { set => Node.Width = YogaValue.Percent(value); }
        public int RelativeHeight { set => Node.Height = YogaValue.Percent(value); }
        public int Margin { get => (int)Node.Margin.Value; set => Node.Margin = value; }
        public int Padding { get => (int)Node.Padding.Value; set => Node.Padding = value; }

        public int OffsetV { get; set; }
        public int OffsetH { get; set; }

        public Rectangle LocalRect => new Rectangle(Position, new Point((int)Node.LayoutWidth, (int)Node.LayoutHeight));

        // public Rectangle GlobalRect => new Rectangle(GlobalPosition, new Point((int)Node.LayoutWidth, (int)Node.LayoutHeight));
        public Rectangle GlobalRect => new Rectangle(GlobalPosition + new Point(BorderThickness), new Point((int)Node.LayoutWidth - 2 * BorderThickness, (int)Node.LayoutHeight - 2 * BorderThickness));

        // public int BorderThickness { get => (int)Node.BorderWidth; set => Node.BorderWidth = value; }
        public int BorderThickness { get; set; }

        //public int BorderThickness { get => (int)Node.Padding.Value; set => Node.Padding = value; }
        public ViewControl Parent { get; set; }

        public ViewControl Root => Parent?.Root ?? this;
        public List<ViewControl> Children { get; set; } = new List<ViewControl>();
        public IEnumerable<ViewControl> VisibleChildren => Children?.Where(x => x.Visible);
        public bool Hover { get; private set; }
        public bool Visible { get => Node.Display == YogaDisplay.Flex; set => Node.Display = value ? YogaDisplay.Flex : YogaDisplay.None; }
        public bool IsDragSource { get; set; } = false;
        protected virtual GameObject DragObject { get; set; }
        public bool IsDropTarget { get; set; } = false;

        public virtual bool ConsumesEvents => true;

        /// <summary>
        /// Rasterizer state for scissor rect testing
        /// </summary>
        static RasterizerState raster = new RasterizerState() { ScissorTestEnable = true };

        /// <summary>
        /// Cached scissor rectangle of the control's parent
        /// </summary>
        Rectangle CurrentScissors { get; set; }

        // Events
        public event EventHandler<MouseEventArgs> MouseMove = delegate { };

        public event EventHandler MouseEnter = delegate { };

        public event EventHandler MouseLeave = delegate { };

        public event EventHandler<MouseEventArgs> MouseDown = delegate { };

        public event EventHandler<MouseEventArgs> MouseUp = delegate { };

        public event EventHandler<MouseEventArgs> MouseClick = delegate { };

        public event EventHandler<MouseEventArgs> MouseWheelUp = delegate { };

        public event EventHandler<MouseEventArgs> MouseWheelDown = delegate { };

        public ViewControl()
        {
            Node = new YogaNode();
            SetupNode();
        }

        public virtual void SetupNode()
        {
        }

        public ViewControl onMouseDown(MouseButton button, Point pt)
        {
            foreach (var child in Children.Where(x => x.Visible))
            {
                if (child.LocalRect.Contains(pt))
                {
                    var result = child.onMouseDown(button, convertToChildCoords(child, pt));
                    if (result != null)
                    {
                        return result;
                    }
                }
            }
            if (ConsumesEvents)
            {
                MouseDown(this, new MouseEventArgs(button, pt));
                return this;
            }
            return null;
        }

        public void onMouseUp(MouseButton button, Point pt)
        {
            if (GlobalRect.Contains(pt))
                MouseClick(this, new MouseEventArgs(button, pt));
            MouseUp(this, new MouseEventArgs(button, pt));
            return;
        }

        public void onMouseWheelUp(Point pt)
        {
            foreach (var child in Children.Where(x => x.Visible))
            {
                if (child.LocalRect.Contains(pt))
                {
                    if (child.Node.Overflow == YogaOverflow.Scroll)
                    {
                        child.ScrollUp();
                        return;
                    }
                    else
                        child.onMouseWheelUp(convertToChildCoords(child, pt));
                }
            }
            MouseWheelUp(this, new MouseEventArgs());
        }

        public void onMouseWheelDown(Point pt)
        {
            foreach (var child in Children.Where(x => x.Visible))
            {
                if (child.LocalRect.Contains(pt))
                {
                    if (child.Node.Overflow == YogaOverflow.Scroll)
                    {
                        child.ScrollDown();
                        return;
                    }
                    else
                        child.onMouseWheelDown(convertToChildCoords(child, pt));
                }
            }
            MouseWheelDown(this, new MouseEventArgs());
        }

        public void ScrollUp()
        {
            OffsetV += 25;
            OffsetV = Math.Min(OffsetV, 0);
        }

        public void ScrollDown()
        {
            OffsetV -= 25;
            int max_offset = (int)Children[Children.Count - 1].Node.LayoutY - Height;
            OffsetV = Math.Min(0, Math.Max(OffsetV, -max_offset));
        }

        public void onMouseMove(Point pt)
        {
            if (!Hover)
            {
                Hover = true;
                MouseEnter(this, new EventArgs());
            }
            foreach (var child in VisibleChildren)
            {
                if (child.LocalRect.Contains(pt))
                {
                    child.onMouseMove(convertToChildCoords(child, pt));
                }
                else if (child.Hover)
                {
                    child.onMouseLeave();
                }
            }
        }

        public void onMouseLeave()
        {
            if (Hover)
            {
                Hover = false;
                MouseLeave(this, new EventArgs());
                foreach (var child in VisibleChildren)
                {
                    child.onMouseLeave();
                }
            }
        }

        IEnumerator<ViewControl> IEnumerable<ViewControl>.GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        public void Add(ViewControl control)
        {
            RegisterChild(control);
        }

        public void RegisterChild(ViewControl control)
        {
            Children.Add(control);
            control.Parent = this;
            Node.AddChild(control.Node);
        }

        public void UnregisterChild(ViewControl control)
        {
            Node.RemoveChild(control.Node);
        }

        public void Clear()
        {
            Children.Clear();
            Node.Clear();
        }

        Point convertToChildCoords(ViewControl child, Point pt)
        {
            return pt - child.Position;
        }

        public virtual void DrawSelf()
        {
        }

        public virtual void Draw()
        {
            CurrentScissors = GraphicsService.GraphicsDevice.ScissorRectangle;
            var rect = GlobalRect;
            rect.Inflate(BorderThickness, BorderThickness);
            var scissor = Rectangle.Intersect(CurrentScissors, rect);
            GraphicsService.GraphicsDevice.ScissorRectangle = scissor;
            if (scissor.Size != new Point() || Parent.Node.Overflow == YogaOverflow.Visible)
            {
                var spriteBatch = GraphicsService.Instance;
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, raster);
                DrawSelf();
                spriteBatch.End();
                GraphicsService.GraphicsDevice.ScissorRectangle = Rectangle.Intersect(CurrentScissors, GlobalRect);

                foreach (var control in VisibleChildren)
                {
                    control.Draw();
                }
            }
            GraphicsService.GraphicsDevice.ScissorRectangle = CurrentScissors;
        }

        public void DrawBorder(Color? color = null)
        {
            float thickness = (float)BorderThickness;
            var rect = GlobalRect;
            Color _color = color ?? Color.White;
            var spriteBatch = GraphicsService.Instance;

            spriteBatch.DrawLine(new Vector2(rect.Left - thickness / 2, rect.Top), new Vector2(rect.Left - thickness / 2, rect.Bottom), _color, thickness);
            spriteBatch.DrawLine(new Vector2(rect.Left - thickness, rect.Bottom + thickness / 2), new Vector2(rect.Right + thickness, rect.Bottom + thickness / 2), _color, thickness);
            spriteBatch.DrawLine(new Vector2(rect.Right + thickness / 2, rect.Top - thickness / 2), new Vector2(rect.Right + thickness / 2, rect.Bottom), _color, thickness);
            spriteBatch.DrawLine(new Vector2(rect.Right + thickness, rect.Top - thickness / 2), new Vector2(rect.Left - thickness, rect.Top - thickness / 2), _color, thickness);
        }

        void IUpdatable.Tick(float dt)
        {
        }
    }
}
