using Omniplatformer.Services;
using Omniplatformer.Views.BasicControls;
using Omniplatformer.Views.Character;

namespace Omniplatformer.Views.HUD
{
    /// <summary>
    /// Containing class for all the default HUD elements, such as health/mana bars
    /// </summary>
    public class HUDContainer : ViewControl
    {
        public HUDContainer()
        {
            SetupGUI();
        }

        public void SetupGUI()
        {
            var col = new Column();
            Padding = 5;
            foreach (var bar in new ViewControl[] {
                new HealthBar(),
                new ExperienceBar(),
                new ChaosManaBar(),
                new NatureManaBar(),
                new LifeManaBar(),
                new DeathManaBar(),
                new SorceryManaBar()
            })
            {
                col.RegisterChild(bar);
                GameService.Instance.MainScene.UpdateSystem.RegisterObject(bar);
            }
            RegisterChild(col);

            var view = new QuickSlotViewCollection();
            col = new Column()
            {
                view
            };
            RegisterChild(col);
        }
    }
}
