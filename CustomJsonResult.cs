using System;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using GenuinaBI.Configuration;
public class CustomJsonResult : JsonResult
{
    public override void ExecuteResult(ControllerContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        HttpResponseBase response = context.HttpContext.Response;

        if (!String.IsNullOrEmpty(ContentType))
        {
            response.ContentType = ContentType;
        }
        else
        {
            response.ContentType = "application/json";
        }
        if (ContentEncoding != null)
        {
            response.ContentEncoding = ContentEncoding;
        }
        if (Data != null)
        {
            // Using Json.NET serializer
            var isoConvert = new IsoDateTimeConverter();
            isoConvert.DateTimeFormat = Config.CasinoDateTimeFormat;
            response.Write(JsonConvert.SerializeObject(Data, isoConvert));
        }
    }
}