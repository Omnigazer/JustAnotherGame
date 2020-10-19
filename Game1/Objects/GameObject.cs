using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Omniplatformer.Components;
using Omniplatformer.Components.Character;
using Omniplatformer.Components.Physics;
using Omniplatformer.Components.Rendering;
using Omniplatformer.Enums;
using Omniplatformer.Scenes;
using Omniplatformer.Services;
using Omniplatformer.Utility;
using Omniplatformer.Utility.Extensions;

namespace Omniplatformer.Objects
{
    public abstract class GameObject
    {
        [JsonIgnore]
        public Scene CurrentScene { get; set; }

        [JsonProperty]
        public List<Component> Components { get; set; }

        protected Team _team;
        public Team Team { get => Source._team; set => _team = value; }
        private GameObject _source;

        public GameObject Source
        {
            get => _source?.Source ?? this;
            set => _source = value;
        }

        public HashSet<Descriptor> Descriptors { get; set; } = new HashSet<Descriptor>();

        [JsonIgnore]
        public Subject<GameObject> OnLeaveScene = new Subject<GameObject>();

        public GameObject()
        {
            Components = new List<Component>();
        }

        public virtual void InitializeComponents()
        {
            Components = new List<Component>();
            RegisterComponent(new CooldownComponent());
            InitializeCustomComponents();
            Compile();
        }

        public virtual void InitializeCustomComponents()
        {
        }

        public void RegisterComponent(Component c)
        {
            Components.Add(c);
            c.GameObject = this;
        }

        public T GetComponent<T>() where T : Component
        {
            return Components.Find(x => x is T) as T;
        }

        public bool HasDescriptor(Descriptor descriptor)
        {
            return Descriptors.Contains(descriptor);
        }

        public void LeaveScene()
        {
            OnLeaveScene.OnNext(this);
            OnLeaveScene.OnCompleted();
            CurrentScene.UnregisterObject(this);
        }

        // Process current frame
        public virtual void Tick(float dt)
        {
            for (int i = 0; i < Components.Count; i++)
            {
                Components[i].Tick(dt);
            }
        }

        public virtual void Compile()
        {
            foreach (var c in Components)
            {
                c.Compile();
            }
            OnCompile();
        }

        public virtual void OnCompile()
        {
        }

        public GameObject Clone()
        {
            using (MemoryStream ms = new MemoryStream(65536))
            using (StreamWriter sw = new StreamWriter(ms) { AutoFlush = true })
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                serializer.PreserveReferencesHandling = PreserveReferencesHandling.All;
                serializer.TypeNameHandling = TypeNameHandling.All;
                serializer.Serialize(writer, this);
                using (StreamReader sr = new StreamReader(ms))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    var obj = (GameObject)serializer.Deserialize(reader);
                    return obj;
                }
            }
        }

        //[OnDeserialized]
        //public void onDeserialized(StreamingContext _)
        //{
        //    Compile();
        //}

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

        public static explicit operator HitPointComponent(GameObject obj)
        {
            return obj?.GetComponent<HitPointComponent>();
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
