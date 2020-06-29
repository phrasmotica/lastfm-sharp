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
using System.Text;
using System.Web;

namespace Lastfm
{
    public class RequestParameters : SortedDictionary<string, string>
    {
        public RequestParameters() { }

        public RequestParameters(string serialization)
        {
            string[] values = serialization.Split('\t');

            for (var i = 0; i < values.Length - 1; i++)
            {
                if (i % 2 == 0)
                {
                    this[values[i]] = values[i + 1];
                }
            }
        }

        public byte[] ToBytes()
        {
            return Encoding.ASCII.GetBytes(ToString());
        }

        public string Serialize()
        {
            var line = "";

            foreach (string key in Keys)
            {
                line += key + "\t" + this[key] + "\t";
            }

            return line;
        }

        public override string ToString()
        {
            var values = "";

            foreach (string key in Keys)
            {
                values += HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(this[key]) + "&";
            }

            values = values[0..^1];

            return values;
        }
    }
}
