using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherForecast.Models;
using WeatherForecast.ViewModels;

namespace WeatherForecast.Controllers
{
	public class HomeController : Controller
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly string AppId = "701d10cbbee8cdc0dbc83891eb12df74";
		public HomeController(IHttpClientFactory httpClientFactory)
		{
			_httpClientFactory = httpClientFactory;
		}

		public IActionResult Index()
		{
			if (Request.Cookies.ContainsKey("LastCity"))
			{
				string city = Request.Cookies["LastCity"];
				ViewData["LastCity"] = city;
			}
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Weather(string city)
		{
			WeatherResponse weatherResponse = null;
			try
			{
				HttpClient client = _httpClientFactory.CreateClient("weather");
				HttpResponseMessage responseMessage = await client.GetAsync($"/data/2.5/weather?q={city}&units=metric&appid={AppId}");
				responseMessage.EnsureSuccessStatusCode();
				string result = await responseMessage.Content.ReadAsStringAsync();
				weatherResponse = JsonConvert.DeserializeObject<WeatherResponse>(result);
			}
			catch (HttpRequestException)
			{
				return View("Error");
			}
			DateTime now = DateTime.Now;
			CookieOptions option = new CookieOptions
			{
				Expires = now.AddDays(1)
			};
			Response.Cookies.Append("LastCity", city, option);
			WeatherViewModel weatherViewModel = new WeatherViewModel()
			{
				Precipitation = string.Join(", ", weatherResponse.Weather.Select(w => w.Main)),
				HighestTemp = weatherResponse.Main.Temp_max,
				LowestTemp = weatherResponse.Main.Temp_min,
			};
			ViewData["LastCity"] = city;

			if (!Request.Cookies.ContainsKey($"{city}WarningMessage"))
			{
				foreach (Weather weather in weatherResponse.Weather)
				{
					if (weather.Main == "Rain")
					{
						Response.Cookies.Append($"{city}WarningMessage", city, option);
						ViewData["WarningMessage"] = "Today is rainy!";
					}
				}
			}
			return View(weatherViewModel);
		}
	}
}
