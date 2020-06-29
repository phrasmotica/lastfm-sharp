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

using System.Collections.Generic;
using System.Web;
using System.Xml;

namespace Lastfm.Services
{
    /// <summary>
    /// A base class for most of the objects.
    /// </summary>
    public abstract class Base
    {
        protected Session Session { get; set; }

        public Base(Session session)
        {
            Session = session;
        }

        internal abstract RequestParameters GetParams();

        internal XmlDocument Request(string methodName, RequestParameters parameters)
        {
            return new Request(methodName, Session, parameters).Execute();
        }

        internal XmlDocument Request(string methodName)
        {
            return new Request(methodName, Session, GetParams()).Execute();
        }

        internal string Extract(XmlNode node, string name, int index)
        {
            return ((XmlElement) node).GetElementsByTagName(name)[index].InnerText;
        }

        internal string Extract(XmlNode node, string name)
        {
            return Extract((XmlElement) node, name, 0);
        }

        internal string Extract(XmlDocument document, string name)
        {
            return Extract(document.DocumentElement, name);
        }

        internal string Extract(XmlDocument document, string name, int index)
        {
            return Extract(document.DocumentElement, name, index);
        }

        internal string[] ExtractAll(XmlNode node, string name, int limitCount)
        {
            var s = ExtractAll(node, name);
            var l = new List<string>();

            for (int i = 0; i < limitCount; i++)
            {
                l.Add(s[i]);
            }

            return l.ToArray();
        }

        internal string[] ExtractAll(XmlNode node, string name)
        {
            var list = new List<string>();

            for (int i = 0; i < ((XmlElement) node).GetElementsByTagName(name).Count; i++)
            {
                list.Add(Extract(node, name, i));
            }

            return list.ToArray();
        }

        internal string[] ExtractAll(XmlDocument document, string name)
        {
            return ExtractAll(document.DocumentElement, name);
        }

        internal string[] ExtractAll(XmlDocument document, string name, int limitCount)
        {
            return ExtractAll(document.DocumentElement, name, limitCount);
        }

        internal void RequireAuthentication()
        {
            if (!Session.Authenticated)
            {
                throw new AuthenticationRequiredException();
            }
        }

        internal T[] Sublist<T>(T[] original, int length)
        {
            var list = new List<T>();

            for (int i = 0; i < length; i++)
            {
                list.Add(original[i]);
            }

            return list.ToArray();
        }

        internal string UrlSafe(string text)
        {
            return HttpUtility.UrlEncode(HttpUtility.UrlEncode(text));
        }

        internal string getPeriod(Period period)
        {
            var values = new string[] { "overall", "3month", "6month", "12month" };

            return values[(int) period];
        }

        internal string GetSiteDomain(SiteLanguage language)
        {
            var domains = new Dictionary<SiteLanguage, string>
            {
                { SiteLanguage.English, "www.last.fm" },
                { SiteLanguage.German, "www.lastfm.de" },
                { SiteLanguage.Spanish, "www.lastfm.es" },
                { SiteLanguage.French, "www.lastfm.fr" },
                { SiteLanguage.Italian, "www.lastfm.it" },
                { SiteLanguage.Polish, "www.lastfm.pl" },
                { SiteLanguage.Portuguese, "www.lastfm.com.br" },
                { SiteLanguage.Swedish, "www.lastfm.se" },
                { SiteLanguage.Turkish, "www.lastfm.com.tr" },
                { SiteLanguage.Russian, "www.lastfm.ru" },
                { SiteLanguage.Japanese, "www.lastfm.jp" },
                { SiteLanguage.Chinese, "cn.last.fm" }
            };

            return domains[language];
        }
    }
}
