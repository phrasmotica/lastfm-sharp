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

namespace Lastfm.Services
{
    /// <summary>
    /// The albums in a <see cref="Library"/>.
    /// </summary>
    public class LibraryAlbums : Pages<LibraryAlbum>
    {
        /// <summary>
        /// The library.
        /// </summary>
        public Library Library { get; private set; }

        public LibraryAlbums(Library library, Session session) : base("library.getAlbums", session)
        {
            Library = library;
        }

        internal override RequestParameters GetParams()
        {
            return Library.GetParams();
        }

        public override LibraryAlbum[] GetPage(int page)
        {
            if (page < 1)
            {
                throw new Exception("The first page is 1.");
            }

            var parameters = GetParams();
            parameters["page"] = page.ToString();

            XmlDocument doc = Request("library.getAlbums", parameters);

            List<LibraryAlbum> list = new List<LibraryAlbum>();

            foreach (XmlNode node in doc.GetElementsByTagName("album"))
            {
                var playcount = 0;
                try
                {
                    playcount = int.Parse(Extract(node, "playcount"));
                }
                catch (FormatException)
                {
                }

                var tagcount = 0;
                try
                {
                    tagcount = int.Parse(Extract(node, "tagcount"));
                }
                catch (FormatException)
                {
                }

                Album album = new Album(Extract(node, "name", 1), Extract(node, "name"), Session);
                list.Add(new LibraryAlbum(album, playcount, tagcount));
            }

            return list.ToArray();
        }
    }
}
