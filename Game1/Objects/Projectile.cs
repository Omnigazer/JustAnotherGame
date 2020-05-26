namespace Omniplatformer.Objects
{
    public abstract class Projectile : GameObject
    {
        const int default_ttl = 500;
        public Projectile()
        {
            TTL = default_ttl;
        }

        public float TTL { get; set; }
        public override void Tick(float dt)
        {
            TTL -= dt;
            if (TTL <= 0)
            {
                onDestroy();
            }
            base.Tick(dt);
        }
    }
}
