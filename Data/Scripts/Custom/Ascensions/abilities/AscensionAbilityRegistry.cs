using System.Collections.Generic;

namespace Server.Custom.Ascensions
{
    public static class AscensionAbilityRegistry
    {
        private static List<AscensionAbility> _abilities;

        static AscensionAbilityRegistry()
        {
            _abilities = new List<AscensionAbility>();

            // ----- Berserker
            Register(new BerserkerRageAbility());
            Register(new BerserkerLeapSlamAbility());
            Register(new BerserkerWarCryAbility());
            Register(new BerserkerTenacityAbility());
            // ---- Archmage
            Register(new ArchmageArcaneStormAbility());
            Register(new ArchmageConfluxAbility());
            Register(new ArchmageManaSingularityAbility());
            Register(new ArchmageTimestopAbility());
            // ---- Pale master
            Register(new PalemasterUndyingHordesAbility());
            Register(new PaleMasterEnervateAbility());
            Register(new PalemasterCircleOfDeathAbility());
            Register(new PalemasterDanseMacabreAbility());
            // ---- Crusader
            Register(new CrusaderSmiteAbility());
            Register(new CrusaderChargeAbility());
            Register(new CrusaderAuraOfHopeAbility());
            Register(new CrusaderHeavenlyGateAbility());
            // ---- Assassin
            Register(new AssassinNoxiousCloudAbility());
            Register(new AssassinCripplingPoisonAbility());
            Register(new AssassinToxicSurgeAbility());
            Register(new AssassinCleansingAnnihilationAbility());
            // ---- Blackguard
            Register(new BlackguardDarkSuccorAbility());
            Register(new BlackguardDeathsAdvanceAbility());
            Register(new BlackguardChainsOfIceAbility());
            Register(new BlackguardsFrostwyrmsFuryAbility());
            // ---- Skald
            Register(new SkaldWarChantAbility());
            Register(new SkaldSagaOfValorAbility());
            Register(new SkaldSongOfThunderAbility());
            Register(new SkaldDirgeOfTheFallenAbility());
            // ---- Reaver
            Register(new ReaverGorgeAbility());
            Register(new ReaverExsanguinateAbility());
            Register(new ReaverBloodstormAbility());
            Register(new ReaverAbsoluteTyrannyAbility());
            // ---- Kensai
            Register(new KensaiBattleMeditationAbility());
            Register(new KensaiKaiAbility());
            Register(new KensaiCullingStrikeAbility());
            Register(new KensaiTempestAbility());
            // ---- Hierophant
            Register(new HierophantDivineWrathAbility());
            Register(new HierophantExaltedPresenceAbility());
            Register(new HierophantConsecratedGroundAbility());
            Register(new HierophantDivinePowerAbility());
            // ---- Arcane Archer
            Register(new ArcaneArcherImbueArrowsAbility());
            Register(new ArcaneArcherChargedArrowsAbility());
            Register(new ArcaneArcherArcaneVolleyAbility());
            Register(new ArcaneArcherBarrageAbility());
        }

        private static void Register(AscensionAbility ability)
        {
            if (ability == null)
                return;

            _abilities.Add(ability);
        }

        public static List<AscensionAbility> GetAbilities(AscensionType type)
        {
            List<AscensionAbility> list = new List<AscensionAbility>();

            for (int i = 0; i < _abilities.Count; i++)
            {
                if (_abilities[i].Ascension == type)
                    list.Add(_abilities[i]);
            }

            return list;
        }

        public static AscensionAbility GetAbilityByName(string name)
        {
            if (name == null)
                return null;

            for (int i = 0; i < _abilities.Count; i++)
            {
                if (_abilities[i].Name == name)
                    return _abilities[i];
            }

            return null;
        }
    }
}
