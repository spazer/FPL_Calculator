using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Text;

namespace WL_Calculator
{
    /// <summary>
    /// Fetches a Web Page
    /// </summary>
    class WebFetch
    {
        public string pageContent;
        private ErrorWriter errorwriter;

        /// <summary>
        /// Feeds a file into Webfetch. To be used for debugging.
        /// </summary>
        /// <param name="reader">file stream</param>
        public WebFetch(StreamReader reader)
        {
            pageContent = reader.ReadToEnd();
            Cleanup();
        }

        /// <summary>
        /// Fetch a page from the internet
        /// </summary>
        /// <param name="page">page URL</param>
        /// <param name="ewriter">Log writer</param>
        public WebFetch(string page, ErrorWriter ewriter)
        {
            errorwriter = ewriter;
            errorwriter.Write("Getting page: " + page);

            try
            {
                pageContent = string.Empty;

                // used to build entire input
                StringBuilder sb = new StringBuilder();

                // used on each read operation
                byte[] buf = new byte[8192];

                // prepare the web page we will be asking for
                HttpWebRequest request = (HttpWebRequest)
                    WebRequest.Create(page);

                // execute the request
                HttpWebResponse response = (HttpWebResponse)
                    request.GetResponse();

                // we will read data via the response stream
                Stream resStream = response.GetResponseStream();

                string tempString = null;
                int count = 0;

                do
                {
                    // fill the buffer with data
                    count = resStream.Read(buf, 0, buf.Length);

                    // make sure we read some data
                    if (count != 0)
                    {
                        // translate from bytes to ASCII text
                        tempString = Encoding.ASCII.GetString(buf, 0, count);

                        // continue building the string
                        sb.Append(tempString);
                    }
                }
                while (count > 0); // any more data to read?

                pageContent = sb.ToString();
                Cleanup();
            }
            catch (System.Net.WebException e)
            {
                errorwriter.Write("Error fetching liquipedia page ( " + e.Message + ")");
            }
        }

        /// <summary>
        /// Clean up some HTML quirks
        /// </summary>
        private void Cleanup()
        {
            pageContent = pageContent.Replace("&lt;", "<");
            pageContent = pageContent.Replace("&amp;", "&");
        }

        /// <summary>
        /// Gets rid of HTML, keeps only the wiki Markup
        /// </summary>
        public void ReduceToWikiMarkup()
        {
            try
            {
                // Find the tags that surround the wiki markup
                int start = pageContent.IndexOf(">", pageContent.IndexOf("<textarea")) + 1;
                int end = pageContent.IndexOf("</textarea");

                // Get all data between those tags
                pageContent = pageContent.Substring(start, end - start);
            }
            catch (System.Net.WebException e)
            {
                errorwriter.Write("Error reducing source to wiki markup ( " + e.Message + ")");
            }
        }
    }
}