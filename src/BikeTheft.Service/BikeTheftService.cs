using BikeTheft.Service.Response;
using BikeTheft.Service.Settings;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace BikeTheft.Service
{
    public class BikeTheftService : IBikeTheftService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IOptions<BikeIndexApiSettings> _bikeIndexApiSettings;

        public BikeTheftService(
            IHttpClientFactory httpClientFactory,
            IOptions<BikeIndexApiSettings> clientOptions)
        {
            _httpClientFactory = httpClientFactory;
            _bikeIndexApiSettings = clientOptions;
        }

        //Data caching is suitable here cause we don't need the real time data for this task.
        public async Task<Dictionary<string, int>> GetStolenBikesCountByCity(string location)
        {
            var httpClient = _httpClientFactory.CreateClient("BikeIndex");

            int pageNumber = 1;
            bool hasNextPage = true;
            int stolenBikesCount = 0;
            var result = new Dictionary<string, int>();

            do
            {
                var apiResult = await httpClient.GetAsync(BuildRequestUrl(pageNumber, location));

                if (apiResult.IsSuccessStatusCode)
                {
                    var apiStreamResult = await apiResult.Content.ReadAsStreamAsync();
                    var apiObjectResult = JsonSerializer.Deserialize<ApiResponseDto>(apiStreamResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (apiObjectResult != null && apiObjectResult.Bikes != null && apiObjectResult.Bikes.Any())
                    {
                        stolenBikesCount += apiObjectResult.Bikes.Count(x => x.Stolen is true);
                        var orderedResult = apiObjectResult.Bikes.OrderByDescending(x => x.Date_Stolen);

                        foreach (var bike in orderedResult)
                        {
                            if (bike.Date_Stolen != null)
                            {
                                var date = BuildDateTime(bike.Date_Stolen.Value);

                                if (!result.ContainsKey(BuildDictionaryKey(date)))
                                {
                                    result.Add(BuildDictionaryKey(date), 1);
                                }
                                else
                                {
                                    result.TryGetValue(BuildDictionaryKey(date), out int total);
                                    result[BuildDictionaryKey(date)] = total + 1;
                                }
                            }
                        }
                        pageNumber++;
                    }
                    else
                    {
                        hasNextPage = false;
                    }
                }
                else
                {
                    throw new Exception("Request failed for bike index api.");
                }
            } while (hasNextPage);
            return result;
        }

        public IList<string> GetCities()
        {
            return _bikeIndexApiSettings.Value.Cities.Split(",");
        }

        private static DateTime BuildDateTime(long date)
        {
            return DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(date)).DateTime;
        }

        private static string BuildDictionaryKey(DateTime date)
        {
            return $"{date.Year}-{date.Month}";
        }

        private string BuildRequestUrl(int pageNumber, string location)
        {
            var perPage = _bikeIndexApiSettings.Value.PerPage;
            var distance = _bikeIndexApiSettings.Value.Distance;
            var apiBaseUrl = _bikeIndexApiSettings.Value.ApiBaseUrl;

            return $"{apiBaseUrl}search?page={pageNumber}&per_page={perPage}&location={location}&distance={distance}&stolenness=proximity";
        }
    }
}
