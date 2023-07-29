namespace Coralite.Content.Bosses.VanillaReinforce.SlimeEmperor
{
    public partial class SlimeEmperor
    {
        public void HealGelBall()
        {
            switch ((int)SonState)
            {
                case 0:
                    Jump(2f, 6f, onLanding: () => SonState++);
                    break;
                case 1:
                    break;
                default:
                    break;
            }
        }
    }
}
