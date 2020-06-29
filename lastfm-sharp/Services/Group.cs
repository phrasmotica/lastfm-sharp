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
    /// A Last.fm Group.
    /// </summary>
    public class Group : Base, IEquatable<Group>, IHasWeeklyAlbumCharts, IHasWeeklyArtistCharts, IHasWeeklyTrackCharts, IHasURL
    {
        /// <summary>
        /// Name of the group.
        /// </summary>
        public string Name { get; private set; }

        public Group(string groupName, Session session) : base(session)
        {
            Name = groupName;
        }

        internal override RequestParameters GetParams()
        {
            return new RequestParameters
            {
                ["group"] = Name
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
        /// Returns the latest weekly track chart for this group.
        /// </summary>
        /// <returns>A <see cref="WeeklyTrackChart"/></returns>
        public WeeklyTrackChart GetWeeklyTrackChart()
        {
            XmlDocument doc = Request("group.getWeeklyTrackChart");

            XmlNode n = doc.GetElementsByTagName("weeklytrackchart")[0];

            var nfrom = Utilities.TimestampToDateTime(long.Parse(n.Attributes[1].InnerText), DateTimeKind.Utc);
            var nto = Utilities.TimestampToDateTime(long.Parse(n.Attributes[2].InnerText), DateTimeKind.Utc);

            var chart = new WeeklyTrackChart(new WeeklyChartTimeSpan(nfrom, nto));

            foreach (XmlNode node in doc.GetElementsByTagName("track"))
            {
                var rank = int.Parse(node.Attributes[0].InnerText);
                var playcount = int.Parse(Extract(node, "playcount"));

                var artist = new Track(Extract(node, "artist"), Extract(node, "name"), Session);
                var timeSpan = new WeeklyChartTimeSpan(nfrom, nto);
                var item = new WeeklyTrackChartItem(artist, rank, playcount, timeSpan);

                chart.Add(item);
            }

            return chart;
        }

        /// <summary>
        /// Returns the weekly track chart for this group in the given <see cref="WeeklyChartTimeSpan"/>.
        /// </summary>
        /// <param name="span">A <see cref="WeeklyChartTimeSpan"/></param>
        /// <returns>A <see cref="WeeklyTrackChart"/></returns>
        public WeeklyTrackChart GetWeeklyTrackChart(WeeklyChartTimeSpan span)
        {
            var parameters = GetParams();

            parameters["from"] = Utilities.DateTimeToUTCTimestamp(span.From).ToString();
            parameters["to"] = Utilities.DateTimeToUTCTimestamp(span.To).ToString();

            XmlDocument doc = Request("group.getWeeklyTrackChart", parameters);

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
        /// Returns the latest weekly artist chart for this group.
        /// </summary>
        /// <returns>A <see cref="WeeklyArtistChart"/></returns>
        public WeeklyArtistChart GetWeeklyArtistChart()
        {
            XmlDocument doc = Request("group.getWeeklyArtistChart");

            XmlNode n = doc.GetElementsByTagName("weeklyartistchart")[0];

            var nfrom = Utilities.TimestampToDateTime(long.Parse(n.Attributes[1].InnerText), DateTimeKind.Utc);
            var nto = Utilities.TimestampToDateTime(long.Parse(n.Attributes[2].InnerText), DateTimeKind.Utc);

            var chart = new WeeklyArtistChart(new WeeklyChartTimeSpan(nfrom, nto));

            foreach (XmlNode node in doc.GetElementsByTagName("artist"))
            {
                int rank = int.Parse(node.Attributes[0].InnerText);
                int playcount = int.Parse(Extract(node, "playcount"));

                var artist = new Artist(Extract(node, "name"), Session);
                var timeSpan = new WeeklyChartTimeSpan(nfrom, nto);
                var item = new WeeklyArtistChartItem(artist, rank, playcount, timeSpan);

                chart.Add(item);
            }

            return chart;
        }

        /// <summary>
        /// Returns the weekly artist chart for this group in the given <see cref="WeeklyChartTimeSpan"/>.
        /// </summary>
        /// <param name="span">A <see cref="WeeklyChartTimeSpan"/></param>
        /// <returns>A <see cref="WeeklyArtistChart"/></returns>
        public WeeklyArtistChart GetWeeklyArtistChart(WeeklyChartTimeSpan span)
        {
            var parameters = GetParams();

            parameters["from"] = Utilities.DateTimeToUTCTimestamp(span.From).ToString();
            parameters["to"] = Utilities.DateTimeToUTCTimestamp(span.To).ToString();

            XmlDocument doc = Request("group.getWeeklyArtistChart", parameters);

            XmlNode n = doc.GetElementsByTagName("weeklyartistchart")[0];

            var nfrom = Utilities.TimestampToDateTime(long.Parse(n.Attributes[1].InnerText), DateTimeKind.Utc);
            var nto = Utilities.TimestampToDateTime(long.Parse(n.Attributes[2].InnerText), DateTimeKind.Utc);

            WeeklyArtistChart chart = new WeeklyArtistChart(new WeeklyChartTimeSpan(nfrom, nto));

            foreach (XmlNode node in doc.GetElementsByTagName("artist"))
            {
                int rank = int.Parse(node.Attributes[0].InnerText);
                int playcount = int.Parse(Extract(node, "playcount"));

                var artist = new Artist(Extract(node, "name"), Session);
                var timeSpan = new WeeklyChartTimeSpan(nfrom, nto);
                var item = new WeeklyArtistChartItem(artist, rank, playcount, timeSpan);

                chart.Add(item);
            }

            return chart;
        }

        /// <summary>
        /// Returns the latest weekly album chart for this group.
        /// </summary>
        /// <returns>A <see cref="WeeklyAlbumChart"/></returns>
        public WeeklyAlbumChart GetWeeklyAlbumChart()
        {
            XmlDocument doc = Request("group.getWeeklyAlbumChart");

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
        /// Returns the weekly album chart for this group in the given <see cref="Lastfm.Services.WeeklyChartTimeSpan"/>.
        /// </summary>
        /// <param name="span">A <see cref="WeeklyChartTimeSpan"/></param>
        /// <returns>A <see cref="WeeklyAlbumChart"/></returns>
        public WeeklyAlbumChart GetWeeklyAlbumChart(WeeklyChartTimeSpan span)
        {
            var parameters = GetParams();

            parameters["from"] = Utilities.DateTimeToUTCTimestamp(span.From).ToString();
            parameters["to"] = Utilities.DateTimeToUTCTimestamp(span.To).ToString();

            XmlDocument doc = Request("group.getWeeklyAlbumChart", parameters);

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
        /// Returns the available timespans for charts available for this group.
        /// </summary>
        /// <returns>A <see cref="WeeklyChartTimeSpan"/></returns>
        public WeeklyChartTimeSpan[] GetWeeklyChartTimeSpans()
        {
            XmlDocument doc = Request("group.getWeeklyChartList");

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

        public bool Equals(Group group)
        {
            return group.Name == Name;
        }

        /// <summary>
        /// Returns the Last.fm page of this object.
        /// </summary>
        /// <param name="language">A <see cref="SiteLanguage"/></param>
        /// <returns>A <see cref="string"/></returns>
        public string GetURL(SiteLanguage language)
        {
            var domain = GetSiteDomain(language);

            return $"http://{domain}/group/{UrlSafe(Name)}";
        }

        /// <summary>
        /// The object's Last.fm page url.
        /// </summary>
        public string URL => GetURL(SiteLanguage.English);

        /// <summary>
        /// The members in this group.
        /// </summary>
        public GroupMembers Members => new GroupMembers(this, Session);
    }
}
