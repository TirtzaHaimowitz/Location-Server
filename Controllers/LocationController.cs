using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Xml;
using static LocationAPI.LocationEntities2;
namespace LocationAPI.Controllers
{
    public class LocationController : ApiController
    {

        public JObject getDistance(string source, string destination)
        {
           // using (LocationEntities2 db = new LocationEntities2())
            {
                LocationEntities2 db = new LocationEntities2();
                Location loc = db.Location.FirstOrDefault(l => l.Target == source && l.Destination == destination);
                if (loc == null)
                   loc = getDistanceFromAPI(source, destination);
                else
                {
                    loc.counter += 1;
                    db.SaveChanges();
                }
                
                JObject jObject = new JObject();
                jObject["Distance"] = loc.Distance;
                jObject["Counter"] = loc.counter;
                return (jObject);


            }
        }

        private Location getDistanceFromAPI(string source, string destination)
        {
            string url = @"https://maps.googleapis.com/maps/api/distancematrix/xml?origins=" +
            source + "&destinations=" + destination + "&key=AIzaSyAwLbz_DgWGXlxplEN6A0nm0wKrbCd60Kc";
            // "&mode=driving&sensor=false&language=en-EN&units=imperial";


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader sreader = new StreamReader(dataStream);
            string responsereader = sreader.ReadToEnd();
            response.Close();

            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml(responsereader);

             decimal dis = new decimal();
            if (xmldoc.GetElementsByTagName("status")[0].ChildNodes[0].InnerText == "OK")
            {
                XmlNodeList distance = xmldoc.GetElementsByTagName("distance");
              // dis= Convert.ToDouble(distance[0].ChildNodes[1].InnerText.Replace(" km", ""));
                dis = Convert.ToDecimal(distance[0].ChildNodes[1].InnerText.Replace(" km", ""));
            }
          //  using (LocationEntities2 db = new LocationEntities2())
            {
                LocationEntities2 db = new LocationEntities2();
                Location location = new Location();
                location.counter = 1;
                location.Target = source;
                location.Destination = destination;
                location.Distance = dis;

                Location lo = db.Location.Add(location);
                db.SaveChanges();
                return lo;
            }
               
            //return 0;
            //https://maps.googleapis.com/maps/api/distancematrix/json?units=imperial&origins=Jerusalem,DC&destinations=Tel+Aviv,NY&key=AIzaSyAwLbz_DgWGXlxplEN6A0nm0wKrbCd60Kc
            //throw new NotImplementedException();
        }
    }
}
