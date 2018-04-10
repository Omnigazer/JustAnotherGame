using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omniplatformer
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
