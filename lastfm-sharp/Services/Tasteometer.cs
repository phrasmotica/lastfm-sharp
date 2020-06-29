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
    /// Compare Last.fm users and others.
    /// </summary>
    public class Tasteometer : Base
    {
        private string FirstType { get; set; }
        private string FirstValue { get; set; }
        private string SecondType { get; set; }
        private string SecondValue { get; set; }

        internal override RequestParameters GetParams()
        {
            return new RequestParameters
            {
                ["type1"] = FirstType,
                ["type2"] = SecondType,
                ["value1"] = FirstValue,
                ["value2"] = SecondValue
            };
        }

        /// <summary>
        /// Compare a user with another user.
        /// </summary>
        /// <param name="firstUser">A <see cref="User"/></param>
        /// <param name="secondUser">A <see cref="User"/></param>
        /// <param name="session">A <see cref="Session"/></param>
        public Tasteometer(User firstUser, User secondUser, Session session) : base(session)
        {
            FirstType = "user";
            SecondType = "user";

            FirstValue = firstUser.Name;
            SecondValue = secondUser.Name;
        }

        /// <summary>
        /// Compare a list of artists with another list of artists.
        /// </summary>
        /// <param name="firstArtists">A <see cref="IEnumerable`1"/></param>
        /// <param name="secondArtists">A <see cref="IEnumerable`1"/></param>
        /// <param name="session">A <see cref="Session"/></param>
        public Tasteometer(IEnumerable<Artist> firstArtists, IEnumerable<Artist> secondArtists, Session session) : base(session)
        {
            FirstType = "artists";
            SecondType = "artists";

            foreach (var artist in firstArtists)
            {
                FirstValue += "," + artist.Name;
            }

            foreach (var artist in secondArtists)
            {
                SecondValue += "," + artist.Name;
            }
        }

        /// <summary>
        /// Compare a myspace profile with another.
        /// </summary>
        /// <param name="firstMyspaceURL">A <see cref="string"/></param>
        /// <param name="secondMyspaceURL">A <see cref="string"/></param>
        /// <param name="session">A <see cref="Session"/></param>
        public Tasteometer(string firstMyspaceURL, string secondMyspaceURL, Session session) : base(session)
        {
            FirstType = "myspace";
            SecondType = "myspace";

            FirstValue = firstMyspaceURL;
            SecondValue = secondMyspaceURL;
        }

        /// <summary>
        /// Compare a user with a list of artists.
        /// </summary>
        /// <param name="user">A <see cref="User"/></param>
        /// <param name="artists">A <see cref="IEnumerable`1"/></param>
        /// <param name="session">A <see cref="Session"/></param>
        public Tasteometer(User user, IEnumerable<Artist> artists, Session session) : base(session)
        {
            FirstType = "user";
            SecondType = "artists";

            FirstValue = user.Name;

            foreach (var artist in artists)
            {
                SecondValue += "," + artist.Name;
            }
        }

        /// <summary>
        /// Compare a user with a myspace profile.
        /// </summary>
        /// <param name="user">A <see cref="User"/></param>
        /// <param name="myspaceURL">A <see cref="string"/></param>
        /// <param name="session">A <see cref="Session"/></param>
        public Tasteometer(User user, string myspaceURL, Session session) : base(session)
        {
            FirstType = "user";
            SecondType = "myspace";

            FirstValue = user.Name;
            SecondValue = myspaceURL;
        }

        /// <summary>
        /// Compare a list of artists with a myspace profile.
        /// </summary>
        /// <param name="artists">A <see cref="IEnumerable`1"/></param>
        /// <param name="myspaceURL">A <see cref="string"/></param>
        /// <param name="session">A <see cref="Session"/></param>
        public Tasteometer(IEnumerable<Artist> artists, string myspaceURL, Session session) : base(session)
        {
            FirstType = "artists";
            SecondType = "myspace";

            foreach (var artist in artists)
            {
                FirstValue += "," + artist.Name;
            }

            SecondValue = myspaceURL;
        }

        /// <summary>
        /// Returns the comparison percentage.
        /// </summary>
        /// <returns>A <see cref="System.Single"/></returns>
        public float GetScore()
        {
            XmlDocument doc = Request("tasteometer.compare");

            return float.Parse(Extract(doc, "score"));
        }

        /// <summary>
        /// Returns the shared artits.
        /// </summary>
        /// <returns>A <see cref="Artist"/></returns>
        public Artist[] GetSharedArtists()
        {
            // default limit value
            return GetSharedArtists(5);
        }

        /// <summary>
        /// Returns the shared artists.
        /// </summary>
        /// <param name="limit">A <see cref="int"/></param>
        /// <returns>A <see cref="Artist"/></returns>
        public Artist[] GetSharedArtists(int limit)
        {
            var parameters = GetParams();
            parameters["limit"] = limit.ToString();

            XmlDocument doc = Request("tasteometer.compare", parameters);

            var list = new List<Artist>();

            foreach (XmlNode node in doc.GetElementsByTagName("artist"))
            {
                list.Add(new Artist(Extract(node, "name"), Session));
            }

            return list.ToArray();
        }
    }
}
