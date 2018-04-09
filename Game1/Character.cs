﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Omniplatformer.Components;

namespace Omniplatformer
{
    public abstract class Character : GameObject
    {
        public float CurrentHitPoints { get; set; }
        public float MaxHitPoints { get; set; }
        public bool Vulnerable { get; set; }

        public Character()
        {
            CurrentHitPoints = MaxHitPoints = 50;
            Hittable = true;
            Solid = false;
            Vulnerable = true;
        }


        public override void onDestroy()
        {
            // TODO: find another approach for earning exp
            GameService.Player.EarnExperience(300);
            base.onDestroy();
        }

        public override void ApplyDamage(float damage)
        {
            CurrentHitPoints -= damage;
            var drawable = GetComponent<CharacterRenderComponent>();
            drawable.StartAnimation(Animation.Hit, 15);
            if (CurrentHitPoints <= 0)
            {
                onDestroy();
            }
        }
    }
}
