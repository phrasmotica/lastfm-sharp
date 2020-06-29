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

namespace Lastfm.Services
{
    /// <summary>
    /// An artist's bio on Last.fm.
    /// </summary>
    public class ArtistBio : Wiki
    {
        /// <summary>The concerned artist.</summary>
        public Artist Artist { get; private set; }

        public ArtistBio(Artist artist, Session session) : base("artist", session)
        {
            Artist = artist;
        }

        internal override RequestParameters GetParams()
        {
            return new RequestParameters
            {
                ["artist"] = Artist.Name
            };
        }

        /// <summary>
        /// The object's Last.fm page url.
        /// </summary>
        public string URL => GetURL(SiteLanguage.English);

        /// <summary>
        /// Returns the Last.fm page of this object.
        /// </summary>
        /// <param name="language">A <see cref="SiteLanguage"/></param>
        /// <returns>A <see cref="string"/></returns>
        private string GetURL(SiteLanguage language)
        {
            return $"{Artist.GetURL(language)}/+wiki";
        }
    }
}
