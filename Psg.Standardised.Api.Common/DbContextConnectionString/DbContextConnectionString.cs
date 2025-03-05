namespace Psg.Standardised.Api.Common.DbContextConnectionString
{
    public class DbContextConnectionString
    {
        public string ConnectionString { get; set; }

        public DbContextConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
        }
    }
}
