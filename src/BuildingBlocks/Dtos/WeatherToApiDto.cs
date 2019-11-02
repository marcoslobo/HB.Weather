using System;
using System.Collections.Generic;
using System.Text;


namespace Infra.Dto
{
    public class WeatherToApiDto
        {

        public string CityName { get; set; }
        public string CountryName { get; set; }

        public float MaxTemp { get; set; }

        public float MinTemp { get; set; }

        public DateTime Date { get; set; }
    }

}
