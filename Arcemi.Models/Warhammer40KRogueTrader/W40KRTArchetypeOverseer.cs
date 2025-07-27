using System.Collections.Generic;
using System;
using System.Linq;

namespace Arcemi.Models.Warhammer40KRogueTrader
{
    public static class W40KRTArchetypeOverseer
    {
        public static void Downgrade(W40KRTArchetypeDowngradeArguments args)
        {
            if (!args.IsLastLevel) return;

            args.Mediator.RemoveChildOf(args.Owner);
            var autoFeats = new HashSet<string>(args.Archetype.AutomaticFeats.Select(af => af.Id), StringComparer.Ordinal);
            var feats = args.Owner.Feats.Where(f => autoFeats.Contains(f.Blueprint)).ToArray();
            foreach (var feat in feats) {
                args.Owner.Feats.Remove(feat);
            }
        }
    }
}
