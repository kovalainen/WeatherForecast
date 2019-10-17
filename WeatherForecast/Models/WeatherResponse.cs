using System.Collections.Generic;

namespace WeatherForecast.Models
{
	public class WeatherResponse
	{
		public Coord Coord { get; set; }
		public IEnumerable<Weather> Weather { get; set; }
		public string Base { get; set; }
		public Main Main { get; set; }
		public int Visibility { get; set; }
		public Wind Wind { get; set; }
		public Clouds Clouds { get; set; }
		public long Dt { get; set; }
		public Sys Sys { get; set; }
		public int Timezone { get; set; }
		public long Id { get; set; }
		public string Name { get; set; }
		public int Cod { get; set; }
	}
}
