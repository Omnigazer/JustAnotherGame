using Omniplatformer.Services;

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
                RegisterChild(bar);
                GameService.Instance.MainScene.UpdateSystem.RegisterObject(bar);
            }
        }
    }
}
