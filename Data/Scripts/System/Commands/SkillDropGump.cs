using System;
using System.Collections;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Custom
{
    public class SkillDropGump : Gump
    {
        private PlayerMobile m_Player;

        // Button ID ranges
        // 1000 + skillIndex  = -1
        // 2000 + skillIndex  = -5
        // 3000 + skillIndex  = -10
        // 4000 + skillIndex  = set to zero (opens confirm)

        public static void Initialize()
        {
            CommandSystem.Register("SkillDrop", AccessLevel.Player, new CommandEventHandler(SkillDrop_OnCommand));
        }

        [Usage("SkillDrop")]
        [Description("Opens the Skill Drop gump, allowing you to reduce your skills.")]
        private static void SkillDrop_OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm == null)
                return;

            pm.CloseGump(typeof(SkillDropGump));
            pm.CloseGump(typeof(SkillDropConfirmGump));
            pm.SendGump(new SkillDropGump(pm));
        }

        public SkillDropGump(PlayerMobile pm) : base(50, 50)
        {
            m_Player = pm;

            Closable   = true;
            Dragable   = true;
            Disposable = true;

            ArrayList skills = new ArrayList();

            for (int i = 0; i < pm.Skills.Length; i++)
            {
                if (pm.Skills[i].Base > 0.1)
                    skills.Add(pm.Skills[i]);
            }

            int rowHeight = 30;
            int rows      = skills.Count;
            int height    = 80 + (rows * rowHeight);
            if (height < 150) height = 150;

            AddPage(0);
            AddBackground(0, 0, 560, height, 9270);
            AddAlphaRegion(10, 10, 540, height - 20);

            AddLabel(210, 15, 1152, "Skill Drop");

            // Column headers
            AddLabel(15,  45, 1152, "Skill");
            AddLabel(230, 45, 1152, "-1");
            AddLabel(290, 45, 1152, "-5");
            AddLabel(350, 45, 1152, "-10");
            AddLabel(420, 45, 1152, "Set to Zero");

            int y = 75;

            for (int i = 0; i < skills.Count; i++)
            {
                Skill skill     = (Skill)skills[i];
                int   skillIdx  = (int)skill.SkillName;

                AddLabel(15,  y, 1152, skill.Name);
                AddLabel(140, y, 68,   skill.Base.ToString("F1"));

                // -1 button
                AddButton(225, y, 4005, 4007, 1000 + skillIdx, GumpButtonType.Reply, 0);

                // -5 button
                AddButton(285, y, 4005, 4007, 2000 + skillIdx, GumpButtonType.Reply, 0);

                // -10 button
                AddButton(345, y, 4005, 4007, 3000 + skillIdx, GumpButtonType.Reply, 0);

                // Set to zero button
                AddButton(430, y, 4017, 4019, 4000 + skillIdx, GumpButtonType.Reply, 0);

                y += rowHeight;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 0)
                return;

            PlayerMobile pm = sender.Mobile as PlayerMobile;
            if (pm == null || pm != m_Player)
                return;

            int buttonID = info.ButtonID;

            if (buttonID >= 1000 && buttonID < 2000)
            {
                // -1
                int       skillIdx = buttonID - 1000;
                SkillName skillName = (SkillName)skillIdx;
                ReduceSkill(pm, skillName, 1.0);
                pm.SendGump(new SkillDropGump(pm));
            }
            else if (buttonID >= 2000 && buttonID < 3000)
            {
                // -5
                int       skillIdx  = buttonID - 2000;
                SkillName skillName = (SkillName)skillIdx;
                ReduceSkill(pm, skillName, 5.0);
                pm.SendGump(new SkillDropGump(pm));
            }
            else if (buttonID >= 3000 && buttonID < 4000)
            {
                // -10
                int       skillIdx  = buttonID - 3000;
                SkillName skillName = (SkillName)skillIdx;
                ReduceSkill(pm, skillName, 10.0);
                pm.SendGump(new SkillDropGump(pm));
            }
            else if (buttonID >= 4000)
            {
                // Set to zero — open confirmation gump
                int       skillIdx  = buttonID - 4000;
                SkillName skillName = (SkillName)skillIdx;
                pm.CloseGump(typeof(SkillDropConfirmGump));
                pm.SendGump(new SkillDropConfirmGump(pm, skillName));
            }
        }

        private static void ReduceSkill(PlayerMobile pm, SkillName skillName, double amount)
        {
            Skill skill = pm.Skills[skillName];

            if (skill == null)
                return;

            double newBase = skill.Base - amount;

            if (newBase < 0.0)
                newBase = 0.0;

            skill.Base = newBase;
        }
    }

    public class SkillDropConfirmGump : Gump
    {
        private PlayerMobile m_Player;
        private SkillName    m_Skill;

        private const int ButtonConfirm = 1;
        private const int ButtonCancel  = 2;

        public SkillDropConfirmGump(PlayerMobile pm, SkillName skill) : base(200, 200)
        {
            m_Player = pm;
            m_Skill  = skill;

            Closable   = true;
            Dragable   = true;
            Disposable = true;

            string skillName = pm.Skills[skill].Name;

            AddPage(0);
            AddBackground(0, 0, 360, 160, 9270);
            AddAlphaRegion(10, 10, 340, 140);

            AddLabel(85, 20, 1152, "Confirm Skill Reset");

            AddLabel(20,  55, 33, "Are you sure you want to set");
            AddLabel(20,  75, 33, skillName + " to zero?");
            AddLabel(20,  95, 33, "This cannot be undone.");

            // Confirm
            AddButton(60,  125, 4005, 4007, ButtonConfirm, GumpButtonType.Reply, 0);
            AddLabel(95,  125, 68,  "Yes, set to zero");

            // Cancel
            AddButton(230, 125, 4017, 4019, ButtonCancel, GumpButtonType.Reply, 0);
            AddLabel(265, 125, 33,  "Cancel");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            PlayerMobile pm = sender.Mobile as PlayerMobile;
            if (pm == null || pm != m_Player)
                return;

            if (info.ButtonID == ButtonConfirm)
            {
                Skill skill = pm.Skills[m_Skill];

                if (skill != null)
                {
                    skill.Base = 0.0;
                    pm.SendMessage(68, skill.Name + " has been set to zero.");
                }

                pm.CloseGump(typeof(SkillDropGump));
                pm.SendGump(new SkillDropGump(pm));
            }
            else
            {
                // Cancel — reopen the main gump
                pm.CloseGump(typeof(SkillDropGump));
                pm.SendGump(new SkillDropGump(pm));
            }
        }
    }
}