using Server.ContextMenus;
using Server.Mobiles;

namespace Scripts.Mythik.Systems.Achievements
{
    /// <summary>
    /// If you want to add this context entry you will need to add a cliloc in the 300**** range, or
    /// use an existing one
    /// </summary>
    public class AchievementMenuEntry : ContextMenuEntry
    {
        private readonly PlayerMobile _from;
        private readonly PlayerMobile _target;

        public AchievementMenuEntry(PlayerMobile from, PlayerMobile target)
            : base(6257, -1) // View Achievements // 3006257
        {
            _from = from;
            _target = target;
        }

        public override void OnClick()
        {
            AchievementSystem.OpenGump(_from, _target);
        }
    }
}
