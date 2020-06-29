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
    /// A Last.fm track.
    /// </summary>
    public class Track : Base, IEquatable<Track>, IShareable, ITaggable, IHasURL
    {
        /// <summary>
        /// The track title.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// The artist.
        /// </summary>
        public Artist Artist { get; private set; }

        /// <summary>
        /// The track wiki on Last.fm.
        /// </summary>
        public Wiki Wiki => new TrackWiki(this, Session);

        public Track(string artistName, string title, Session session) : base(session)
        {
            Title = title;
            Artist = new Artist(artistName, Session);
        }

        public Track(Artist artist, string title, Session session) : base(session)
        {
            Title = title;
            Artist = artist;
        }

        /// <summary>
        /// String representation of the object.
        /// </summary>
        /// <returns>A <see cref="string"/></returns>
        public override string ToString()
        {
            return $"{Artist} - {Title}";
        }

        internal override RequestParameters GetParams()
        {
            return new RequestParameters
            {
                ["artist"] = Artist.Name,
                ["track"] = Title
            };
        }

        /// <summary>
        /// A unique Last.fm ID.
        /// </summary>
        /// <returns>A <see cref="int"/></returns>
        public int GetID()
        {
            XmlDocument doc = Request("track.getInfo");

            return int.Parse(Extract(doc, "id"));
        }

        /// <summary>
        /// Returns the duration.
        /// </summary>
        /// <returns>A <see cref="TimeSpan"/></returns>
        public TimeSpan GetDuration()
        {
            XmlDocument doc = Request("track.getInfo");

            // Duration is returned in milliseconds.
            return new TimeSpan(0, 0, 0, 0, int.Parse(Extract(doc, "duration")));
        }

        /// <summary>
        /// Returns true if the track is available for streaming.
        /// </summary>
        /// <returns>A <see cref="bool"/></returns>
        public bool IsStreamable()
        {
            XmlDocument doc = Request("track.getInfo");

            int value = int.Parse(Extract(doc, "streamable"));

            return value == 1;
        }

        /// <summary>
        /// Returns the album of this track.
        /// </summary>
        /// <returns>A <see cref="Album"/></returns>
        public Album GetAlbum()
        {
            XmlDocument doc = Request("track.getInfo");

            if (doc.GetElementsByTagName("album").Count > 0)
            {
                XmlNode n = doc.GetElementsByTagName("album")[0];

                var artist = Extract(n, "artist");
                var title = Extract(n, "title");

                return new Album(artist, title, Session);
            }

            return null;
        }

        /// <summary>
        /// Ban this track.
        /// </summary>
        public void Ban()
        {
            //This method requires authentication
            RequireAuthentication();

            Request("track.ban");
        }

        /// <summary>
        /// Return similar tracks.
        /// </summary>
        /// <returns>A <see cref="Track"/></returns>
        public Track[] GetSimilar()
        {
            XmlDocument doc = Request("track.getSimilar");

            List<Track> list = new List<Track>();

            foreach (XmlNode n in doc.GetElementsByTagName("track"))
            {
                var track = new Track(Extract(n, "name", 1), Extract(n, "name"), Session);
                list.Add(track);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Love this track.
        /// </summary>
        public void Love()
        {
            //This method requires authentication
            RequireAuthentication();

            Request("track.love");
        }

        public void UnLove()
        {
            //This method requires authentication
            RequireAuthentication();

            Request("track.unlove");
        }

        /// <summary>
        /// Check to see if this object equals another.
        /// </summary>
        /// <param name="track">A <see cref="Track"/></param>
        /// <returns>A <see cref="System.Boolean"/></returns>
        public bool Equals(Track track)
        {
            return track.Title == Title && track.Artist.Name == Artist.Name;
        }

        /// <summary>
        /// Share this track with others.
        /// </summary>
        /// <param name="recipients">A <see cref="Recipients"/></param>
        /// <param name="message">A <see cref="System.String"/></param>
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

            Request("track.Share", parameters);
        }

        /// <summary>
        /// Share this track with others.
        /// </summary>
        /// <param name="recipients">A <see cref="Recipients"/></param>
        public void Share(Recipients recipients)
        {
            if (recipients.Count > 1)
            {
                foreach (var recipient in recipients)
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

            Request("track.Share", parameters);
        }

        /// <summary>
        /// Search for tracks on Last.fm.
        /// </summary>
        /// <param name="artist">A <see cref="string"/></param>
        /// <param name="title">A <see cref="string"/></param>
        /// <param name="session">A <see cref="Session"/></param>
        /// <returns>A <see cref="TrackSearch"/></returns>
        public static TrackSearch Search(string artist, string title, Session session)
        {
            return new TrackSearch(artist, title, session);
        }

        /// <summary>
        /// Search for tracks on Last.fm.
        /// </summary>
        /// <param name="title">A <see cref="string"/></param>
        /// <param name="session">A <see cref="Session"/></param>
        /// <returns>A <see cref="TrackSearch"/></returns>
        public static TrackSearch Search(string title, Session session)
        {
            return new TrackSearch(title, session);
        }

        /// <summary>
        /// Add tags to this track.
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

                Request("track.addTags", parameters);
            }
        }

        /// <summary>
        /// Add tags to this track.
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
        /// Add tags to this track.
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
        /// Returns the tags set by the authenticated user to this track.
        /// </summary>
        /// <returns>A <see cref="Tag"/></returns>
        public Tag[] GetTags()
        {
            //This method requires authentication
            RequireAuthentication();

            XmlDocument doc = Request("track.getTags");

            var collection = new TagCollection(Session);

            foreach (var name in ExtractAll(doc, "name"))
            {
                collection.Add(name);
            }

            return collection.ToArray();
        }

        /// <summary>
        /// Return the top tags.
        /// </summary>
        /// <returns>A <see cref="TopTag"/></returns>
        public TopTag[] GetTopTags()
        {
            XmlDocument doc = Request("track.getTopTags");

            var list = new List<TopTag>();
            foreach (XmlNode n in doc.GetElementsByTagName("tag"))
            {
                var tag = new Tag(Extract(n, "name"), Session);
                var count = int.Parse(Extract(n, "count"));

                list.Add(new TopTag(tag, count));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the top tags.
        /// </summary>
        /// <param name="limit">A <see cref="int"/></param>
        /// <returns>A <see cref="TopTag"/></returns>
        public TopTag[] GetTopTags(int limit)
        {
            return Sublist(GetTopTags(), limit);
        }

        /// <summary>
        /// Remove a bunch of tags from this track.
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

                Request("track.removeTag", parameters);
            }
        }

        /// <summary>
        /// Remove a bunch of tags from this track.
        /// </summary>
        /// <param name="tags">A <see cref="string"/></param>
        public void RemoveTags(params string[] tags)
        {
            //This method requires authentication
            RequireAuthentication();

            var tagsToRemove = tags.Select(t => new Tag(t, Session)).ToArray();
            RemoveTags(tagsToRemove);
        }

        /// <summary>
        /// Remove a bunch of tags from this track.
        /// </summary>
        /// <param name="tags">A <see cref="TagCollection"/></param>
        public void RemoveTags(TagCollection tags)
        {
            RemoveTags(tags.ToArray());
        }

        /// <summary>
        /// Set the tags of this tracks to only those tags.
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
        /// Set the tags of this tracks to only those tags.
        /// </summary>
        /// <param name="tags">A <see cref="Tag"/></param>
        public void SetTags(Tag[] tags)
        {
            var newSet = new List<Tag>(tags);
            var current = new List<Tag>(GetTags());
            var toAdd = new List<Tag>();
            var toRemove = new List<Tag>();

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
        /// Set the tags of this tracks to only those tags.
        /// </summary>
        /// <param name="tags">A <see cref="TagCollection"/></param>
        public void SetTags(TagCollection tags)
        {
            SetTags(tags.ToArray());
        }

        /// <summary>
        /// Clear all the tags from this track.
        /// </summary>
        public void ClearTags()
        {
            foreach (var tag in GetTags())
            {
                RemoveTags(tag);
            }
        }

        /// <summary>
        /// Returns the top fans.
        /// </summary>
        /// <returns>A <see cref="TopFan"/></returns>
        public TopFan[] GetTopFans()
        {
            XmlDocument doc = Request("track.getTopFans");

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
        /// Returns the top fans.
        /// </summary>
        /// <param name="limit">A <see cref="int"/></param>
        /// <returns>A <see cref="TopFan"/></returns>
        public TopFan[] GetTopFans(int limit)
        {
            return Sublist(GetTopFans(), limit);
        }

        /// <summary>
        /// Returns the track's MusicBrainz id.
        /// </summary>
        /// <returns>A <see cref="System.String"/></returns>
        public string GetMBID()
        {
            XmlDocument doc = Request("track.getInfo");

            return doc.GetElementsByTagName("mbid")[0].InnerText;
        }

        /// <summary>
        /// Returns a track by its MusicBrainz id.
        /// </summary>
        /// <param name="mbid">A <see cref="string"/></param>
        /// <param name="session">A <see cref="Session"/></param>
        /// <returns>A <see cref="Track"/></returns>
        public static Track GetByMBID(string mbid, Session session)
        {
            var parameters = new RequestParameters
            {
                ["mbid"] = mbid
            };

            XmlDocument doc = (new Request("track.getInfo", session, parameters)).Execute();

            var title = doc.GetElementsByTagName("name")[0].InnerText;
            var artist = doc.GetElementsByTagName("name")[1].InnerText;

            return new Track(artist, title, session);
        }

        /// <summary>
        /// Returns the Last.fm page of this object.
        /// </summary>
        /// <param name="language">A <see cref="SiteLanguage"/></param>
        /// <returns>A <see cref="string"/></returns>
        public string GetURL(SiteLanguage language)
        {
            var domain = GetSiteDomain(language);

            return $"http://{domain}/music/{UrlSafe(Artist.Name)}/_/{UrlSafe(Title)}";
        }

        /// <summary>
        /// The object's Last.fm page url.
        /// </summary>
        public string URL => GetURL(SiteLanguage.English);

        /// <summary>
        /// Add this track to a <see cref="Playlist"/>
        /// </summary>
        /// <param name="playlist">A <see cref="Playlist"/></param>
        public void AddToPlaylist(Playlist playlist)
        {
            playlist.AddTrack(this);
        }
    }
}
