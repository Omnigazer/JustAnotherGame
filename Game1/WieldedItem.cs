﻿using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Omniplatformer
{
    public class WieldedItem : GameObject
    {
        // public int Damage { get; set; }
        // public Vector2 Knockback { get; set; }

        public WieldedItem(int damage)
        {
            Solid = false;
            Team = Team.Friend;
            // Damage = damage;
            var Knockback = new Vector2(3, 3);
            var halfsize = new Vector2(3, 25);
            Components.Add(new PositionComponent(this, Vector2.Zero, halfsize, 0, new Vector2(0, 1)));
            Components.Add(new RenderComponent(this, Color.White, GameContent.Instance.cursor));
            Components.Add(new DamageHitComponent(this, damage, Knockback));
        }
    }
}
