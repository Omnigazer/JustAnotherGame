using Microsoft.Xna.Framework;
using Omniplatformer.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Omniplatformer
{
    class DestructibleObject : GameObject
    {        
        public DestructibleObject(Vector2 center, Vector2 halfsize)
        {
            Hittable = true;
            // TODO: supply colors / textures to the component                    
            Components.Add(new PositionComponent(this, center, halfsize));
            Components.Add(new WallRenderComponent(this, Color.Yellow));
        }
        float hit_points = 10;             

        public override void ApplyDamage(float damage)
        {
            hit_points -= damage;
            if (hit_points <= 0)
            {
                var drawable = GetComponent<AnimatedRenderComponent>();
                drawable._onAnimationEnd += onDeathAnimationEnd;                
                drawable.StartAnimation(Animation.Death, 50);                                                         
            }            
        }

        private void onDeathAnimationEnd(object sender, AnimatedRenderComponent.AnimationEventArgs e)
        {
            if (e.animation == Animation.Death)
            {
                onDestroy();
            }
        }        
    }    
}
