using EveCentralProvider.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace EveCentralProvider
{
	public sealed class Services : IServices
	{
		private readonly string ApiFormat = "http://api.eve-central.com/api/{0}?{1}";
		private readonly string UserAgent = String.Format(".NET Eve Central Provider v{0}", FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion);

		private static readonly Services _instance = new Services();
		public static Services Instance { get { return _instance; } }

		private Services()
		{

		}

		public List<TypeMarketStats> MarketStat(List<int> typeid, List<int> regionlimit, int minQ = 10001, int usesystem = 0, int hours = 24)
		{
			var args = HttpUtility.ParseQueryString(string.Empty);

			foreach (int type in typeid)
			{
				args.Add("typeid", type.ToString());
			}

			args.Add("minQ", minQ.ToString());
			
			foreach (int region in regionlimit)
			{
				args.Add("regionlimit", region.ToString());
			}

			if (usesystem != 0)
				args.Add("usesystem", usesystem.ToString());

			if (hours != 24)
				args.Add("hours", hours.ToString());

			Uri apiUrl = new Uri(String.Format(ApiFormat, "marketstat", args.ToString()));

			HttpWebRequest req = WebRequest.CreateHttp(apiUrl);
			req.UserAgent = UserAgent;
			req.Method = "GET";

			List<TypeMarketStats> output = new List<TypeMarketStats>();
			using (var response = req.GetResponse())
			{
				XmlSerializer xml = new XmlSerializer(typeof(EveCentralApiMarketStatResult));
				var stream = response.GetResponseStream();
				var results = (EveCentralApiMarketStatResult)xml.Deserialize(stream);
				output = results.marketstat.type;
			}
			return output;
		}

		public QuickLookResult QuickLook(int typeid, List<int> regionlimit, int setminQ = 10001, int usesystem = 0, int sethours = 360)
		{
			var args = HttpUtility.ParseQueryString(string.Empty);

			args.Add("typeid", typeid.ToString());

			foreach (int region in regionlimit)
			{
				args.Add("regionlimit", region.ToString());
			}

			args.Add("setminQ", setminQ.ToString());

			if (usesystem != 0)
				args.Add("usesystem", usesystem.ToString());

			if (sethours != 360)
				args.Add("sethours", sethours.ToString());

			Uri apiUrl = new Uri(String.Format(ApiFormat, "quicklook", args.ToString()));

			HttpWebRequest req = WebRequest.CreateHttp(apiUrl);
			req.UserAgent = UserAgent;
			req.Method = "GET";

			QuickLookResult output;
			using (var response = req.GetResponse())
			{
				XmlSerializer xml = new XmlSerializer(typeof(EveCentralApiQuickLookResult));
				var stream = response.GetResponseStream();
				var results = (EveCentralApiQuickLookResult)xml.Deserialize(stream);
				output = results.quicklook;
			}
			return output;
		}


		public QuickLookPathResult QuickLookPath(string start, string end, int type, int setminQ = 10001, int sethours = 360)
		{
			string apiRelativeUrl = String.Format("quicklook/onpath/from/{0}/to/{1}/fortype/{2}", start, end, type);

			var args = HttpUtility.ParseQueryString(string.Empty);

			if (setminQ != 10001)
				args.Add("setminQ", setminQ.ToString());

			if (sethours != 360)
				args.Add("sethours", sethours.ToString());

			Uri apiUrl = new Uri(String.Format(ApiFormat, apiRelativeUrl, args.ToString()));

			HttpWebRequest req = WebRequest.CreateHttp(apiUrl);
			req.UserAgent = UserAgent;
			req.Method = "GET";

			QuickLookPathResult output;
			using (var response = req.GetResponse())
			{
				XmlSerializer xml = new XmlSerializer(typeof(EveCentralApiQuickLookPathResult));
				var stream = response.GetResponseStream();
				var results = (EveCentralApiQuickLookPathResult)xml.Deserialize(stream);
				output = results.quicklook;
			}
			return output;
			
		}


		public List<TypeHistory> History(int type, LocaleType locale, string idOrName, OrderType bid)
		{
			string apiRelativeUrl = String.Format("history/for/type/{0}/{1}/{2}/bid/{3}", type, locale.ToString().ToLower(), idOrName, (int)bid);

			Uri apiUrl = new Uri(String.Format(ApiFormat, apiRelativeUrl, String.Empty));

			HttpWebRequest req = WebRequest.CreateHttp(apiUrl);
			req.UserAgent = UserAgent;
			req.Method = "GET";

			List<TypeHistory> output = new List<TypeHistory>();
			using (var response = req.GetResponse())
			{
				var stream = response.GetResponseStream();
				StreamReader reader = new StreamReader(stream);
				string json = reader.ReadToEnd();
				dynamic parsed = JObject.Parse(json);
				dynamic values = parsed.values;
				foreach (var item in values)
				{
					TypeHistory derp = new TypeHistory();
					derp.Median = (float)item.median;
					derp.Maximum = (float)item.max;
					derp.Average = (float)item.avg;
					derp.StandardDeviation = (float)item.stdDev;
					derp.Minimum = (float)item.min;
					derp.Volume = (long)item.volume;
					derp.FivePercent = (float)item.fivePercent;
					derp.At = (DateTime)item.at;
					output.Add(derp);
				}
			}
			return output;
		}

		public EveMonResult EveMon()
		{
			string apiRelativeUrl = "evemon";
			Uri apiUrl = new Uri(String.Format(ApiFormat, apiRelativeUrl, String.Empty));

			HttpWebRequest req = WebRequest.CreateHttp(apiUrl);
			req.UserAgent = UserAgent;
			req.Method = "GET";

			EveMonResult output;
			using (var response = req.GetResponse())
			{
				XmlSerializer xml = new XmlSerializer(typeof(EveMonResult));
				var stream = response.GetResponseStream();
				var results = (EveMonResult)xml.Deserialize(stream);
				output = results;
			}
			return output;
		}

		public List<RouteJump> Route(string start, string end)
		{
			string apiRelativeUrl = String.Format("route/from/{0}/to/{1}", start, end);

			Uri apiUrl = new Uri(String.Format(ApiFormat, apiRelativeUrl, String.Empty));

			HttpWebRequest req = WebRequest.CreateHttp(apiUrl);
			req.UserAgent = UserAgent;
			req.Method = "GET";
			List<RouteJump> output = new List<RouteJump>();
			using (var response = req.GetResponse())
			{
				var stream = response.GetResponseStream();
				StreamReader reader = new StreamReader(stream);
				string json = reader.ReadToEnd();
				dynamic parsed = JArray.Parse(json);
				foreach (var item in parsed)
				{
					var derp = JsonConvert.DeserializeObject<RouteJump>(item.ToString());
					output.Add(derp);
				}
			}
			return output;
		}
	}
}
