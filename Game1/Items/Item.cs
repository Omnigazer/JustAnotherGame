using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer.Items
{
    public abstract class Item : GameObject
    {
        public virtual bool CanEquip => false;
        public virtual void OnEquip(Character character) { }
        public virtual void OnUnequip() { }
    }
}
