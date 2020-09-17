using Omniplatformer.Enums;
using System;

namespace Omniplatformer.Objects.Items
{
    public abstract class Item : GameObject
    {
        public Item()
        {
            Team = Team.Friend;
            Descriptors.Add(Descriptor.Item);
        }

        public int Count { get; set; } = 1;
        public int MaxCount { get; set; } = 1;

        public virtual bool CanEquip => false;
        public virtual void OnEquip(Character character) { }
        public virtual void OnUnequip(Character character) { }

        public string ItemId => GetType().ToString();
    }
}
