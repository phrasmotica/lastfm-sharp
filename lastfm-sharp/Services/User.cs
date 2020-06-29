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
    /// A Last.fm user.
    /// </summary>
    public class User : Base, IEquatable<User>, IHasWeeklyTrackCharts, IHasWeeklyAlbumCharts, IHasWeeklyArtistCharts, IHasURL
    {
        /// <summary>
        /// The user's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Returns the user's library.
        /// </summary>
        public Library Library => new Library(this, Session);

        public User(string name, Session session) : base(session)
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
                ["user"] = Name
            };
        }

        /// <summary>
        /// Check to see if this object equals another.
        /// </summary>
        /// <param name="user">A <see cref="User"/></param>
        /// <returns>A <see cref="bool"/></returns>
        public bool Equals(User user)
        {
            return user.Name == Name;
        }

        /// <summary>
        /// Returns the latest weekly track chart.
        /// </summary>
        /// <returns>A <see cref="WeeklyTrackChart"/></returns>
        public WeeklyTrackChart GetWeeklyTrackChart()
        {
            XmlDocument doc = Request("user.getWeeklyTrackChart");

            XmlNode n = doc.GetElementsByTagName("weeklytrackchart")[0];

            var nfrom = Utilities.TimestampToDateTime(Int64.Parse(n.Attributes[1].InnerText), DateTimeKind.Utc);
            var nto = Utilities.TimestampToDateTime(Int64.Parse(n.Attributes[2].InnerText), DateTimeKind.Utc);

            var chart = new WeeklyTrackChart(new WeeklyChartTimeSpan(nfrom, nto));

            foreach (XmlNode node in doc.GetElementsByTagName("track"))
            {
                var rank = int.Parse(node.Attributes[0].InnerText);
                var playcount = int.Parse(Extract(node, "playcount"));

                var track = new Track(Extract(node, "artist"), Extract(node, "name"), Session);
                var timeSpan = new WeeklyChartTimeSpan(nfrom, nto);
                var item = new WeeklyTrackChartItem(track, rank, playcount, timeSpan);

                chart.Add(item);
            }

            return chart;
        }

        /// <summary>
        /// Returns a weekly chart specified by a certain week.
        /// </summary>
        /// <param name="span">A <see cref="WeeklyChartTimeSpan"/></param>
        /// <returns>A <see cref="WeeklyTrackChart"/></returns>
        public WeeklyTrackChart GetWeeklyTrackChart(WeeklyChartTimeSpan span)
        {
            var parameters = GetParams();

            parameters["from"] = Utilities.DateTimeToUTCTimestamp(span.From).ToString();
            parameters["to"] = Utilities.DateTimeToUTCTimestamp(span.To).ToString();

            XmlDocument doc = Request("user.getWeeklyTrackChart", parameters);

            XmlNode n = doc.GetElementsByTagName("weeklytrackchart")[0];

            var nfrom = Utilities.TimestampToDateTime(long.Parse(n.Attributes[1].InnerText), DateTimeKind.Utc);
            var nto = Utilities.TimestampToDateTime(long.Parse(n.Attributes[2].InnerText), DateTimeKind.Utc);

            var chart = new WeeklyTrackChart(new WeeklyChartTimeSpan(nfrom, nto));

            foreach (XmlNode node in doc.GetElementsByTagName("track"))
            {
                var rank = int.Parse(node.Attributes[0].InnerText);
                var playcount = int.Parse(Extract(node, "playcount"));

                var track = new Track(Extract(node, "artist"), Extract(node, "name"), Session);
                var timeSpan = new WeeklyChartTimeSpan(nfrom, nto);
                var item = new WeeklyTrackChartItem(track, rank, playcount, timeSpan);

                chart.Add(item);
            }

            return chart;
        }

        /// <summary>
        /// Returns the latest weekly artist chart.
        /// </summary>
        /// <returns>A <see cref="WeeklyArtistChart"/></returns>
        public WeeklyArtistChart GetWeeklyArtistChart()
        {
            XmlDocument doc = Request("user.getWeeklyArtistChart");

            XmlNode n = doc.GetElementsByTagName("weeklyartistchart")[0];

            var nfrom = Utilities.TimestampToDateTime(long.Parse(n.Attributes[1].InnerText), DateTimeKind.Utc);
            var nto = Utilities.TimestampToDateTime(long.Parse(n.Attributes[2].InnerText), DateTimeKind.Utc);

            var chart = new WeeklyArtistChart(new WeeklyChartTimeSpan(nfrom, nto));

            foreach (XmlNode node in doc.GetElementsByTagName("artist"))
            {
                var rank = int.Parse(node.Attributes[0].InnerText);
                var playcount = int.Parse(Extract(node, "playcount"));

                var artist = new Artist(Extract(node, "name"), Session);
                var timeSpan = new WeeklyChartTimeSpan(nfrom, nto);
                var item = new WeeklyArtistChartItem(artist, rank, playcount, timeSpan);

                chart.Add(item);
            }

            return chart;
        }

        /// <summary>
        /// Returns a weekly artist chart of a specified week.
        /// </summary>
        /// <param name="span">A <see cref="WeeklyChartTimeSpan"/></param>
        /// <returns>A <see cref="WeeklyArtistChart"/></returns>
        public WeeklyArtistChart GetWeeklyArtistChart(WeeklyChartTimeSpan span)
        {
            var parameters = GetParams();

            parameters["from"] = Utilities.DateTimeToUTCTimestamp(span.From).ToString();
            parameters["to"] = Utilities.DateTimeToUTCTimestamp(span.To).ToString();

            XmlDocument doc = Request("user.getWeeklyArtistChart", parameters);

            XmlNode n = doc.GetElementsByTagName("weeklyartistchart")[0];

            var nfrom = Utilities.TimestampToDateTime(long.Parse(n.Attributes[1].InnerText), DateTimeKind.Utc);
            var nto = Utilities.TimestampToDateTime(long.Parse(n.Attributes[2].InnerText), DateTimeKind.Utc);

            var chart = new WeeklyArtistChart(new WeeklyChartTimeSpan(nfrom, nto));

            foreach (XmlNode node in doc.GetElementsByTagName("artist"))
            {
                var rank = int.Parse(node.Attributes[0].InnerText);
                var playcount = int.Parse(Extract(node, "playcount"));

                var artist = new Artist(Extract(node, "name"), Session);
                var timeSpan = new WeeklyChartTimeSpan(nfrom, nto);
                var item = new WeeklyArtistChartItem(artist, rank, playcount, timeSpan);

                chart.Add(item);
            }

            return chart;
        }

        /// <summary>
        /// Returns the latest weekly album chart.
        /// </summary>
        /// <returns>A <see cref="WeeklyAlbumChart"/></returns>
        public WeeklyAlbumChart GetWeeklyAlbumChart()
        {
            XmlDocument doc = Request("user.getWeeklyAlbumChart");

            XmlNode n = doc.GetElementsByTagName("weeklyalbumchart")[0];

            var nfrom = Utilities.TimestampToDateTime(long.Parse(n.Attributes[1].InnerText), DateTimeKind.Utc);
            var nto = Utilities.TimestampToDateTime(long.Parse(n.Attributes[2].InnerText), DateTimeKind.Utc);

            var chart = new WeeklyAlbumChart(new WeeklyChartTimeSpan(nfrom, nto));

            foreach (XmlNode node in doc.GetElementsByTagName("album"))
            {
                var rank = int.Parse(node.Attributes[0].InnerText);
                var playcount = int.Parse(Extract(node, "playcount"));

                var album = new Album(Extract(node, "artist"), Extract(node, "name"), Session);
                var timeSpan = new WeeklyChartTimeSpan(nfrom, nto);
                var item = new WeeklyAlbumChartItem(album, rank, playcount, timeSpan);

                chart.Add(item);
            }

            return chart;
        }

        /// <summary>
        /// Returns a weekly album chart of a specified week.
        /// </summary>
        /// <param name="span">A <see cref="WeeklyChartTimeSpan"/></param>
        /// <returns>A <see cref="WeeklyAlbumChart"/></returns>
        public WeeklyAlbumChart GetWeeklyAlbumChart(WeeklyChartTimeSpan span)
        {
            var parameters = GetParams();

            parameters["from"] = Utilities.DateTimeToUTCTimestamp(span.From).ToString();
            parameters["to"] = Utilities.DateTimeToUTCTimestamp(span.To).ToString();

            XmlDocument doc = Request("user.getWeeklyAlbumChart", parameters);

            XmlNode n = doc.GetElementsByTagName("weeklyalbumchart")[0];

            var nfrom = Utilities.TimestampToDateTime(long.Parse(n.Attributes[1].InnerText), DateTimeKind.Utc);
            var nto = Utilities.TimestampToDateTime(long.Parse(n.Attributes[2].InnerText), DateTimeKind.Utc);

            var chart = new WeeklyAlbumChart(new WeeklyChartTimeSpan(nfrom, nto));

            foreach (XmlNode node in doc.GetElementsByTagName("album"))
            {
                var rank = int.Parse(node.Attributes[0].InnerText);
                var playcount = int.Parse(Extract(node, "playcount"));

                var album = new Album(Extract(node, "artist"), Extract(node, "name"), Session);
                var timeSpan = new WeeklyChartTimeSpan(nfrom, nto);
                var item = new WeeklyAlbumChartItem(album, rank, playcount, timeSpan);

                chart.Add(item);
            }

            return chart;
        }

        /// <summary>
        /// Returns all the vailable weeks (as an array of <see cref="WeeklyChartTimeSpan"/>)
        /// </summary>
        /// <returns>A <see cref="WeeklyChartTimeSpan"/></returns>
        public WeeklyChartTimeSpan[] GetWeeklyChartTimeSpans()
        {
            XmlDocument doc = Request("user.getWeeklyChartList");

            var list = new List<WeeklyChartTimeSpan>();
            foreach (XmlNode node in doc.GetElementsByTagName("chart"))
            {
                long lfrom = long.Parse(node.Attributes[0].InnerText);
                long lto = long.Parse(node.Attributes[1].InnerText);

                var from = Utilities.TimestampToDateTime(lfrom, DateTimeKind.Utc);
                var to = Utilities.TimestampToDateTime(lto, DateTimeKind.Utc);

                list.Add(new WeeklyChartTimeSpan(from, to));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the top tracks listened to by this user in a specified period.
        /// </summary>
        /// <param name="period">A <see cref="Period"/></param>
        /// <returns>A <see cref="TopTrack"/></returns>
        public TopTrack[] GetTopTracks(Period period)
        {
            var parameters = GetParams();
            parameters["period"] = getPeriod(period);

            XmlDocument doc = Request("user.getTopTracks", parameters);

            var list = new List<TopTrack>();
            foreach (XmlNode node in doc.GetElementsByTagName("track"))
            {
                var track = new Track(Extract(node, "name", 1), Extract(node, "name"), Session);
                var count = int.Parse(Extract(node, "playcount"));

                list.Add(new TopTrack(track, count));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the overall most listned-to tracks by this user.
        /// </summary>
        /// <returns>A <see cref="TopTrack"/></returns>
        public TopTrack[] GetTopTracks()
        {
            return GetTopTracks(Period.Overall);
        }

        /// <summary>
        /// Returns the top tags used by this user.
        /// </summary>
        /// <returns>A <see cref="TopTag"/></returns>
        public TopTag[] GetTopTags()
        {
            XmlDocument doc = Request("user.getTopTags");

            var list = new List<TopTag>();
            foreach (XmlNode node in doc.GetElementsByTagName("tag"))
            {
                var tag = new Tag(Extract(node, "name"), Session);
                var count = int.Parse(Extract(node, "count"));

                list.Add(new TopTag(tag, count));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the top tags used by this user.
        /// </summary>
        /// <param name="limit">A <see cref="int"/></param>
        /// <returns>A <see cref="TopTag"/></returns>
        public TopTag[] GetTopTags(int limit)
        {
            return Sublist(GetTopTags(), limit);
        }

        /// <summary>
        /// Returns the top artists listened-to by this user in a specified period.
        /// </summary>
        /// <param name="period">A <see cref="Period"/></param>
        /// <returns>A <see cref="TopArtist"/></returns>
        public TopArtist[] GetTopArtists(Period period)
        {
            var parameters = GetParams();
            parameters["period"] = getPeriod(period);

            XmlDocument doc = Request("user.getTopArtists", parameters);
            var list = new List<TopArtist>();

            foreach (XmlNode node in doc.GetElementsByTagName("artist"))
            {
                var artist = new Artist(Extract(node, "name"), Session);
                var playcount = int.Parse(Extract(node, "playcount"));

                list.Add(new TopArtist(artist, playcount));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns teh overall most listened-to artists by this user.
        /// </summary>
        /// <returns>A <see cref="TopArtist"/></returns>
        public TopArtist[] GetTopArtists()
        {
            return GetTopArtists(Period.Overall);
        }

        /// <summary>
        /// Returns the most listened-to albums by this user in a specified period.
        /// </summary>
        /// <param name="period">A <see cref="Period"/></param>
        /// <returns>A <see cref="TopAlbum"/></returns>
        public TopAlbum[] GetTopAlbums(Period period)
        {
            var paraeters = GetParams();
            paraeters["period"] = getPeriod(period);

            XmlDocument doc = Request("user.getTopAlbums", paraeters);
            var list = new List<TopAlbum>();

            foreach (XmlNode node in doc.GetElementsByTagName("album"))
            {
                var album = new Album(Extract(node, "name", 1), Extract(node, "name"), Session);
                var playcount = int.Parse(Extract(node, "playcount"));

                list.Add(new TopAlbum(album, playcount));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returne the overall most listened-to albums by this user.
        /// </summary>
        /// <returns>A <see cref="TopAlbum"/></returns>
        public TopAlbum[] GetTopAlbums()
        {
            return GetTopAlbums(Period.Overall);
        }

        /// <summary>
        /// Returns the most recent played tracks for this user.
        /// </summary>
        /// <param name="limit">A <see cref="int"/></param>
        /// <returns>A <see cref="Track"/></returns>
        public Track[] GetRecentTracks(int limit)
        {
            var parameters = GetParams();
            parameters["limit"] = limit.ToString();

            XmlDocument doc = Request("user.getRecentTracks", parameters);
            var list = new List<Track>();

            foreach (XmlNode node in doc.GetElementsByTagName("track"))
            {
                // skip the track that is now playing.
                if (node.Attributes.Count > 0)
                {
                    continue;
                }

                var track = new Track(Extract(node, "artist"), Extract(node, "name"), Session);
                list.Add(track);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the most recent played tracks for this user.
        /// </summary>
        /// <returns>A <see cref="Track"/></returns>
        public Track[] GetRecentTracks()
        {
            // default value is 10.
            return GetRecentTracks(10);
        }

        /// <summary>
        /// Returns the track that the user's currently listening to.
        /// </summary>
        /// <returns>
        /// A <see cref="Track"/> if the user is listening to a track, or null if they're not.
        /// </returns>
        public Track GetNowPlaying()
        {
            // Would return null if no track is now playing.

            RequestParameters p = GetParams();
            p["limit"] = "1";

            XmlDocument doc = Request("user.getRecentTracks", p);
            XmlNode node = doc.GetElementsByTagName("track")[0];

            if (node.Attributes.Count > 0)
            {
                return new Track(Extract(node, "artist"), Extract(node, "name"), Session);
            }

            return null;
        }

        /// <summary>
        /// Returns true if the user's listening right now.
        /// </summary>
        /// <returns>A <see cref="bool"/></returns>
        public bool IsNowListening()
        {
            return GetNowPlaying() != null;
        }

        /// <summary>
        /// Returns this user's playlists.
        /// </summary>
        /// <returns>A <see cref="Playlist"/></returns>
        public Playlist[] GetPlaylists()
        {
            XmlDocument doc = Request("user.getPlaylists");

            var list = new List<Playlist>();
            foreach (var id in ExtractAll(doc, "id"))
            {
                list.Add(new Playlist(Name, Int32.Parse(id), Session));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the past events attended by this user.
        /// </summary>
        public PastEvents PastEvents => new PastEvents(this, Session);

        /// <summary>
        /// Returns the neighbours of this user.
        /// </summary>
        /// <returns>A <see cref="User"/></returns>
        public User[] GetNeighbours()
        {
            XmlDocument doc = Request("user.getNeighbours");

            var list = new List<User>();
            foreach (var name in ExtractAll(doc, "name"))
            {
                var user = new User(name, Session);
                list.Add(user);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the neighbours of this user.
        /// </summary>
        /// <param name="limit">A <see cref="int"/></param>
        /// <returns>A <see cref="User"/></returns>
        public User[] GetNeighbours(int limit)
        {
            return Sublist(GetNeighbours(), limit);
        }

        /// <summary>
        /// Returns the most recent 50 loved tracks by this user.
        /// </summary>
        /// <returns>A <see cref="Track"/></returns>
        public Track[] GetLovedTracks()
        {
            XmlDocument doc = Request("user.getLovedTracks");

            var list = new List<Track>();
            foreach (XmlNode node in doc.GetElementsByTagName("track"))
            {
                var track = new Track(Extract(node, "name", 1), Extract(node, "name"), Session);
                list.Add(track);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the user's friends.
        /// </summary>
        /// <returns>A <see cref="User"/></returns>
        public User[] GetFriends()
        {
            XmlDocument doc = Request("user.getFriends");

            var list = new List<User>();
            foreach (var name in ExtractAll(doc, "name"))
            {
                var user = new User(name, Session);
                list.Add(user);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the user's friends.
        /// </summary>
        /// <param name="limit">A <see cref="int"/></param>
        /// <returns>A <see cref="User"/></returns>
        public User[] GetFriends(int limit)
        {
            return Sublist(GetFriends(), limit);
        }

        /// <summary>
        /// Returns the upcoming events for this user.
        /// </summary>
        /// <returns>A <see cref="Event"/></returns>
        public Event[] GetUpcomingEvents()
        {
            XmlDocument doc = Request("user.getEvents");

            var list = new List<Event>();
            foreach (var id in ExtractAll(doc, "id"))
            {
                var eventItem = new Event(int.Parse(id), Session);
                list.Add(eventItem);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Compare this user with another.
        /// </summary>
        /// <param name="anotherUser">A <see cref="User"/></param>
        /// <returns>A <see cref="Tasteometer"/></returns>
        public Tasteometer Compare(User anotherUser)
        {
            return new Tasteometer(this, anotherUser, Session);
        }

        /// <summary>
        /// Compare this user with a list of artists.
        /// </summary>
        /// <param name="artists">A <see cref="IEnumerable`1"/></param>
        /// <returns>A <see cref="Tasteometer"/></returns>
        public Tasteometer Compare(IEnumerable<Artist> artists)
        {
            return new Tasteometer(this, artists, Session);
        }

        /// <summary>
        /// Compare this user with a myspace profile.
        /// </summary>
        /// <param name="myspaceURL">A <see cref="string"/></param>
        /// <returns>A <see cref="Tasteometer"/></returns>
        public Tasteometer Compare(string myspaceURL)
        {
            return new Tasteometer(this, myspaceURL, Session);
        }

        /// <summary>
        /// Returns the Last.fm page of this object.
        /// </summary>
        /// <param name="language">A <see cref="SiteLanguage"/></param>
        /// <returns>A <see cref="string"/></returns>
        public string GetURL(SiteLanguage language)
        {
            var domain = GetSiteDomain(language);

            return $"http://{domain}/user/{UrlSafe(Name)}";
        }

        /// <summary>
        /// The object's Last.fm page url.
        /// </summary>
        public string URL => GetURL(SiteLanguage.English);
    }
}
