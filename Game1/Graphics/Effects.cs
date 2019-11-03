using Microsoft.Xna.Framework;
using Omniplatformer.Content;

namespace Omniplatformer.Graphics
{
    static class Effects
    {
        public static void ApplyBlur(float lighten, Vector2 dir)
        {
            var blurEffect = GameContent.Instance.BlurEffect;
            blurEffect.Parameters["lighten"].SetValue(lighten);
            blurEffect.Parameters["dirx"].SetValue(dir.X);
            blurEffect.Parameters["diry"].SetValue(dir.Y);
            blurEffect.CurrentTechnique.Passes[0].Apply();
        }
    }
}
