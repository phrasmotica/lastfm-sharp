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
    /// <summary>
    /// A Last.fm XSPF playlist.
    /// </summary>
    public class XSPF : Base
    {
        /// <summary>
        /// The Last.fm playlist url.
        /// </summary>
        public string PlaylistUrl { get; private set; }

        public XSPF(string playlistUrl, Session session) : base(session)
        {
            PlaylistUrl = playlistUrl;
        }

        internal override RequestParameters GetParams()
        {
            return new RequestParameters
            {
                ["playlistURL"] = PlaylistUrl
            };
        }

        /// <summary>
        /// Returns the tracks on this XSPF playlist.
        /// </summary>
        /// <returns>A <see cref="Track"/></returns>
        public Track[] GetTracks()
        {
            XmlDocument doc = Request("playlist.fetch");

            var list = new List<Track>();
            foreach (XmlNode n in doc.GetElementsByTagName("track"))
            {
                var track = new Track(Extract(n, "creator"), Extract(n, "title"), Session);
                list.Add(track);
            }

            return list.ToArray();
        }
    }
}
