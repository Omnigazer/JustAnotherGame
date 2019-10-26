using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components;
using Omniplatformer.Scenes;
using Omniplatformer.Utility;
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
        public Scene CurrentScene { get; set; }
        public bool Tickable { get; set; }
        public Guid Id { get; set; }

        public T GetComponent<T>() where T : Component
        {
            return Components.Find(x => x is T) as T;
        }

        public bool HasDescriptor(Descriptor descriptor)
        {
            return Descriptors.Exists(x => x == descriptor);
        }

        public virtual object AsJson()
        {
            return new { };
        }

        protected List<Component> Components { get; set; }
        protected Dictionary<string, float> Cooldowns { get; set; }
        public Game1 Game => GameService.Instance;

        // Candidates for component extraction
        // public virtual bool Draggable { get; set; }
        // public virtual bool Hidden { get; set; }
        public Team Team { get; set; }
        public virtual GameObject Source => this;
        public List<Descriptor> Descriptors { get; set; } = new List<Descriptor>();

        // Graphics
        // SpriteBatch spriteBatch;
        public event EventHandler _onDestroy = delegate { };
        public virtual void onDestroy()
        {
            _onDestroy(this, new EventArgs());
        }

        public GameObject()
        {
            Id = Id == Guid.Empty ? Guid.NewGuid() : Id;
            Components = new List<Component>();
            Cooldowns = new Dictionary<string, float>();
            Tickable = true;
            Team = Team.Neutral;
        }

        // Process a single game frame
        public virtual void Tick(float dt)
        {
            // foreach (var c in Components)
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].Tick(dt);
            }
            foreach (var (key, ticks) in Cooldowns.ToList())
            {
                Cooldowns[key] = Math.Max(ticks - dt, 0);
            }
        }

        // TODO: move this into a damageable component
        public virtual void ApplyDamage(float damage)
        {

        }

        public bool TryCooldown(string key, int value)
        {
            if (!Cooldowns.ContainsKey(key) || Cooldowns[key] <= 0)
            {
                Cooldowns.SetOrAdd(key, value);
                return true;
            }
            return false;
        }

        // Typecasts
        public static explicit operator RenderComponent(GameObject obj)
        {
            return obj?.GetComponent<RenderComponent>();
        }

        public static explicit operator PositionComponent(GameObject obj)
        {
            return obj?.GetComponent<PositionComponent>();
        }

        public static explicit operator MoveComponent(GameObject obj)
        {
            return obj?.GetComponent<MoveComponent>();
        }

        public static explicit operator HitComponent(GameObject obj)
        {
            return obj?.GetComponent<HitComponent>();
        }

        public static explicit operator BonusComponent(GameObject obj)
        {
            return obj?.GetComponent<BonusComponent>();
        }

        public override string ToString()
        {
            var pos = GetComponent<PositionComponent>();
            if (pos != null)
            {
                return $"{GetType().Name} {pos.ToString()}";
            }
            else return base.ToString();
        }
    }
}
