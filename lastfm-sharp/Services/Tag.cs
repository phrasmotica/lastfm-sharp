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
    public class Tag : Base, IEquatable<Tag>, IHasWeeklyArtistCharts, IHasURL
    {
        /// <summary>
        /// The tag name.
        /// </summary>
        public string Name { get; private set; }

        public Tag(string name, Session session) : base(session)
        {
            Name = name;
        }

        internal override RequestParameters GetParams()
        {
            return new RequestParameters
            {
                ["tag"] = Name
            };
        }

        /// <summary>
        /// String representation of the object.
        /// </summary>
        /// <returns>A <see cref="string"/></returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Returns similar tags.
        /// </summary>
        /// <returns>A <see cref="Tag"/></returns>
        public Tag[] GetSimilar()
        {
            XmlDocument doc = Request("tag.getSimilar");

            var list = new List<Tag>();
            foreach (var name in ExtractAll(doc, "name"))
            {
                var tag = new Tag(name, Session);
                list.Add(tag);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the top albums tagged with this tag.
        /// </summary>
        /// <returns>A <see cref="TopAlbum"/></returns>
        public TopAlbum[] GetTopAlbums()
        {
            XmlDocument doc = Request("tag.getTopAlbums");

            var list = new List<TopAlbum>();
            foreach (XmlNode n in doc.GetElementsByTagName("album"))
            {
                var album = new Album(Extract(n, "name", 1), Extract(n, "name"), Session);
                var count = int.Parse(Extract(n, "tagcount"));

                list.Add(new TopAlbum(album, count));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the top artists tagged with this tag.
        /// </summary>
        /// <returns>A <see cref="TopArtist"/></returns>
        public TopArtist[] GetTopArtists()
        {
            XmlDocument doc = Request("tag.getTopArtists");

            var list = new List<TopArtist>();
            foreach (XmlNode node in doc.GetElementsByTagName("artist"))
            {
                var artist = new Artist(Extract(node, "name"), Session);
                var count = int.Parse(Extract(node, "tagcount"));

                list.Add(new TopArtist(artist, count));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the top tracks tagged with this track.
        /// </summary>
        /// <returns>A <see cref="TopTrack"/></returns>
        public TopTrack[] GetTopTracks()
        {
            XmlDocument doc = Request("tag.getTopTracks");

            var list = new List<TopTrack>();
            foreach (XmlNode n in doc.GetElementsByTagName("track"))
            {
                var weight = int.Parse(Extract(n, "tagcount"));
                var track = new Track(Extract(n, "name", 1), Extract(n, "name"), Session);

                list.Add(new TopTrack(track, weight));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Check to see if this object equals another.
        /// </summary>
        /// <param name="tag">A <see cref="Tag"/></param>
        /// <returns>A <see cref="bool"/></returns>
        public bool Equals(Tag tag)
        {
            return tag.Name == Name;
        }

        /// <summary>
        /// Search for tags by name.
        /// </summary>
        /// <param name="name">A <see cref="string"/></param>
        /// <param name="session">A <see cref="Session"/></param>
        /// <returns>A <see cref="TagSearch"/></returns>
        public static TagSearch Search(string name, Session session)
        {
            return new TagSearch(name, session);
        }

        /// <summary>
        /// Returns the available weekly chart time spans (weeks) for this tag.
        /// </summary>
        /// <returns>A <see cref="WeeklyChartTimeSpan"/></returns>
        public WeeklyChartTimeSpan[] GetWeeklyChartTimeSpans()
        {
            XmlDocument doc = Request("tag.getWeeklyChartList");

            var list = new List<WeeklyChartTimeSpan>();
            foreach (XmlNode node in doc.GetElementsByTagName("chart"))
            {
                var lfrom = long.Parse(node.Attributes[0].InnerText);
                var lto = long.Parse(node.Attributes[1].InnerText);

                var from = Utilities.TimestampToDateTime(lfrom, DateTimeKind.Utc);
                var to = Utilities.TimestampToDateTime(lto, DateTimeKind.Utc);

                list.Add(new WeeklyChartTimeSpan(from, to));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the latest weekly artist chart.
        /// </summary>
        /// <returns>A <see cref="WeeklyArtistChart"/></returns>
        public WeeklyArtistChart GetWeeklyArtistChart()
        {
            XmlDocument doc = Request("tag.getWeeklyArtistChart");

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
        /// Returns the weekly artist chart for a specified time span (week).
        /// </summary>
        /// <param name="span">A <see cref="WeeklyChartTimeSpan"/></param>
        /// <returns>A <see cref="WeeklyArtistChart"/></returns>
        public WeeklyArtistChart GetWeeklyArtistChart(WeeklyChartTimeSpan span)
        {
            var parameters = GetParams();

            parameters["from"] = Utilities.DateTimeToUTCTimestamp(span.From).ToString();
            parameters["to"] = Utilities.DateTimeToUTCTimestamp(span.To).ToString();

            XmlDocument doc = Request("tag.getWeeklyArtistChart", parameters);

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
        /// Returns the Last.fm page of this object.
        /// </summary>
        /// <param name="language">A <see cref="SiteLanguage"/></param>
        /// <returns>A <see cref="string"/></returns>
        public string GetURL(SiteLanguage language)
        {
            var domain = GetSiteDomain(language);

            return $"http://{domain}/tag/{UrlSafe(Name)}";
        }

        /// <summary>
        /// The object's Last.fm page url.
        /// </summary>
        public string URL => GetURL(SiteLanguage.English);
    }
}
