using System;

namespace Arcemi.Pathfinder.Kingmaker
{
    [Flags]
    public enum Alignment
    {
        LawfulGood = 0x1,
        NeutralGood = 0x2,
        ChaoticGood = 0x4,
        LawfulNeutral = 0x8,
        TrueNeutral = 0x10,
        ChaoticNeutral = 0x20,
        LawfulEvil = 0x40,
        NeutralEvil = 0x80,
        ChaoticEvil = 0x100,
        Good = LawfulGood | NeutralGood | ChaoticGood,
        Evil = LawfulEvil | NeutralEvil | ChaoticEvil,
        Lawful = LawfulGood | LawfulNeutral | LawfulEvil,
        Chaotic = ChaoticGood | ChaoticNeutral | ChaoticEvil,
        Any = 0x1FF,
        None = 0x0
    }
}
