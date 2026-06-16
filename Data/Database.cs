using MySqlConnector;

namespace faturamentoClienteDotnet.Data;

public class Database
{
    private readonly IConfiguration _config;

    public Database(IConfiguration config)
    {
        _config = config;
    }

    public MySqlConnection GetConnection()
    {
        return new MySqlConnection(
            _config.GetConnectionString("DefaultConnection")
        );
    }
}