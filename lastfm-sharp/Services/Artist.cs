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
using System.Linq;
using System.Xml;

namespace Lastfm.Services
{
    /// <summary>
    /// An artist on Last.fm
    /// </summary>
    public class Artist : Base, ITaggable, IEquatable<Artist>, IShareable, IHasImage, IHasURL
    {
        /// <summary>
        /// The name of the artist.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The current wiki version.
        /// </summary>
        public ArtistBio Bio => new ArtistBio(this, Session);

        public Artist(string name, Session session) : base(session)
        {
            Name = name;
        }

        /// <summary>
        /// String representation of the object.
        /// </summary>
        /// <returns>A <see cref="string"/></returns>
        public override string ToString()
        {
            return Name;
        }

        internal override RequestParameters GetParams()
        {
            return new RequestParameters
            {
                ["artist"] = Name
            };
        }

        /// <summary>
        /// Returns the similar artists to this artist ordered by similarity from the most similar
        /// to the least similar.
        /// </summary>
        /// <param name="limit">A <see cref="int"/></param>
        /// <returns>A <see cref="Artist"/></returns>
        public Artist[] GetSimilar(int limit)
        {
            var parameters = GetParams();
            if (limit > -1)
            {
                parameters["limit"] = limit.ToString();
            }

            XmlDocument doc = Request("artist.getSimilar", parameters);

            var names = ExtractAll(doc, "name");

            var list = new List<Artist>();

            foreach (var name in names)
            {
                list.Add(new Artist(name, Session));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the similar artists to this artist ordered by similarity from the most similar
        /// to the least similar.
        /// </summary>
        /// <returns>A <see cref="Artist"/></returns>
        public Artist[] GetSimilar()
        {
            return GetSimilar(-1);
        }

        /// <summary>
        /// Returns the total number of listeners on Last.fm.
        /// </summary>
        /// <returns>A <see cref="int"/></returns>
        public int GetListenerCount()
        {
            XmlDocument doc = Request("artist.getInfo");

            return Convert.ToInt32(Extract(doc, "listeners"));
        }

        /// <summary>
        /// Returns the total number of playcounts on Last.fm.
        /// </summary>
        /// <returns>A <see cref="int"/></returns>
        public int GetPlaycount()
        {
            XmlDocument doc = Request("artist.getInfo");

            return Convert.ToInt32(Extract(doc, "playcount"));
        }

        /// <summary>
        /// Returns the url of the artist's image on Last.fm.
        /// </summary>
        /// <param name="size">A <see cref="ImageSize"/></param>
        /// <returns>A <see cref="string"/></returns>
        public string GetImageURL(ImageSize size)
        {
            XmlDocument doc = Request("artist.getInfo");

            var sizes = ExtractAll(doc, "image", 3);

            return sizes[(int) size];
        }

        /// <summary>
        /// Returns the url of the artist's image on Last.fm.
        /// </summary>
        /// <returns>A <see cref="System.String"/></returns>
        public string GetImageURL()
        {
            return GetImageURL(ImageSize.Large);
        }

        /// <summary>
        /// Returns the top most popular tracks by this artist.
        /// </summary>
        /// <returns>A <see cref="TopTrack"/></returns>
        public TopTrack[] GetTopTracks()
        {
            XmlDocument doc = Request("artist.getTopTracks");

            var list = new List<TopTrack>();

            foreach (XmlNode n in doc.GetElementsByTagName("track"))
            {
                var track = new Track(Extract(n, "name", 1), Extract(n, "name"), Session);
                var weight = int.Parse(Extract(n, "playcount"));

                list.Add(new TopTrack(track, weight));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns a list of upcoming events for this artist.
        /// </summary>
        /// <returns>A <see cref="Event"/></returns>
        public Event[] GetEvents()
        {
            XmlDocument doc = Request("artist.getEvents");

            var list = new List<Event>();
            foreach (var id in ExtractAll(doc, "id"))
            {
                list.Add(new Event(int.Parse(id), Session));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the most popular albums by this artist.
        /// </summary>
        /// <returns>A <see cref="TopAlbum"/></returns>
        public TopAlbum[] GetTopAlbums()
        {
            XmlDocument doc = Request("artist.getTopAlbums");

            var list = new List<TopAlbum>();
            foreach (XmlNode n in doc.GetElementsByTagName("album"))
            {
                var album = new Album(Extract(n, "name", 1), Extract(n, "name"), Session);
                var weight = int.Parse(Extract(n, "playcount"));

                list.Add(new TopAlbum(album, weight));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the the top fans for this artist.
        /// </summary>
        /// <returns>A <see cref="TopFan"/></returns>
        public TopFan[] GetTopFans()
        {
            XmlDocument doc = Request("artist.getTopFans");

            var list = new List<TopFan>();
            foreach (XmlNode node in doc.GetElementsByTagName("user"))
            {
                var user = new User(Extract(node, "name"), Session);
                var weight = int.Parse(Extract(node, "weight"));

                list.Add(new TopFan(user, weight));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Check to see if this object equals another.
        /// </summary>
        /// <param name="artist">A <see cref="Artist"/></param>
        /// <returns>A <see cref="bool"/></returns>
        public bool Equals(Artist artist)
        {
            return artist.Name == Name;
        }

        /// <summary>
        /// Share this artist with others.
        /// </summary>
        /// <param name="recipients">A <see cref="Recipients"/></param>
        /// <param name="message">A <see cref="string"/></param>
        public void Share(Recipients recipients, string message)
        {
            if (recipients.Count > 1)
            {
                foreach (var recipient in recipients)
                {
                    var r = new Recipients
                    {
                        recipient
                    };

                    Share(r, message);
                }

                return;
            }

            RequireAuthentication();

            var parameters = GetParams();
            parameters["recipient"] = recipients[0];
            parameters["message"] = message;

            Request("artist.Share", parameters);
        }

        /// <summary>
        /// Share this artist with others.
        /// </summary>
        /// <param name="recipients">A <see cref="Recipients"/></param>
        public void Share(Recipients recipients)
        {
            if (recipients.Count > 1)
            {
                foreach (string recipient in recipients)
                {
                    var r = new Recipients
                    {
                        recipient
                    };
                    Share(r);
                }

                return;
            }

            RequireAuthentication();

            var parameters = GetParams();
            parameters["recipient"] = recipients[0];

            Request("artist.Share", parameters);
        }

        /// <summary>
        /// Search for artists on Last.fm.
        /// </summary>
        /// <param name="artistName">A <see cref="string"/></param>
        /// <param name="session">A <see cref="Session"/></param>
        /// <returns>A <see cref="ArtistSearch"/></returns>
        public static ArtistSearch Search(string artistName, Session session)
        {
            return new ArtistSearch(artistName, session);
        }

        /// <summary>
        /// Add one or more tags to this artist.
        /// </summary>
        /// <param name="tags">A <see cref="Tag"/></param>
        public void AddTags(params Tag[] tags)
        {
            //This method requires authentication
            RequireAuthentication();

            foreach (var tag in tags)
            {
                var parameters = GetParams();
                parameters["tags"] = tag.Name;

                Request("artist.addTags", parameters);
            }
        }

        /// <summary>
        /// Add one or more tags to this artist.
        /// </summary>
        /// <param name="tags">A <see cref="string"/></param>
        public void AddTags(params string[] tags)
        {
            foreach (var tag in tags)
            {
                AddTags(new Tag(tag, Session));
            }
        }

        /// <summary>
        /// Add one or more tags to this artist.
        /// </summary>
        /// <param name="tags">A <see cref="TagCollection"/></param>
        public void AddTags(TagCollection tags)
        {
            foreach (var tag in tags)
            {
                AddTags(tag);
            }
        }

        /// <summary>
        /// Returns the tags set by the authenticated user to this artist.
        /// </summary>
        /// <returns>A <see cref="Tag"/></returns>
        public Tag[] GetTags()
        {
            //This method requires authentication
            RequireAuthentication();

            XmlDocument doc = Request("artist.getTags");

            TagCollection collection = new TagCollection(Session);

            foreach (var name in ExtractAll(doc, "name"))
            {
                collection.Add(name);
            }

            return collection.ToArray();
        }

        /// <summary>
        /// Returns the top tags of this artist on Last.fm.
        /// </summary>
        /// <returns>A <see cref="TopTag"/></returns>
        public TopTag[] GetTopTags()
        {
            XmlDocument doc = Request("artist.getTopTags");

            var list = new List<TopTag>();
            foreach (XmlNode n in doc.GetElementsByTagName("tag"))
            {
                list.Add(new TopTag(new Tag(Extract(n, "name"), Session), int.Parse(Extract(n, "count"))));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the top tags of this artist on Last.fm.
        /// </summary>
        /// <param name="limit">A <see cref="int"/></param>
        /// <returns>A <see cref="TopTag"/></returns>
        public TopTag[] GetTopTags(int limit)
        {
            return Sublist(GetTopTags(), limit);
        }

        /// <summary>
        /// Removes from the tags that the authenticated user has set to this artist.
        /// </summary>
        /// <param name="tags">A <see cref="Tag"/></param>
        public void RemoveTags(params Tag[] tags)
        {
            //This method requires authentication
            RequireAuthentication();

            foreach (var tag in tags)
            {
                var parameters = GetParams();
                parameters["tag"] = tag.Name;

                Request("artist.removeTag", parameters);
            }
        }

        /// <summary>
        /// Removes from the tags that the authenticated user has set to this artist.
        /// </summary>
        /// <param name="tags">A <see cref="string"/></param>
        public void RemoveTags(params string[] tags)
        {
            //This method requires authentication
            RequireAuthentication();

            foreach (var tag in tags)
            {
                RemoveTags(new Tag(tag, Session));
            }
        }

        /// <summary>
        /// Removes from the tags that the authenticated user has set to this artist.
        /// </summary>
        /// <param name="tags">A <see cref="TagCollection"/></param>
        public void RemoveTags(TagCollection tags)
        {
            foreach (var tag in tags)
            {
                RemoveTags(tag);
            }
        }

        /// <summary>
        /// Sets the tags applied by the authenticated user to this artist to only those tags.
        /// Removing and adding tags as neccessary.
        /// </summary>
        /// <param name="tags">A <see cref="string"/></param>
        public void SetTags(string[] tags)
        {
            var list = new List<Tag>();
            foreach (var name in tags)
            {
                list.Add(new Tag(name, Session));
            }

            SetTags(list.ToArray());
        }

        /// <summary>
        /// Sets the tags applied by the authenticated user to this artist to only those tags.
        /// Removing and adding tags as neccessary.
        /// </summary>
        /// <param name="tags">A <see cref="Tag"/></param>
        public void SetTags(Tag[] tags)
        {
            List<Tag> newSet = new List<Tag>(tags);
            List<Tag> current = new List<Tag>(GetTags());
            List<Tag> toAdd = new List<Tag>();
            List<Tag> toRemove = new List<Tag>();

            foreach (var tag in newSet)
            {
                if (!current.Contains(tag))
                {
                    toAdd.Add(tag);
                }
            }

            foreach (var tag in current)
            {
                if (!newSet.Contains(tag))
                {
                    toRemove.Add(tag);
                }
            }

            if (toAdd.Any())
            {
                AddTags(toAdd.ToArray());
            }

            if (toRemove.Any())
            {
                RemoveTags(toRemove.ToArray());
            }
        }

        /// <summary>
        /// Sets the tags applied by the authenticated user to this artist to only those tags.
        /// Removing and adding tags as neccessary.
        /// </summary>
        /// <param name="tags">A <see cref="TagCollection"/></param>
        public void SetTags(TagCollection tags)
        {
            SetTags(tags.ToArray());
        }

        /// <summary>
        /// Clears the tags that the authenticated user has set to this artist.
        /// </summary>
        public void ClearTags()
        {
            foreach (var tag in GetTags())
            {
                RemoveTags(tag);
            }
        }

        /// <summary>
        /// Returns an artist by their MusicBrainz artist id.
        /// </summary>
        /// <param name="mbid">A <see cref="System.String"/></param>
        /// <param name="session">A <see cref="Session"/></param>
        /// <returns>A <see cref="Artist"/></returns>
        public static Artist GetByMBID(string mbid, Session session)
        {
            var parameters = new RequestParameters
            {
                ["mbid"] = mbid
            };

            XmlDocument doc = new Request("artist.getInfo", session, parameters).Execute();

            var name = doc.GetElementsByTagName("name")[0].InnerText;

            return new Artist(name, session);
        }

        /// <summary>
        /// Returns the artist's MusicBrainz id.
        /// </summary>
        /// <returns>A <see cref="string"/></returns>
        public string GetMBID()
        {
            XmlDocument doc = Request("artist.getInfo");

            return Extract(doc, "mbid");
        }

        /// <summary>
        /// Returns the Last.fm page of this object.
        /// </summary>
        /// <param name="language">A <see cref="SiteLanguage"/></param>
        /// <returns>A <see cref="string"/></returns>
        public string GetURL(SiteLanguage language)
        {
            var domain = GetSiteDomain(language);

            return $"http://{domain}/music/{UrlSafe(Name)}";
        }

        /// <summary>
        /// The object's Last.fm page url.
        /// </summary>
        public string URL => GetURL(SiteLanguage.English);

        /// <summary>
        /// Returns true if the artist's music is available for streaming.
        /// </summary>
        /// <returns>A <see cref="bool"/></returns>
        public bool IsStreamable()
        {
            XmlDocument doc = Request("artist.getInfo");

            return Extract(doc, "streamable") == "1";
        }
    }
}
