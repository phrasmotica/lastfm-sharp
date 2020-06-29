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
    /// The members of a Last.fm group.
    /// </summary>
    public class GroupMembers : Pages<User>
    {
        /// <summary>
        /// The concerned group.
        /// </summary>
        public Group Group { get; private set; }

        public GroupMembers(Group group, Session session) : base("group.getMembers", session)
        {
            Group = group;
        }

        internal override RequestParameters GetParams()
        {
            return Group.GetParams();
        }

        public override User[] GetPage(int page)
        {
            if (page < 1)
            {
                throw new InvalidPageException(page, 1);
            }

            var parameters = GetParams();
            parameters["page"] = page.ToString();

            XmlDocument doc = Group.Request("group.getMembers", parameters);

            var list = new List<User>();
            foreach (XmlNode node in doc.GetElementsByTagName("user"))
            {
                var user = new User(Extract(node, "name"), Session);
                list.Add(user);
            }

            return list.ToArray();
        }
    }
}
