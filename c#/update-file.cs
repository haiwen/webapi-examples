using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Linq;
using System.Net;

//  List<KeyValuePair<string, string>> formData = null,         

namespace hwapp
{
         
    class Program
    {
        enum UploadState { PendingUpload, Uploading, PendingResponse }
        public static string HttpGet(string url, Encoding encoding = null)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Token d206fa23813ed737eec26fe78c0291feac8f1bbe");
            var t = httpClient.GetByteArrayAsync(url);
            t.Wait();
            var ret = encoding.GetString(t.Result);
            return ret;
        }
        static async Task<string> HttpPostAsync(string url)
        {
            string boundary = "Upload---------" + Guid.NewGuid().ToString();
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Accept", "application/json; charset=utf-8; indent=4");
            request.Headers.Add("Authorization", "Token d206fa23813ed737eec26fe78c0291feac8f1bbe");

            var content = new MultipartFormDataContentEx(boundary);

            string filepath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "test.txt");
            FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read);
            var fileContent = new ProgressableStreamContent(fs);

            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            fileContent.Headers.ContentType.CharSet = "\'utf8\'";
            
            String headerValue = "form-data; name=\"file\"; filename=\"test.txt\"";
            byte[] bytes = Encoding.UTF8.GetBytes(headerValue);
            headerValue = "";
            foreach (byte b in bytes)
            {
                headerValue += (Char)b;
            }
            fileContent.Headers.Add("Content-Disposition", headerValue);
            content.Add(fileContent);

            string tDir = "/test.txt";
            var dirContent = new StringContent(tDir, Encoding.UTF8);
            dirContent.Headers.ContentType = null;
            dirContent.Headers.TryAddWithoutValidation("Content-Disposition", @"form-data; name=""target_file""");
            

            content.Add(dirContent);

            // transmit the content length, for this we use the private method TryComputeLength() called by reflection
            long conLen;

            if (!content.ComputeLength(out conLen))
                conLen = 0;

            // the seafile-server implementation rejects the content-type if the boundary value is
            // placed inside quotes which is what HttpClient does, so we have to redefine the content-type without using quotes
            // and remove the actual content-type which uses quotes beforehand
            content.Headers.ContentType = null;
            content.Headers.ContentLength = null;
            content.Headers.TryAddWithoutValidation("Content-Type", "multipart/form-data; boundary=" + boundary);
            //content.Headers.ContentType.CharSet = "UTF-8";
            //client.DefaultRequestHeaders.TransferEncodingChunked = true;                
            if (conLen > 0)
            {
                // in order to disable buffering
                // and make the progress work
                content.Headers.Add("Content-Length", conLen.ToString());
            }

            request.Content = content;
            HttpResponseMessage response;
            HttpClientHandler handler = new HttpClientHandler();
            handler.AllowAutoRedirect = false;
            using (HttpClient client = new HttpClient(handler))   
            response = await client.SendAsync(request,new CancellationTokenSource().Token);  

            return await response.Content.ReadAsStringAsync();;
        }

        static async void ExecPostAsync(string url)
        {
            string res = await HttpPostAsync(url);
        }

        class MultipartFormDataContentEx : MultipartFormDataContent
        {
            public MultipartFormDataContentEx(String boundary)
                : base(boundary)
            {
                // --
            }

            public bool ComputeLength(out long length)
            {
                return base.TryComputeLength(out length);
            }
        }
    
        class ProgressableStreamContent : HttpContent
        {
            private const int defaultBufferSize = 4096;

            private Stream content;
            private int bufferSize;
            private bool contentConsumed;

            public UploadState State { get; set; }
            long UploadedBytes = 0;

            public ProgressableStreamContent(Stream content) : this(content, defaultBufferSize) { }

            public ProgressableStreamContent(Stream content, int bufferSize)
            {
                if (content == null)
                {
                    throw new ArgumentNullException("content");
                }
                if (bufferSize <= 0)
                {
                    throw new ArgumentOutOfRangeException("bufferSize");
                }

                State = UploadState.PendingUpload;

                this.content = content;
                this.bufferSize = bufferSize;
            }

            protected async override Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {            
                PrepareContent();

                State = UploadState.PendingUpload;

                var buffer = new Byte[this.bufferSize];
                var size = content.Length;
                long uploaded = 0;

                State = UploadState.PendingUpload;

                using (content) while (true)
                    {
                        var length = content.Read(buffer, 0, buffer.Length);
                        if (length <= 0) break;

                        UploadedBytes = uploaded += length;                

                        State = UploadState.Uploading;
                        await stream.WriteAsync(buffer, 0, length);                   
                    }

                State = UploadState.PendingResponse;
            }

            public long ComputeLength()
            {
                long length;
                if (TryComputeLength(out length))
                    return length;
                else
                    return -1;
            }

            protected override bool TryComputeLength(out long length)
            {
                length = content.Length;
                return true;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    content.Dispose();
                }
                base.Dispose(disposing);
            }


            private void PrepareContent()
            {
                if (contentConsumed)
                {
                    // If the content needs to be written to a target stream a 2nd time, then the stream must support
                    // seeking (e.g. a FileStream), otherwise the stream can't be copied a second time to a target 
                    // stream (e.g. a NetworkStream).
                    if (content.CanSeek)
                    {
                        content.Position = 0;
                    }
                    else
                    {
                        throw new InvalidOperationException("SR.net_http_content_stream_already_read");
                    }
                }
                contentConsumed = true;
            }
        }

        static void Main(string[] args)
        {
            var downlink =  hwapp.Program.HttpGet("http://172.16.3.130:8000/api2/repos/40757770-a0ad-4238-bf93-27afe1eb885d/update-link/", new System.Text.UTF8Encoding());
            ExecPostAsync(downlink.Replace("\"", ""));
            Thread.Sleep(100);
        }
    }
}