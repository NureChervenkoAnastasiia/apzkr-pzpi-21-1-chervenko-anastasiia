namespace TastifyAPI.Data
{
    public class TastifyDbSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string RestaurantsCollectionName { get; set; } = null!;
        public string StaffCollectionName { get; set; } = null!;

    }
}
