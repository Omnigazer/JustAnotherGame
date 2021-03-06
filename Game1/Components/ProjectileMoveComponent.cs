﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{
    public class ProjectileMoveComponent : DynamicPhysicsComponent
    {
        public Vector2 Direction { get; set; }

        public ProjectileMoveComponent(GameObject obj, Vector2 coords, Vector2 halfsize) : base(obj, coords, halfsize)
        {
        }

        bool done = false;

        public override bool ProcessCollision(Direction direction, PhysicsComponent obj)
        {
            if (done)
            {
                return false;
            }
            base.ProcessCollision(direction, obj);

            if (direction != Omniplatformer.Direction.None && obj.GameObject != GameObject.Source && (obj.Solid || obj.Hittable) && obj.GameObject.Team != GameObject.Team)
            {
                var hittable = GetComponent<HitComponent>();
                hittable?.Hit(obj.GameObject);
                // TODO: might have to extract this
                // GameObject.onDestroy();
                CurrentMovement = Vector2.Zero;
                GameObject.onDestroy();
                done = true;
                return true;
                // Hit(obj);
            }
            return false;
        }

        public override void ProcessMovement(float dt)
        {
            // CurrentMovement = Direction;
        }
    }
}
