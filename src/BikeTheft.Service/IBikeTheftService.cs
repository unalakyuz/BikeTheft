namespace BikeTheft.Service
{
    public interface IBikeTheftService
    {
        Task<Dictionary<string, int>> GetStolenBikesCountByCity(string cityName);
    }
}
