using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Omniplatformer
{
    public abstract class GameObject
    {
        public T GetComponent<T>() where T : Component
        {
            return Components.Find(x => x is T) as T;
        }

        protected List<Component> Components { get; set; }

        public float Friction {
            get
            {
                return 0.2f;
            }
        }

        // Candidates for component extraction
        public virtual bool Solid { get; set; }
        public virtual bool Liquid { get; set; }
        public virtual bool Climbable { get; set; }
        public virtual bool Pickupable { get; set; }
        public virtual bool Hittable { get; set; }
        public virtual bool Draggable { get; set; }
        public Team Team { get; set; }

        // Graphics
        // SpriteBatch spriteBatch;
        public event EventHandler _onDestroy = delegate { };
        public void onDestroy()
        {
            _onDestroy(this, new EventArgs());
        }

        public GameObject()
        {
            Components = new List<Component>();
            // TODO: move solid implementation to derived classes
            Solid = true;
            Team = Team.Neutral;
        }

        // Process a single game frame
        public virtual void Tick()
        {
            foreach (var c in Components)
            {
                c.Tick();
            }
        }

        // TODO: move this into a damageable component
        public virtual void ApplyDamage(float damage)
        {

        }

        // Typecasts
        public static explicit operator RenderComponent(GameObject obj)
        {
            return obj.GetComponent<RenderComponent>();
        }

        public static explicit operator PositionComponent(GameObject obj)
        {
            return obj.GetComponent<PositionComponent>();
        }

        public static explicit operator MoveComponent(GameObject obj)
        {
            return obj.GetComponent<MoveComponent>();
        }

        public static explicit operator HitComponent(GameObject obj)
        {
            return obj.GetComponent<HitComponent>();
        }
    }
}
