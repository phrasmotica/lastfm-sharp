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
    /// A Last.fm event.
    /// </summary>
    public class Event : Base, IEquatable<Event>, IShareable, IHasImage, IHasURL
    {
        /// <summary>
        /// The event ID.
        /// </summary>
        public int ID { get; private set; }

        public Event(int id, Session session) : base(session)
        {
            ID = id;
        }

        internal override RequestParameters GetParams()
        {
            var parameters = new RequestParameters
            {
                ["event"] = ID.ToString()
            };

            return parameters;
        }

        /// <summary>
        /// Set the authenticated user's status for this event.
        /// </summary>
        /// <param name="attendance">A <see cref="EventAttendance"/></param>
        public void SetAttendance(EventAttendance attendance)
        {
            RequireAuthentication();

            var parameters = GetParams();

            var i = (int) attendance;
            parameters["status"] = i.ToString();

            Request("event.attend", parameters);
        }

        /// <summary>
        /// Returns the title of the event.
        /// </summary>
        /// <returns>A <see cref="string"/></returns>
        public string GetTitle()
        {
            XmlDocument doc = Request("event.getInfo");

            return Extract(doc, "title");
        }

        /// <summary>
        /// Returns the participating artists in this event.
        /// </summary>
        /// <returns>A <see cref="Artist"/></returns>
        public Artist[] GetArtists()
        {
            XmlDocument doc = Request("event.getInfo");

            var list = new List<Artist>();
            foreach (var name in ExtractAll(doc, "artist"))
            {
                list.Add(new Artist(name, Session));
            }

            return list.ToArray();
        }

        /// <summary>
        /// Returns the headliner artist.
        /// </summary>
        /// <returns>A <see cref="Artist"/></returns>
        public Artist GetHeadliner()
        {
            XmlDocument doc = Request("event.getInfo");

            return new Artist(Extract(doc, "headliner"), Session);
        }

        /// <summary>
        /// Returns the start time of the event.
        /// </summary>
        /// <returns>A <see cref="DateTime"/></returns>
        public DateTime GetStartDate()
        {
            XmlDocument doc = Request("event.getInfo");

            return DateTime.Parse(Extract(doc, "startDate"));
        }

        /// <summary>
        /// Returns the description of the evnet.
        /// </summary>
        /// <returns>A <see cref="string"/></returns>
        public string GetDescription()
        {
            XmlDocument doc = Request("event.getInfo");

            return Extract(doc, "description");
        }

        /// <summary>
        /// Returns the url to the image of this event.
        /// </summary>
        /// <param name="size">A <see cref="ImageSize"/></param>
        /// <returns>A <see cref="string"/></returns>
        public string GetImageURL(ImageSize size)
        {
            XmlDocument doc = Request("event.getInfo");

            return ExtractAll(doc, "image")[(int) size];
        }

        /// <summary>
        /// Returns the url to the image of this event.
        /// </summary>
        /// <returns>A <see cref="string"/></returns>
        public string GetImageURL()
        {
            return GetImageURL(ImageSize.Large);
        }

        /// <summary>
        /// Returns the number of attendees.
        /// </summary>
        /// <returns>A <see cref="int"/></returns>
        public int GetAttendantCount()
        {
            XmlDocument doc = Request("event.getInfo");

            return int.Parse(Extract(doc, "attendance"));
        }

        /// <summary>
        /// Returns the number of reviews for this event.
        /// </summary>
        /// <returns>A <see cref="int"/></returns>
        public int GetReviewCount()
        {
            XmlDocument doc = Request("event.getInfo");

            return int.Parse(Extract(doc, "reviews"));
        }

        /// <summary>
        /// Returns the flickr tag.
        /// </summary>
        /// <remarks>
        /// You can tag your image on flickr with this tag and they would be imported into the
        /// last.fm page for this event.
        /// </remarks>
        /// <returns>A <see cref="string"/></returns>
        public string GetFlickrTag()
        {
            XmlDocument doc = Request("event.getInfo");

            return Extract(doc, "tag");
        }

        /// <summary>
        /// Check to see if this object equals another.
        /// </summary>
        /// <param name="eventObject">A <see cref="Event"/></param>
        /// <returns>A <see cref="bool"/></returns>
        public bool Equals(Event eventObject)
        {
            return eventObject.ToString() == ToString();
        }

        /// <summary>
        /// Share this event with others.
        /// </summary>
        /// <param name="recipients">A <see cref="Recipients"/></param>
        /// <param name="message">A <see cref="System.String"/></param>
        public void Share(Recipients recipients, string message)
        {
            if (recipients.Count > 1)
            {
                foreach (string recipient in recipients)
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

            Request("event.Share", parameters);
        }

        /// <summary>
        /// Share this event with others.
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

            Request("event.Share", parameters);
        }

        /// <summary>
        /// Returns the Last.fm page of this object.
        /// </summary>
        /// <param name="language">A <see cref="SiteLanguage"/></param>
        /// <returns>A <see cref="string"/></returns>
        public string GetURL(SiteLanguage language)
        {
            var domain = GetSiteDomain(language);

            return $"http://{domain}/event/{ID.ToString()}";
        }

        /// <summary>
        /// The object's Last.fm page url.
        /// </summary>
        public string URL => GetURL(SiteLanguage.English);

        /// <summary>
        /// The venue where the event is being held.
        /// </summary>
        public Venue Venue
        {
            get
            {
                XmlDocument doc = Request("event.getInfo");

                string url = ((XmlElement) doc.GetElementsByTagName("venue")[0]).GetElementsByTagName("url")[0].InnerText;
                int id = int.Parse(url.Substring(url.LastIndexOf('/') + 1));

                return new Venue(id, Session);
            }
        }
    }
}
