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
    public class PastEvents : Pages<Event>
    {
        public User User { get; private set; }

        public PastEvents(User user, Session session) : base("user.getPastEvents", session)
        {
            User = user;
        }

        internal override RequestParameters GetParams()
        {
            return User.GetParams();
        }

        public override Event[] GetPage(int page)
        {
            if (page < 1)
            {
                throw new InvalidPageException(page, 1);
            }

            var parameters = GetParams();
            parameters["page"] = page.ToString();

            XmlDocument doc = Request("user.getPastEvents", parameters);

            var list = new List<Event>();
            foreach (XmlNode node in doc.GetElementsByTagName("event"))
            {
                var eventItem = new Event(int.Parse(Extract(node, "id")), Session);
                list.Add(eventItem);
            }

            return list.ToArray();
        }
    }
}
