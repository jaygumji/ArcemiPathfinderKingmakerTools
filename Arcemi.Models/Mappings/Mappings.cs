#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
#endregion
using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcemi.Models
{

    public class Mappings
    {
        public readonly Dictionary<string, ClassDataMapping> Classes;
        public readonly Dictionary<string, RaceDataMapping> Races;
        public readonly Dictionary<string, CharacterDataMapping> Characters;
        public readonly Dictionary<string, LeaderDataMapping> Leaders;
        public readonly Dictionary<string, ArmyUnitDataMapping> ArmyUnits;

        public Mappings(GameDefinition game)
        {
            var dataMappings = DataMappings.LoadFromDefault();
            Classes = dataMappings.Classes
                .Concat(dataMappings.Classes.Where(c => c.Archetypes != null).SelectMany(c => c.Archetypes))
                .Where(a => !string.IsNullOrEmpty(a.Id))
                .ToDictionary(x => x.Id, StringComparer.Ordinal);

            Races = dataMappings.Races
                .ToDictionary(x => x.Id, StringComparer.Ordinal);

            Characters = dataMappings.Characters
                .Where(c => c.GameId is null || c.GameId.Eq(game.Id))
                .ToDictionary(x => x.Id, StringComparer.Ordinal);

            Leaders = dataMappings.Leaders
                .ToDictionary(x => x.Id, StringComparer.Ordinal);

            ArmyUnits = dataMappings.ArmyUnits
                .ToDictionary(x => x.Id, StringComparer.Ordinal);
        }
    }
}