using System.Text.Json;

namespace BikeTheft.Service
{
    public class BikeTheftService : IBikeTheftService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BikeTheftService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Dictionary<string, int>> GetStolenBikesCountByCity(string cityName)
        {
            var httpClient = _httpClientFactory.CreateClient("BikeIndex");

            bool hasPage = true;
            int page = 1;
            int stolenBikesCount = 0;

            var dictionary = new Dictionary<string, int>();

            try
            {
                do
                {
                    var apiResult = await httpClient.GetAsync("https://bikeindex.org:443/api/v3/search?page=" + page + "&per_page=100&location=" + cityName + "&distance=100&stolenness=proximity");

                    if (apiResult.IsSuccessStatusCode)
                    {
                        var apiStreamResult = await apiResult.Content.ReadAsStreamAsync();

                        var result = JsonSerializer.Deserialize<ApiResponseObject>(apiStreamResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        if (result != null && result.Bikes != null && result.Bikes.Any())
                        {
                            stolenBikesCount += result.Bikes.Count(x => x.Stolen is true);
                            page++;

                            var orderedResult = result.Bikes.OrderByDescending(x => x.Date_Stolen);

                            foreach (var bike in orderedResult)
                            {
                                if (bike.Date_Stolen != null)
                                {
                                    var date = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(bike.Date_Stolen)).DateTime;

                                    if (!dictionary.ContainsKey($"{date.Year}-{date.Month}"))
                                    {
                                        dictionary.Add($"{date.Year}-{date.Month}", 1);
                                    }
                                    else
                                    {
                                        dictionary.TryGetValue($"{date.Year}-{date.Month}", out int currentCount);
                                        dictionary[$"{date.Year}-{date.Month}"] = currentCount + 1;
                                    }
                                }
                            }
                        }
                        else
                        {
                            hasPage = false;
                        }
                    }
                } while (hasPage);
            }

            catch (Exception ex)
            {
                throw;
            }
            return dictionary;
        }
    }

    public class ApiResponseObject
    {
        public IList<Bike>? Bikes { get; set; }
    }

    public class Bike
    {
        public int? Date_Stolen { get; set; }
        public bool? Stolen { get; set; }
    }
}
