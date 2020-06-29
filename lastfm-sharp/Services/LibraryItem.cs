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
    /// An item in a user's library.
    /// </summary>
    public class LibraryItem<T>
    {
        protected internal T Item { get; private set; }

        /// <summary>
        /// How many times the user have played it.
        /// </summary>
        public int Playcount { get; private set; }

        /// <summary>
        /// How many tags have the user set to it.
        /// </summary>
        public int Tagcount { get; private set; }

        public LibraryItem(T item, int playcount, int tagcount)
        {
            Item = item;
            Playcount = playcount;
            Tagcount = tagcount;
        }
    }
}
