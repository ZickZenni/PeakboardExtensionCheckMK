using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace PeakboardExtensionCheckMK
{
    public class JsonContent : ByteArrayContent
    {
        public JsonContent(string json) : base(Encoding.UTF8.GetBytes(json))
        {
            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }
    }
}
