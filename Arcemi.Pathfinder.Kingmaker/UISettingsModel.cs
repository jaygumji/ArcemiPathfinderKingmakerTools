#region License
/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */
 #endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Arcemi.Pathfinder.Kingmaker
{
    public class UISettingsModel : RefModel
    {

        private readonly string _characterBlueprint;
        private Portraits _portraits;

        public UISettingsModel(ModelDataAccessor accessor, string characterBlueprint) : base(accessor)
        {
            _characterBlueprint = characterBlueprint;
        }

        public string PortraitPath
        {
            get {
                if (_portraits == null) return null;

                if (!string.IsNullOrEmpty(CustomPortrait?.CustomPortraitId)) {
                    return _portraits.GetPortraitsUri(CustomPortrait.CustomPortraitId);
                }
                if (!string.IsNullOrEmpty(Portrait)) {
                    return _portraits.GetPortraitsUri(Portrait);
                }
                var portraitId = Mappings.GetCharacterPotraitIdentifier(_characterBlueprint);
                return _portraits.GetPortraitsUri(portraitId);
            }
        }

        public string Portrait { get => A.Value<string>("m_Portrait"); set => A.Value(value, "m_Portrait"); }

        private CustomPortraitModel CustomPortrait => A.Object("m_CustomPortrait", a => new CustomPortraitModel(a));

        public IReadOnlyList<Portrait> AvailablePortraits => _portraits.GetAvailableFor(_characterBlueprint);

        public void SetPortrait(Portrait portrait)
        {
            N.Supress();
            if (portrait.IsCompanion) {
                A.Value(null, "m_Portrait");
                A.SetObjectToNull("m_CustomPortrait");
            }
            else if (portrait.IsCustom) {
                A.Value(null, "m_Portrait");
                var c = A.Object("m_CustomPortrait", a => new CustomPortraitModel(a), createIfNull: true);
                c.CustomPortraitId = portrait.Key;
            }
            else {
                A.Value(portrait.Key, "m_Portrait");
                A.SetObjectToNull("m_CustomPortrait");
            }
            N.Resume();
            N.Notify(nameof(PortraitPath));
        }

        public void Init(Portraits portraits)
        {
            _portraits = portraits;
        }
    }
}