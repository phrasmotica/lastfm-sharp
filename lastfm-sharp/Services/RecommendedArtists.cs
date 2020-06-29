// Copyright (C) 2009 Amr Hassan
//
// This program is free software: you can redistribute it and/or modify it under the terms of the
// GNU General Public License as published by the Free Software Foundation, either version 3 of the
// License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
// even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with this program. If
// not, see <http://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Xml;

namespace Lastfm.Services
{
    public class RecommendedArtists : Pages<Artist>
    {
        public AuthenticatedUser User { get; private set; }

        public RecommendedArtists(AuthenticatedUser user, Session session) : base("user.getRecommendedArtists", session)
        {
            User = user;
        }

        internal override RequestParameters GetParams()
        {
            return User.GetParams();
        }

        /// <summary>
        /// Returns a page of artists.
        /// </summary>
        /// <param name="page">A <see cref="System.Int32"/></param>
        /// <returns>A <see cref="Artist"/></returns>
        public override Artist[] GetPage(int page)
        {
            if (page < 1)
            {
                throw new InvalidPageException(page, 1);
            }

            var parameters = GetParams();
            parameters["page"] = page.ToString();

            XmlDocument doc = Request("user.getRecommendedArtists", parameters);

            var list = new List<Artist>();
            foreach (XmlNode node in doc.GetElementsByTagName("artist"))
            {
                var artist = new Artist(Extract(node, "name"), Session);
                list.Add(artist);
            }

            return list.ToArray();
        }
    }
}
