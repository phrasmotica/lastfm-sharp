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
    /// Global functions that don't fit anywhere else.
    /// </summary>
    public class Global : Base
    {
        public Global(Session session) : base(session)
        {
        }

        internal override RequestParameters GetParams()
        {
            return new RequestParameters();
        }

        /// <summary>
        /// Returns the most popular tags on Last.fm.
        /// </summary>
        /// <returns>A <see cref="TopTag"/></returns>
        public TopTag[] GetTopTags()
        {
            XmlDocument doc = Request("tag.getTopTags");

            var list = new List<TopTag>();
            foreach (XmlNode node in doc.GetElementsByTagName("tag"))
            {
                var tag = new Tag(Extract(node, "name"), Session);
                var count = int.Parse(Extract(node, "count"));

                list.Add(new TopTag(tag, count));
            }

            return list.ToArray();
        }
    }
}
