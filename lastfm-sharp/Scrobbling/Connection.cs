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
using System.Threading.Tasks;
using Lastfm.Scrobbling.Exceptions;

namespace Lastfm.Scrobbling
{
    /// <summary>
    /// A connection to the Last.fm scrobbling service. Can be used individually for scrobbling or
    /// through a <see cref="ScrobbleManager"/> object.
    /// </summary>
    public class Connection
    {
        public string ClientID { get; private set; }

        private string SessionID { get; set; }
        private Uri SubmissionURL { get; set; }
        private Uri NowplayingURL { get; set; }

        private RequestParameters handshakeParameters;
        private bool firstHandshakeDone { get; set; }
        public Session session;

        public Connection(Session authenticatedSession)
        {
            session = authenticatedSession;
        }

        /// <summary>
        /// Send the now playing notification.
        /// </summary>
        /// <param name="track">A <see cref="NowplayingTrack"/></param>
        public async Task ReportNowplayingAsync(NowplayingTrack track)
        {
            var parameters = new RequestParameters
            {
                ["api_key"] = session.APIKey,
                ["album"] = track.Album,
                ["artist"] = track.Artist,
                ["duration"] = ((int) track.Duration.TotalSeconds).ToString(),
                ["method"] = "track.updateNowPlaying",
                ["sk"] = session.SessionKey,
                ["track"] = track.Title
            };

            var api_sig = "";
            foreach (var item in parameters)
            {
                api_sig += item.Key + item.Value;
            }

            api_sig += session.APISecret;

            parameters["api_sig"] = Utilities.MD5(api_sig);
            Request request = new Request(new Uri("http://ws.audioscrobbler.com/2.0/"), parameters);

            // A BadSessionException occurs when another client has made a handshake with this
            // user's credentials, should redo a handshake and pass this exception quietly.
            try
            {
                await request.Execute();
            }
            catch (BadSessionException)
            {
                //this.doHandshake();
                await ReportNowplayingAsync(track);
            }
        }

        /// <summary>
        /// Public scrobble function. Scrobbles a PlayedTrack object.
        /// </summary>
        /// <param name="track">A <see cref="PlayedTrack"/></param>
        public async Task Scrobble(Entry track)
        {
            var parameters = track.getParameters(session);

            // This scrobbles the collection of parameters no matter what they belong to.
            await Scrobble(parameters);
        }

        /// <summary>
        /// The internal scrobble function, scrobbles pure request parameters. Could be for more
        /// than one track, as specified by Last.fm, but they recommend that only one track should
        /// be submitted at a time.
        /// </summary>
        /// <param name="parameters">A <see cref="RequestParameters"/></param>
        internal async Task Scrobble(RequestParameters parameters)
        {
            //Initialize();

            var request = new Request(new Uri("http://ws.audioscrobbler.com/2.0/"), parameters);

            // A BadSessionException occurs when another client has made a handshake with this
            // user's credentials, should redo a handshake and pass this exception quietly.
            try
            {
                await request.Execute();
            }
            catch (BadSessionException)
            {
                //this.doHandshake();
                await Scrobble(parameters);
            }
        }
    }
}
