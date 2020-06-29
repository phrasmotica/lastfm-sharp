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

using System;
using System.Collections.Generic;
using System.Xml;
using Lastfm.Services;

namespace Lastfm.Radio
{
    /// <summary>
    /// A Last.fm radio station.
    /// </summary>
    public class Station : Base
    {
        //// <summary>
        /// The unique station path.
        /// </summary>
        public StationURI URI { get; private set; }

        //// <summary>
        /// Station title. Like "Cher Similar Artists".
        /// </summary>
        public string Title { get; private set; }

        private bool tunedIn;

        public Station(StationURI uri, Session session) : base(session)
        {
            URI = uri;
        }

        public Station(string uri, Session session) : this(new StationURI(uri), session)
        {
        }

        private void TuneIn()
        {
            var parameters = new RequestParameters
            {
                ["station"] = URI.ToString()
            };

            var doc = (new Request("radio.tune", Session, parameters)).Execute();
            Title = Extract(doc, "name");

            tunedIn = true;
        }

        /// <summary>
        /// Fetches new radio content periodically.
        /// </summary>
        /// <param name="discoveryMode">
        /// A <see cref="bool"/> Whether to request last.fm content with discovery mode
        /// switched on.
        /// </param>
        /// <param name="isScrobbling">
        /// A <see cref="bool"/> Whether the user is scrobbling or not during this radio
        /// session (helps content generation).
        /// </param>
        /// <returns>A <see cref="Track"/></returns>
        public Track[] FetchTracks(bool discoveryMode, bool isScrobbling)
        {
            // tuneIn if necessary
            if (!tunedIn)
            {
                TuneIn();
            }

            // Fetch tracks
            var parameters = new RequestParameters();
            if (discoveryMode)
            {
                parameters["discovery"] = "true";
            }

            if (isScrobbling)
            {
                parameters["rtp"] = "true";
            }

            var doc = new Request("radio.getPlaylist", Session, parameters).Execute();

            var list = new List<Track>();
            foreach (XmlNode node in doc.GetElementsByTagName("track"))
            {
                var track = new Track(
                    Extract(node, "creator"),
                    Extract(node, "title"),
                    Extract(node, "album"),
                    Extract(node, "location"),
                    Extract(node, "identifier"),
                    Extract(node, "image"),
                    new TimeSpan(0, 0, 0, 0, int.Parse(Extract(node, "duration")))
                );

                list.Add(track);
            }

            return list.ToArray();
        }

        internal override RequestParameters GetParams()
        {
            throw new NotImplementedException();
        }
    }
}
