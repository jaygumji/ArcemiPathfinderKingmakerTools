using System;

namespace Arcemi.Models
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

    public static class AlignmentExtensions
    {
        public static Alignment Detect(double X, double Y)
        {
            if (Math.Pow(X, 2) + Math.Pow(Y, 2) < Math.Pow(33.3, 2)) {
                return Alignment.TrueNeutral;
            }

            var angle = Math.Atan2(Y, X) * 180 / Math.PI;
            if (angle < 157.5 && angle >= 112.5) return Alignment.LawfulGood;
            if (angle < 112.5 && angle >= 67.5) return Alignment.NeutralGood;
            if (angle < 67.5 && angle >= 22.5) return Alignment.ChaoticGood;
            if (angle < 22.5 && angle >= -22.5) return Alignment.ChaoticNeutral;
            if (angle < -22.5 && angle >= -67.5) return Alignment.ChaoticEvil;
            if (angle < -67.5 && angle >= -112.5) return Alignment.NeutralEvil;
            if (angle < -112.5 && angle >= -157.5) return Alignment.LawfulEvil;
            return Alignment.LawfulNeutral;

        }
    }
}
