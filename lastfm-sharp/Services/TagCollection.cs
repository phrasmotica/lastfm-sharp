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

namespace Lastfm.Services
{
    /// <summary>
    /// A collection of tags.
    /// </summary>
    public class TagCollection : List<Tag>
    {
        private Session Session { get; set; }

        public TagCollection(Session session) : base()
        {
            Session = session;
        }

        /// <summary>
        /// Add a tag name.
        /// </summary>
        /// <param name="tag">A <see cref="string"/></param>
        public void Add(string tag)
        {
            var item = new Tag(tag, Session);
            Add(item);
        }
    }
}
