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
using System.Security.Cryptography;
using System.Text;

namespace Lastfm
{
    /// <summary>
    /// General utility functions
    /// </summary>
    public static class Utilities
    {
        internal static string UserAgent => "lastfm-sharp/" + Lib.Version.ToString();

        /// <summary>
        /// Returns the md5 hash of a string.
        /// </summary>
        /// <param name="text">A <see cref="string"/></param>
        /// <returns>A <see cref="string"/></returns>
        public static string MD5(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);

            var crypto = new MD5CryptoServiceProvider();
            buffer = crypto.ComputeHash(buffer);

            var builder = new StringBuilder();
            foreach (byte b in buffer)
            {
                builder.Append(b.ToString("x2").ToLower());
            }

            return builder.ToString();
        }

        public static DateTime TimestampToDateTime(long timestamp, DateTimeKind kind)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, kind).AddSeconds(timestamp).ToLocalTime();
        }

        public static long DateTimeToUTCTimestamp(DateTime dateTime)
        {
            DateTime baseDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            TimeSpan span = dateTime.ToUniversalTime() - baseDate;

            return (long) span.TotalSeconds;
        }
    }
}
