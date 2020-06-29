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
using System.Xml;

namespace Lastfm.Services
{
    /// <summary>
    /// An abstract wiki object.
    /// </summary>
    public abstract class Wiki : Base
    {
        private string Prefix { get; set; }

        internal Wiki(string prefix, Session session) : base(session)
        {
            Prefix = prefix;
        }

        /// <summary>
        /// Returns the date that the current version of the wiki was published on.
        /// </summary>
        /// <returns>A <see cref="DateTime"/></returns>
        public DateTime GetPublishedDate()
        {
            XmlDocument doc = Request(Prefix + ".getInfo");

            return DateTime.Parse(Extract(doc, "published"));
        }

        /// <summary>
        /// Returns the summary of the content.
        /// </summary>
        /// <returns>A <see cref="string"/></returns>
        public string GetSummary()
        {
            // TODO: Clean the string before return

            XmlDocument doc = Request(Prefix + ".getInfo");

            return Extract(doc, "summary");
        }

        /// <summary>
        /// Returns the entire content of the current version.
        /// </summary>
        /// <returns>A <see cref="string"/></returns>
        public string getContent()
        {
            // FIXME: Clean the string first

            XmlDocument doc = Request(Prefix + ".getInfo");

            return Extract(doc, "content");
        }
    }
}
