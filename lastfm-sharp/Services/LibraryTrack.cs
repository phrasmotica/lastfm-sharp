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
    /// A <see cref="Track"/> in a <see cref="Library"/>.
    /// </summary>
    public class LibraryTrack : LibraryItem<Track>
    {
        public LibraryTrack(Track track, int playcount, int tagcount)
            : base(track, playcount, tagcount)
        { }

        /// <summary>
        /// The track.
        /// </summary>
        public Track Track => Item;
    }
}
