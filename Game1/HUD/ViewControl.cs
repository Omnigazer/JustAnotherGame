using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.HUD
{
    public abstract class ViewControl
    {
        public Point Position { get; set; }
        public Point GlobalPosition => Parent != null ? Parent.GlobalPosition + Position : Position;
        public int Width { get; set; }
        public int Height { get; set; }
        public Rectangle Rect => new Rectangle(Position, new Point(Width, Height));
        public ViewControl Parent { get; set; }
        public ViewControl Root => Parent?.Root ?? this;
        public List<ViewControl> Children { get; set; } = new List<ViewControl>();
        public IEnumerable<ViewControl> VisibleChildren => Children?.Where(x => x.Visible);
        public bool Hover { get; private set; }
        public bool Visible { get; set; } = true;
        public bool IsDragSource { get; set; } = false;
        protected virtual GameObject DragObject { get; set; }
        public bool IsDropTarget { get; set; } = false;

        public bool mouse_capture;

        // Events
        public event EventHandler MouseEnter = delegate { };
        public event EventHandler MouseLeave = delegate { };
        public event EventHandler MouseDown = delegate { };
        public event EventHandler MouseUp = delegate { };
        /*
        public event EventHandler Drag = delegate { };
        public class DropEventArgs : EventArgs
        {
            public GameObject DraggedItem { get; set; }
            public DropEventArgs(GameObject dragged_item)
            {
                DraggedItem = dragged_item;
            }
        }
        public event EventHandler<DropEventArgs> Drop = delegate { };
        */

        public void onMouseDown(Point pt)
        {
            foreach (var child in Children.Where(x => x.Visible))
            {
                if (child.Rect.Contains(pt))
                {
                    child.onMouseDown(convertToChildCoords(child, pt));
                }
            }
            MouseDown(this, new EventArgs());
        }

        public void onMouseUp(Point pt)
        {
            foreach (var child in Children.Where(x => x.Visible))
            {
                if (child.Rect.Contains(pt))
                {
                    child.onMouseUp(convertToChildCoords(child, pt));
                }
            }
            MouseUp(this, new EventArgs());
        }

        /*
        public virtual GameObject onDrag(Point pt)
        {
            foreach(var child in VisibleChildren)
            {
                if (child.Rect.Contains(pt))
                {
                    var res = child.onDrag(convertToChildCoords(child, pt));
                    if (res != null)
                        return res;
                }
            }
            if (IsDragSource)
            {
                var obj = DragObject;
                Drag(this, new EventArgs());
                DragObject = null;
                return obj;
            }
            return null;
        }

        public virtual bool onDrop(Point pt, GameObject item)
        {
            foreach (var child in VisibleChildren)
            {
                if (child.Rect.Contains(pt))
                {
                    if (child.onDrop(convertToChildCoords(child, pt), item))
                        return true;
                }
            }
            if (IsDropTarget)
            {
                // TODO: refactor this
                Drop(this, new DropEventArgs(item));
                DragObject = item;
                return true;
            }
            return false;
        }
        */

        public void MouseMove(Point pt)
        {
            if (!Hover)
            {
                Hover = true;
                MouseEnter(this, new EventArgs());
            }
            foreach (var child in VisibleChildren)
            {
                if (child.Rect.Contains(pt))
                {
                    child.MouseMove(convertToChildCoords(child, pt));
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

        public void RegisterChild(ViewControl control)
        {
            Children.Add(control);
            control.Parent = this;
        }

        Point convertToChildCoords(ViewControl child, Point pt)
        {
            return pt - child.Position;
        }

        public virtual void Draw()
        {
            foreach (var control in VisibleChildren)
            {
                control.Draw();
            }
        }
    }
}
