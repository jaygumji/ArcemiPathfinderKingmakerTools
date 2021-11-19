using System;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class AlignmentMaskView
    {
        public AlignmentMaskView()
        {

        }

        public Alignment AlignmentMask { get; private set; }
        
        public void Set(string mask)
        {
            AlignmentMask = Enum.TryParse<Alignment>(mask ?? "Any", ignoreCase: true, out var enumMask) ? enumMask : Alignment.Any;
        }

        public void Set(Alignment mask)
        {
            AlignmentMask = mask;
        }

        public bool IsAlignmentAllowed(Alignment alignment)
        {
            return (AlignmentMask & alignment) == alignment;
        }

        private void SetAlignmentFlag(Alignment alignment, bool value)
        {
            if (value && (AlignmentMask & alignment) != alignment) {
                AlignmentMask |= alignment;
            }
            if (!value && (AlignmentMask & alignment) > 0) {
                AlignmentMask &= (Alignment)(Alignment.Any - alignment);
            }
        }

        public bool IsLawfulGoodAllowed
        {
            get => (AlignmentMask & Alignment.LawfulGood) == Alignment.LawfulGood;
            set => SetAlignmentFlag(Alignment.LawfulGood, value);
        }

        public bool IsNeutralGoodAllowed
        {
            get => (AlignmentMask & Alignment.NeutralGood) == Alignment.NeutralGood;
            set => SetAlignmentFlag(Alignment.NeutralGood, value);
        }

        public bool IsChaoticGoodAllowed
        {
            get => (AlignmentMask & Alignment.ChaoticGood) == Alignment.ChaoticGood;
            set => SetAlignmentFlag(Alignment.ChaoticGood, value);
        }

        public bool IsLawfulNeutralAllowed
        {
            get => (AlignmentMask & Alignment.LawfulNeutral) == Alignment.LawfulNeutral;
            set => SetAlignmentFlag(Alignment.LawfulNeutral, value);
        }

        public bool IsTrueNeutralAllowed
        {
            get => (AlignmentMask & Alignment.TrueNeutral) == Alignment.TrueNeutral;
            set => SetAlignmentFlag(Alignment.TrueNeutral, value);
        }

        public bool IsChaoticNeutralAllowed
        {
            get => (AlignmentMask & Alignment.ChaoticNeutral) == Alignment.ChaoticNeutral;
            set => SetAlignmentFlag(Alignment.ChaoticNeutral, value);
        }

        public bool IsLawfulEvilAllowed
        {
            get => (AlignmentMask & Alignment.LawfulEvil) == Alignment.LawfulEvil;
            set => SetAlignmentFlag(Alignment.LawfulEvil, value);
        }

        public bool IsNeutralEvilAllowed
        {
            get => (AlignmentMask & Alignment.NeutralEvil) == Alignment.NeutralEvil;
            set => SetAlignmentFlag(Alignment.NeutralEvil, value);
        }

        public bool IsChaoticEvilAllowed
        {
            get => (AlignmentMask & Alignment.ChaoticEvil) == Alignment.ChaoticEvil;
            set => SetAlignmentFlag(Alignment.ChaoticEvil, value);
        }

        public bool IsLawfulAllowed
        {
            get => (AlignmentMask & Alignment.Lawful) == Alignment.Lawful;
            set => SetAlignmentFlag(Alignment.Lawful, value);
        }

        public bool IsChaoticAllowed
        {
            get => (AlignmentMask & Alignment.Chaotic) == Alignment.Chaotic;
            set => SetAlignmentFlag(Alignment.Chaotic, value);
        }

        public bool IsGoodAllowed
        {
            get => (AlignmentMask & Alignment.Good) == Alignment.Good;
            set => SetAlignmentFlag(Alignment.Good, value);
        }

        public bool IsEvilAllowed
        {
            get => (AlignmentMask & Alignment.Evil) == Alignment.Evil;
            set => SetAlignmentFlag(Alignment.Evil, value);
        }

        public bool IsAnyAllowed
        {
            get => (AlignmentMask & Alignment.Any) == Alignment.Any;
            set => SetAlignmentFlag(Alignment.Any, value);
        }

    }
}
