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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Lastfm.Scrobbling.Exceptions;

namespace Lastfm.Scrobbling
{
    internal class Request
    {
        private readonly RequestParameters Parameters;
        private Uri URI { get; set; }

        internal Request(Uri uri, RequestParameters parameters)
        {
            URI = uri;
            Parameters = parameters;
        }

        internal async Task<string> Execute()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.AcceptCharset.ParseAdd("utf-8");

            HttpResponseMessage webresponse;
            var output = "FAILED";
            try
            {
                webresponse = client.PostAsync(URI, new StringContent(Parameters.ToString(), Encoding.UTF8, "application/x-www-form-urlencoded")).Result;
                output = await webresponse.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            CheckForErrors(output);

            return output;
        }

        private void CheckForErrors(string output)
        {
            var line = output.Split('\n')[0];

            if (line.StartsWith("BANNED"))
            {
                throw new BannedClientException();
            }
            else if (line.StartsWith("BADAUTH"))
            {
                throw new AuthenticationFailureException();
            }
            else if (line.StartsWith("BADTIME"))
            {
                throw new WrongTimeException();
            }
            else if (line.StartsWith("FAILED") || output.Contains("lfm status=\"failed\""))
            {
                throw new ScrobblingException(output.Substring(output.IndexOf(' ') + 1));
            }
        }
    }
}
