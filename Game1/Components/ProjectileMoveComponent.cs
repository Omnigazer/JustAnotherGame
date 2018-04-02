using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Components
{

    public class ProjectileMoveComponent : MoveComponent
    {
        public Vector2 direction;

        public ProjectileMoveComponent(GameObject obj) : base(obj)
        {
        }

        // Check what kinds of objects are we colliding here
        // TODO: problematic method & overrides, refactor
        public override void ProcessCollisionInteractions(List<(Direction, GameObject)> collisions)
        {            
            base.ProcessCollisionInteractions(collisions);            
        }

        protected override void ProcessCollision(Direction direction, GameObject obj)
        {
            base.ProcessCollision(direction, obj);
            if (direction != Direction.None && (obj.Solid || obj.Hittable))
            {
                var hittable = GetComponent<HitComponent>();                
                hittable?.Hit(obj);                
                // TODO: might have to extract this
                GameObject.onDestroy();
                // Hit(obj);
            }            
        }

        public override Vector2 GetMoveVector()
        {
            CurrentMovement = direction;         
            return CurrentMovement;            
        }        
    }
}
