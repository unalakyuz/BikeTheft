namespace BikeTheft.Service
{
    public interface IBikeTheftService
    {
        Task<Dictionary<string, int>> GetStolenBikesByCity(string cityName);

        IList<string> GetCities();
    }
}
