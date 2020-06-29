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
using System.Xml;

namespace Lastfm.Services
{
    /// <summary>
    /// A country on Last.fm.
    /// </summary>
    public class Country : Base, IHasURL
    {
        /// <summary>
        /// The country's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// A country by its name or its ISO 3166-1 alpha-2 code.
        /// </summary>
        /// <param name="name">A <see cref="string"/></param>
        /// <param name="session">A <see cref="Session"/></param>
        public Country(string name, Session session)
            : base(session)
        {
            Name = name.Length == 2 ? GetName(name) : name;
        }

        internal override RequestParameters GetParams()
        {
            return new RequestParameters
            {
                ["country"] = Name
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

        private string GetName(string alpha2code)
        {
            var codes = new Dictionary<string, string>
            {
                { "AD", "Andorra" },
                { "AE", "United Arab Emirates" },
                { "AF", "Afghanistan" },
                { "AG", "Antigua and Barbuda" },
                { "AI", "Anguilla" },
                { "AL", "Albania" },
                { "AM", "Armenia" },
                { "AN", "Netherlands Antilles" },
                { "AO", "Angola" },
                { "AQ", "Antarctica" },
                { "AR", "Argentina" },
                { "AS", "American Samoa" },
                { "AT", "Austria" },
                { "AU", "Australia" },
                { "AW", "Aruba" },
                { "AX", "Åland Islands" },
                { "AZ", "Azerbaijan" },
                { "BA", "Bosnia and Herzegovina" },
                { "BB", "Barbados" },
                { "BD", "Bangladesh" },
                { "BE", "Belgium" },
                { "BF", "Burkina Faso" },
                { "BG", "Bulgaria" },
                { "BH", "Bahrain" },
                { "BI", "Burundi" },
                { "BJ", "Benin" },
                { "BM", "Bermuda" },
                { "BN", "Brunei Darussalam" },
                { "BO", "Bolivia" },
                { "BR", "Brazil" },
                { "BS", "Bahamas" },
                { "BT", "Bhutan" },
                { "BV", "Bouvet Island" },
                { "BW", "Botswana" },
                { "BY", "Belarus" },
                { "BZ", "Belize" },
                { "CA", "Canada" },
                { "CC", "Cocos (Keeling) Islands" },
                { "CD", "Congo, Democratic Republic of the" },
                { "CF", "Central African Republic" },
                { "CG", "Congo" },
                { "CH", "Switzerland" },
                { "CI", "Côte d'Ivoire" },
                { "CK", "Cook Islands" },
                { "CL", "Chile" },
                { "CM", "Cameroon" },
                { "CN", "China" },
                { "CO", "Colombia" },
                { "CR", "Costa Rica" },
                { "CS", "Serbia" },
                { "CU", "Cuba" },
                { "CV", "Cape Verde" },
                { "CX", "Christmas Island" },
                { "CY", "Cyprus" },
                { "CZ", "Czech Republic" },
                { "DE", "Germany" },
                { "DJ", "Djibouti" },
                { "DK", "Denmark" },
                { "DM", "Dominica" },
                { "DO", "Dominican Republic" },
                { "DZ", "Algeria" },
                { "EC", "Ecuador" },
                { "EE", "Estonia" },
                { "EG", "Egypt" },
                { "EH", "Western Sahara" },
                { "ER", "Eritrea" },
                { "ES", "Spain" },
                { "ET", "Ethiopia" },
                { "FI", "Finland" },
                { "FJ", "Fiji" },
                { "FK", "Falkland Islands" },
                { "FM", "Micronesia, Federated States of" },
                { "FO", "Faroe Islands" },
                { "FR", "France" },
                { "GA", "Gabon" },
                { "GB", "United Kingdom" },
                { "GD", "Grenada" },
                { "GE", "Georgia" },
                { "GF", "French Guiana" },
                { "GH", "Ghana" },
                { "GI", "Gibraltar" },
                { "GL", "Greenland" },
                { "GM", "Gambia" },
                { "GN", "Guinea" },
                { "GP", "Guadeloupe" },
                { "GQ", "Equatorial Guinea" },
                { "GR", "Greece" },
                { "GS", "South Georgia and the South Sandwich Islands" },
                { "GT", "Guatemala" },
                { "GU", "Guam" },
                { "GW", "Guinea-Bissau" },
                { "GY", "Guyana" },
                { "HK", "Hong Kong" },
                { "HM", "Heard Island and McDonald Islands" },
                { "HN", "Honduras" },
                { "HR", "Croatia" },
                { "HT", "Haiti" },
                { "HU", "Hungary" },
                { "ID", "Indonesia" },
                { "IE", "Ireland" },
                { "IL", "Israel" },
                { "IN", "India" },
                { "IO", "British Indian Ocean Territory" },
                { "IQ", "Iraq" },
                { "IR", "Iran, Islamic Republic of" },
                { "IS", "Iceland" },
                { "IT", "Italy" },
                { "JM", "Jamaica" },
                { "JO", "Jordan" },
                { "JP", "Japan" },
                { "KE", "Kenya" },
                { "KG", "Kyrgyzstan" },
                { "KH", "Cambodia" },
                { "KI", "Kiribati" },
                { "KM", "Comoros" },
                { "KN", "Saint Kitts and Nevis" },
                { "KP", "Korea, Democratic People's Republic of" },
                { "KR", "Korea, Republic of" },
                { "KW", "Kuwait" },
                { "KY", "Cayman Islands" },
                { "KZ", "Kazakhstan" },
                { "LA", "Lao People's Democratic Republic" },
                { "LB", "Lebanon" },
                { "LC", "Saint Lucia" },
                { "LI", "Liechtenstein" },
                { "LK", "Sri Lanka" },
                { "LR", "Liberia" },
                { "LS", "Lesotho" },
                { "LT", "Lithuania" },
                { "LU", "Luxembourg" },
                { "LV", "Latvia" },
                { "LY", "Libyan Arab Jamahiriya (Libya)" },
                { "MA", "Morocco" },
                { "MC", "Monaco" },
                { "MD", "Moldova" },
                { "MG", "Madagascar" },
                { "MH", "Marshall Islands" },
                { "MK", "Macedonia" },
                { "ML", "Mali" },
                { "MM", "Myanmar (Burma)" },
                { "MN", "Mongolia" },
                { "MO", "Macao (Macau)" },
                { "MP", "Northern Mariana Islands" },
                { "MQ", "Martinique" },
                { "MR", "Mauritania" },
                { "MS", "Montserrat" },
                { "MT", "Malta" },
                { "MU", "Mauritius" },
                { "MV", "Maldives" },
                { "MW", "Malawi" },
                { "MX", "Mexico" },
                { "MY", "Malaysia" },
                { "MZ", "Mozambique" },
                { "NA", "Namibia" },
                { "NC", "New Caledonia" },
                { "NE", "Niger" },
                { "NF", "Norfolk Island" },
                { "NG", "Nigeria" },
                { "NI", "Nicaragua" },
                { "NL", "Netherlands" },
                { "NO", "Norway" },
                { "NP", "Nepal" },
                { "NR", "Nauru" },
                { "NU", "Niue" },
                { "NZ", "New Zealand" },
                { "OM", "Oman" },
                { "PA", "Panama" },
                { "PE", "Peru" },
                { "PF", "French Polynesia" },
                { "PG", "Papua New Guinea" },
                { "PH", "Philippines" },
                { "PK", "Pakistan" },
                { "PL", "Poland" },
                { "PM", "Saint-Pierre and Miquelon" },
                { "PN", "Pitcairn Islands" },
                { "PR", "Puerto Rico" },
                { "PS", "Palestinian Territory, Occupied" },
                { "PT", "Portugal" },
                { "PW", "Palau" },
                { "PY", "Paraguay" },
                { "QA", "Qatar" },
                { "RE", "Réunion" },
                { "RO", "Romania" },
                { "RU", "Russia" },
                { "RW", "Rwanda" },
                { "SA", "Saudi Arabia" },
                { "SB", "Solomon Islands" },
                { "SC", "Seychelles" },
                { "SD", "Sudan" },
                { "SE", "Sweden" },
                { "SG", "Singapore" },
                { "SH", "Saint Helena" },
                { "SI", "Slovenia" },
                { "SJ", "Svalbard and Jan Mayen Islands" },
                { "SK", "Slovakia" },
                { "SL", "Sierra Leone" },
                { "SM", "San Marino" },
                { "SN", "Senegal" },
                { "SO", "Somalia" },
                { "SR", "Suriname" },
                { "ST", "São Tomé and Príncipe" },
                { "SV", "El Salvador" },
                { "SY", "Syrian Arab Republic" },
                { "SZ", "Swaziland" },
                { "TC", "Turks and Caicos Islands" },
                { "TD", "Chad" },
                { "TF", "French Southern Territories" },
                { "TG", "Togo" },
                { "TH", "Thailand" },
                { "TJ", "Tajikistan" },
                { "TK", "Tokelau" },
                { "TL", "Timor-Leste" },
                { "TM", "Turkmenistan" },
                { "TN", "Tunisia" },
                { "TO", "Tonga" },
                { "TR", "Turkey" },
                { "TT", "Trinidad and Tobago" },
                { "TV", "Tuvalu" },
                { "TW", "Taiwan" },
                { "TZ", "Tanzania, United Republic of" },
                { "UA", "Ukraine" },
                { "UG", "Uganda" },
                { "UM", "United States Minor Outlying Islands" },
                { "US", "United States" },
                { "UY", "Uruguay" },
                { "UZ", "Uzbekistan" },
                { "VA", "Holy See" },
                { "VC", "Saint Vincent and the Grenadines" },
                { "VE", "Venezuela" },
                { "VG", "Virgin Islands, British" },
                { "VI", "Virgin Islands, U.S." },
                { "VN", "Viet Nam" },
                { "VU", "Vanuatu" },
                { "WF", "Wallis and Futuna" },
                { "WS", "Samoa" },
                { "YE", "Yemen" },
                { "YT", "Mayotte" },
                { "ZA", "South Africa" },
                { "ZM", "Zambia" },
                { "ZW", "Zimbabwe" }
            };

            return codes[alpha2code];
        }

        public TopArtist[] GetTopArtists()
        {
            XmlDocument doc = Request("geo.getTopArtists");

            var list = new List<TopArtist>();
            foreach (XmlNode node in doc.GetElementsByTagName("artist"))
            {
                var artist = new Artist(Extract(node, "name"), Session);
                var playCount = int.Parse(Extract(node, "playcount"));

                list.Add(new TopArtist(artist, playCount));
            }

            return list.ToArray();
        }

        public TopArtist[] GetTopArtists(int limit)
        {
            return Sublist(GetTopArtists(), limit);
        }

        public TopTrack[] GetTopTracks()
        {
            XmlDocument doc = Request("geo.getTopTracks");

            var list = new List<TopTrack>();
            foreach (XmlNode node in doc.GetElementsByTagName("track"))
            {
                var track = new Track(Extract(node, "name", 1), Extract(node, "name"), Session);
                int playcount = int.Parse(Extract(node, "playcount"));

                list.Add(new TopTrack(track, playcount));
            }

            return list.ToArray();
        }

        public TopTrack[] GetTopTracks(int limit)
        {
            return Sublist(GetTopTracks(), limit);
        }

        /// <summary>
        /// Returns the country's page url on Last.fm.
        /// </summary>
        /// <param name="language">A <see cref="SiteLanguage"/></param>
        /// <returns>A <see cref="System.String"/></returns>
        public string GetURL(SiteLanguage language)
        {
            var domain = GetSiteDomain(language);

            return $"http://{domain}/place/{UrlSafe(Name)}";
        }

        /// <summary>The country's page url on Last.fm.</summary>
        public string URL => GetURL(SiteLanguage.English);
    }
}
