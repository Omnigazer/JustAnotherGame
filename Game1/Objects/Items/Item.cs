using Omniplatformer.Enums;

namespace Omniplatformer.Objects.Items
{
    public abstract class Item : GameObject
    {
        public Item()
        {
            Team = Team.Friend;
            Descriptors.Add(Descriptor.Item);
        }
        public virtual bool CanEquip => false;
        public virtual void OnEquip(Character character) { }
        public virtual void OnUnequip(Character character) { }
    }
}
