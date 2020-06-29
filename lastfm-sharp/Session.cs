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
using System.Xml;
using Lastfm.Services;

namespace Lastfm
{
    /// <summary>
    /// Represents your identity tokens provided by Last.fm.
    /// </summary>
    /// <remarks>
    /// A session with only API Key and API Secret is not authenticated, and it wouldn't let you
    /// perform write operations on Last.fm. In order to have it authenticated you could do the following:
    /// <code>
    ///using System;
    ///using Lastfm.Services;
    ///
    ///string key = "b25b959554ed76058ac220b7b2e0a026";
    ///string secret = "361505f8eeaf61426ef95a4317482251";
    ///
    ///Session session = new Session(key, secret);
    ///
    ///// one way is to ask the user for his username and password.
    ///string username = Console.ReadLine("Please enter your username: ");
    ///string md5Password = Utilities.md5(Console.ReadLine("Please enter your password: "));
    ///
    ///// then authenticate.
    ///session.Authenticate(username, md5Password);
    ///
    ///// another way is to let the user authenticate from the Last.fm website.
    ///string url = session.GetAuthenticationURL();
    ///Console.WriteLine("Please open the following url and follow the procedures, then press Enter: " + url);
    ///
    ///// wait for it.
    ///Console.ReadLine();
    ///
    ///// now that he's done, retreive the session key.
    ///session.AuthenticateViaWeb();
    /// </code>
    /// Please note that a Session object is inhertied among objects. For example if you create an
    /// <see cref="Lastfm.Artist"/> object with an authenticated session and used the artists
    /// returned by <see cref="Artist.GetSimilar"/> they would also have an unauthenticated session.
    /// </remarks>

    [Serializable]
    public class Session : IEquatable<Session>
    {
        /// <summary>
        /// API Key
        /// </summary>
        /// <summary>The Last.fm API Key.</summary>
        /// <remarks>To acquire one, please see http://www.last.fm/api/account</remarks>
        public string APIKey { get; set; }

        /// <summary>
        /// API Secret
        /// </summary>
        /// <summary>The Last.fm API Secret.</summary>
        /// <remarks>To acquire one, please see http://www.last.fm/api/account</remarks>
        public string APISecret { get; set; }

        /// <summary>
        /// Session Key
        /// </summary>
        /// <summary>
        /// The Session key which represents the user's permission to let you perform "write"
        /// operations on his/her profile.
        /// </summary>
        /// <remarks>
        /// To set this value, you have to call either <see cref="Authenticate"/> or <see
        /// cref="GetWebAuthenticationURL"/> and let the user authenticate by theirselves
        /// then call <see cref="AuthenticateViaWeb"/> to complete the process.
        /// </remarks>
        public string SessionKey { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// Returns true if the session is authenticated.
        /// </summary>
        public bool Authenticated => SessionKey != null;

        private string Token { get; set; }

        public Session(string apiKey, string apiSecret)
        {
            APIKey = apiKey;
            APISecret = apiSecret;
        }

        public Session(string apiKey, string apiSecret, string sessionKey) : this(apiKey, apiSecret)
        {
            SessionKey = sessionKey;
        }

        public Task AuthenticateAsync(string username, string md5Password)
        {
            return Task.Run(() => Authenticate(username, md5Password));
        }

        /// <summary>
        /// Authenticate this session using a username and a md5 hash of the password.
        /// </summary>
        /// <param name="username">A <see cref="string"/></param>
        /// <param name="md5Password">A <see cref="string"/></param>
        public void Authenticate(string username, string md5Password)
        {
            Password = md5Password;
            UserName = username;
            var parameters = new RequestParameters
            {
                ["username"] = username,
                ["authToken"] = Utilities.MD5(username + md5Password)
            };

            var request = new Request("auth.getMobileSession", this, parameters);
            request.SignIt();

            XmlDocument doc = request.Execute();

            SessionKey = doc.GetElementsByTagName("key")[0].InnerText;
        }

        private string GetAuthenticationToken()
        {
            XmlDocument doc = new Request("auth.getToken", this, new RequestParameters()).Execute();

            return doc.GetElementsByTagName("token")[0].InnerText;
        }

        /// <summary>
        /// Returns the url for web authentication.
        /// </summary>
        /// <remarks>
        /// <see cref="AuthenticateViaWeb"/> should be called when the user is done.
        /// </remarks>
        /// <returns>A <see cref="string"/></returns>
        public string GetWebAuthenticationURL()
        {
            Token = GetAuthenticationToken();

            return "http://www.last.fm/api/auth/?api_key=" + APIKey + "&token=" + Token;
        }

        /// <summary>
        /// Complete the web authentication.
        /// </summary>
        public void AuthenticateViaWeb()
        {
            var p = new RequestParameters
            {
                ["token"] = Token
            };

            Request r = new Request("auth.getSession", this, p);
            r.SignIt();

            XmlDocument doc = r.Execute();

            SessionKey = doc.GetElementsByTagName("key")[0].InnerText;
        }

        /// <summary>
        /// Check to see if this object equals another.
        /// </summary>
        /// <param name="session">A <see cref="Session"/></param>
        /// <returns>A <see cref="bool"/></returns>
        public bool Equals(Session session)
        {
            return session.APIKey == APIKey &&
                    session.APISecret == APISecret &&
                    session.SessionKey == SessionKey;
        }
    }
}
