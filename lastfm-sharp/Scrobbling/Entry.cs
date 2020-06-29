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

namespace Lastfm.Scrobbling
{
    public class Entry
    {
        public string Artist { get; set; }
        public string Title { get; set; }
        public string Album { get; set; }
        public TimeSpan Duration { get; set; }
        public int Number { get; set; }
        public string MusicBrainzID { get; set; }
        public DateTime TimeStarted { get; set; }
        public string RecommendationKey { get; set; }
        public PlaybackSource Source { get; set; }
        public ScrobbleMode Mode { get; set; }
        public string MBID { get; set; }

        public Entry(
            string artist,
            string title,
            DateTime timeStarted,
            PlaybackSource source,
            TimeSpan duration,
            ScrobbleMode mode)
        {
            Artist = artist;
            Title = title;
            TimeStarted = timeStarted;
            Source = source;
            Duration = duration;
            Mode = mode;
        }

        public Entry(
            string artist,
            string title,
            DateTime timeStarted,
            PlaybackSource source,
            string recommendationKey,
            TimeSpan duration,
            ScrobbleMode mode)
        {
            Artist = artist;
            Title = title;
            TimeStarted = timeStarted;
            Source = source;
            Duration = duration;
            Mode = mode;
            RecommendationKey = recommendationKey;
        }

        public Entry(
            string artist,
            string title,
            DateTime timeStarted,
            PlaybackSource source,
            TimeSpan duration,
            ScrobbleMode mode,
            string album,
            int trackNumber,
            string mbid)
        {
            Artist = artist;
            Title = title;
            TimeStarted = timeStarted;
            Source = source;
            Duration = duration;
            Mode = mode;
            Album = album;
            Number = trackNumber;
            MBID = mbid;
        }

        public Entry(
            string artist,
            string title,
            string album,
            DateTime timeStarted,
            PlaybackSource source,
            TimeSpan duration,
            ScrobbleMode mode)
        {
            //if (string.IsNullOrWhiteSpace (artist)) {
            //	artist = gMusic.Song.GetArtistFromFileName (title);
            //	title = gMusic.Song.GetTitleFromFileName (title);
            //}
            Artist = artist;
            Title = title;
            TimeStarted = timeStarted;
            Source = source;
            Duration = duration;
            Mode = mode;
            Album = album;
        }

        public Entry(
            string artist,
            string title,
            DateTime timeStarted,
            PlaybackSource source,
            string recommendationKey,
            TimeSpan duration,
            ScrobbleMode mode,
            string album,
            int trackNumber,
            string mbid)
        {
            Artist = artist;
            Title = title;
            TimeStarted = timeStarted;
            Source = source;
            Duration = duration;
            Mode = mode;
            Album = album;
            Number = trackNumber;
            MBID = mbid;
            RecommendationKey = recommendationKey;
        }

        internal RequestParameters getParameters(Session session)
        {
            var parameters = new RequestParameters
            {
                ["api_key"] = session.APIKey,
                ["album"] = Album,
                ["artist"] = Artist,
                ["duration"] = ((int) Duration.TotalSeconds).ToString(),
                ["method"] = "track.scrobble",
                ["sk"] = session.SessionKey,
                ["timestamp"] = Utilities.DateTimeToUTCTimestamp(TimeStarted).ToString(),
                ["track"] = Title
            };

            var api_sig = "";
            foreach (var item in parameters)
            {
                api_sig += item.Key + item.Value;
            }

            api_sig += session.APISecret;

            parameters["api_sig"] = Utilities.MD5(api_sig);
            return parameters;
        }

        public override string ToString()
        {
            return $"{Artist} - {Title} ({TimeStarted})";
        }

        public Services.Track GetInfo(Session session)
        {
            return new Services.Track(Artist, Title, session);
        }
    }
}
