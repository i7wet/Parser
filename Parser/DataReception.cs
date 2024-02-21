using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using DbContext.Database.Models;

namespace Parser;

public class DataReception
{
    public static void HttpListen(Data data)
    {
        HttpListener server = new HttpListener();
        server.Prefixes.Add("http://127.0.0.1:3000/");
        server.Start();
        while (true)
        {
            try
            {
                HttpListenerContext ctx = server.GetContext();
                var request = ctx.Request;
                var body = request.InputStream;
                var encoding = request.ContentEncoding;
                var reader = new StreamReader(body, encoding);
                string s = reader.ReadToEnd();
                JsonSerializer.Deserialize<JsonObject>(s).TryGetPropertyValue("Apartment", out var apartJsNode);
                var apart = apartJsNode.Deserialize<ApartmentDb>();
                JsonSerializer.Deserialize<JsonObject>(s).TryGetPropertyValue("Subscriber", out var subJsNode);
                var sub = subJsNode.Deserialize<SubscriberDb>();
                if (apart != null && sub != null)
                    data.AddEntry(sub, apart);
                ctx.Response.StatusCode = 200;
                ctx.Response.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}