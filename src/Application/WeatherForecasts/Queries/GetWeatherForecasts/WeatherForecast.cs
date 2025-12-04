namespace OmniRepo.Application.WeatherForecasts.Queries.GetWeatherForecasts;

public class WeatherForecast
{
    public DateTime Date { get; init; }

    public int TemperatureC { get; init; }

    public int TemperatureF => 32 + (int)Math.Round(TemperatureC * 9.0 / 5.0);


    public string Summary { get; init; } = string.Empty;
}
