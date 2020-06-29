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
    /// A user's library.
    /// </summary>
    public class Library : Base, IHasURL
    {
        /// <summary>
        /// The user who owns the library.
        /// </summary>
        public User User { get; private set; }

        public Library(User user, Session session) : base(session)
        {
            User = user;
        }

        public Library(string username, Session session) : base(session)
        {
            User = new User(username, Session);
        }

        internal override RequestParameters GetParams()
        {
            return new RequestParameters
            {
                ["user"] = User.Name
            };
        }

        /// <summary>
        /// String representation of the object.
        /// </summary>
        /// <returns>A <see cref="string"/></returns>
        public override string ToString()
        {
            return $"The library of {User.Name}";
        }

        /// <summary>
        /// Add an album to the library.
        /// </summary>
        /// <param name="album">A <see cref="Album"/></param>
        public void AddAlbum(Album album)
        {
            var parameters = GetParams();

            parameters["artist"] = album.Artist.Name;
            parameters["album"] = album.Title;

            Request("library.addAlbum", parameters);
        }

        /// <summary>
        /// Add an artist to the library.
        /// </summary>
        /// <param name="artist">A <see cref="Artist"/></param>
        public void AddArtist(Artist artist)
        {
            var parameters = GetParams();

            parameters["artist"] = artist.Name;

            Request("library.addArtist", parameters);
        }

        /// <summary>
        /// Add a track to the library.
        /// </summary>
        /// <param name="track">A <see cref="Track"/></param>
        public void AddTrack(Track track)
        {
            var parameters = GetParams();

            parameters["artist"] = track.Artist.Name;
            parameters["track"] = track.Title;

            Request("library.addTrack", parameters);
        }

        /// <summary>
        /// The albums in the library.
        /// </summary>
        public LibraryAlbums Albums => new LibraryAlbums(this, Session);

        /// <summary>
        /// The tracks in the library.
        /// </summary>
        public LibraryTracks Tracks => new LibraryTracks(this, Session);

        /// <summary>
        /// The artists in the library.
        /// </summary>
        public LibraryArtists Artists => new LibraryArtists(this, Session);

        /// <summary>
        /// Returns the Last.fm page of this object.
        /// </summary>
        /// <param name="language">A <see cref="SiteLanguage"/></param>
        /// <returns>A <see cref="string"/></returns>
        public string GetURL(SiteLanguage language)
        {
            return $"{User.GetURL(language)}/library";
        }

        /// <summary>
        /// The object's Last.fm page url.
        /// </summary>
        public string URL => GetURL(SiteLanguage.English);
    }
}
