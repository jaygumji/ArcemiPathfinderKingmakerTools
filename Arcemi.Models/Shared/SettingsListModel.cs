using Newtonsoft.Json.Linq;

namespace Arcemi.Models
{
    public class SettingsListModel : Model
    {
        public SettingsListModel(ModelDataAccessor accessor) : base(accessor)
        {
        }

        private T Value<T>(string name) => A.Value<T>("settings." + name);
        private void Value(JToken value, string name) => A.Value(value, "settings." + name);
        private T TutVal<T>(string name) => Value<T>("game.tutorial." + name);
        private void TutVal(JToken value, string name) => Value(value, "game.tutorial." + name);
        private T DiffVal<T>(string name) => Value<T>("difficulty." + name);
        private void DiffVal(JToken value, string name) => Value(value, "difficulty." + name);

        public bool Tutorial { get => TutVal<bool>("basis"); set => A.Value(value, "basis"); }
        public bool TutorialControls { get => TutVal<bool>("controls-basis"); set => TutVal(value, "controls-basis"); }
        public bool TutorialControlsAdvanced { get => TutVal<bool>("controls-advanced"); set => TutVal(value, "controls-advanced"); }
        public bool TutorialGameplay { get => TutVal<bool>("gameplay-basic"); set => TutVal(value, "gameplay-basic"); }
        public bool TutorialGameplayAdvanced { get => TutVal<bool>("gameplay-advanced"); set => TutVal(value, "gameplay-advanced"); }
        public bool TutorialPathfinderRules { get => TutVal<bool>("pathfinder-rules"); set => TutVal(value, "pathfinder-rules"); }
        public bool TutorialCrusade { get => TutVal<bool>("crusade"); set => TutVal(value, "crusade"); }
        public bool TutorialArmies { get => TutVal<bool>("armies"); set => TutVal(value, "armies"); }
        public string GameDifficulty { get => DiffVal<string>("game-difficulty"); set => DiffVal(value, "game-difficulty"); }
        public string StatsAdjustments { get => DiffVal<string>("stats-adjustments"); set => DiffVal(value, "stats-adjustments"); }
        public string EnemyDifficulty { get => DiffVal<string>("enemy-difficulty"); set => DiffVal(value, "enemy-difficulty"); }
        public string EnemyCriticalHits { get => DiffVal<string>("enemy-critical-hits"); set => DiffVal(value, "enemy-critical-hits"); }
        public double DamageToParty { get => DiffVal<double>("damage-to-party"); set => DiffVal(value, "damage-to-party"); }
        public bool DeathDoor { get => DiffVal<bool>("death-door"); set => DiffVal(value, "death-door"); }
        public bool DeadCompanionRiseAfterCombat { get => DiffVal<bool>("dead-companion-rise-after-combat"); set => DiffVal(value, "dead-companion-rise-after-combat"); }
        public bool RemoveNegativeLevelsOnRest { get => DiffVal<bool>("remove-negative-levels-on-rest"); set => DiffVal(value, "remove-negative-levels-on-rest"); }
        public bool RemoveAnnoyingBuffsAfterCombat { get => DiffVal<bool>("remove-annoying-buffs-after-combat"); set => DiffVal(value, "remove-annoying-buffs-after-combat"); }
        public string CombatEncountersCapacity { get => DiffVal<string>("combat-encounters-capacity"); set => DiffVal(value, "combat-encounters-capacity"); }
        public string AutoLevelUp { get => DiffVal<string>("auto-level-up"); set => DiffVal(value, "auto-level-up"); }
        public bool RespecAllowed { get => DiffVal<bool>("respec-allowed"); set => DiffVal(value, "respec-allowed"); }
        public bool AdditionalAIBehaviours { get => DiffVal<bool>("additional-ai-behaviours"); set => DiffVal(value, "additional-ai-behaviours"); }
        public bool EncumbranceSlowdown { get => DiffVal<bool>("encumbrance-slowdown"); set => DiffVal(value, "encumbrance-slowdown"); }
        public string WeatherEffects { get => DiffVal<string>("weather-effects"); set => DiffVal(value, "weather-effects"); }
        public bool OnlyInitiatorReceiveSkillCheckExperience { get => DiffVal<bool>("only-initiator-receive-skill-chick-experience"); set => DiffVal(value, "only-initiator-receive-skill-chick-experience"); }
        public bool AutoCrusade { get => DiffVal<bool>("auto-crusade"); set => DiffVal(value, "auto-crusade"); }
        public string KingdomDifficulty { get => DiffVal<string>("kingdom-difficulty"); set => DiffVal(value, "kingdom-difficulty"); }
        public bool OnlyOneSave { get => DiffVal<bool>("only-one-save"); set => DiffVal(value, "only-one-save"); }
        public bool ImmersionMode { get => DiffVal<bool>("immersion-mode"); set => DiffVal(value, "immersion-mode"); }
        public bool OnlyActiveCompanionsReceiveExperience { get => DiffVal<bool>("only-active-companions-receive-experience"); set => DiffVal(value, "only-active-companions-receive-experience"); }
        public bool LimitedAI { get => DiffVal<bool>("limited-ai"); set => DiffVal(value, "limited-ai"); }
        public bool OnlyInitiatorReceiveSkillCheckExperienceWasTouched { get => DiffVal<bool>("settings.difficulty.only-initiator-receive-skill-chick-experience-was-touched"); set => DiffVal(value, "settings.difficulty.only-initiator-receive-skill-chick-experience-was-touched"); }
        public bool AutoCrusadeWasTouched { get => DiffVal<bool>("settings.difficulty.auto-crusade-was-touched"); set => DiffVal(value, "settings.difficulty.auto-crusade-was-touched"); }

    }
}