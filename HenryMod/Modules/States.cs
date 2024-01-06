using HenryMod.SkillStates;
using HenryMod.SkillStates.BaseStates;
using System.Collections.Generic;
using System;
using CourierMod.Content;

namespace HenryMod.Modules
{
    public static class States
    {
        internal static void RegisterStates()
        {
            Modules.Content.AddEntityState(typeof(BaseMeleeAttack));
            Modules.Content.AddEntityState(typeof(SlashCombo));

            Modules.Content.AddEntityState(typeof(ShootOrb));
            Modules.Content.AddEntityState(typeof(TeleportTracker));
            Modules.Content.AddEntityState(typeof(TeleportSkill));
            Modules.Content.AddEntityState(typeof(Roll));

            Modules.Content.AddEntityState(typeof(ThrowBomb));

            Modules.Content.AddEntityState(typeof(FlutterOn));
        }
    }
}