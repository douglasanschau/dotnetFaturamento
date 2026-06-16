using Dapper;
using faturamentoClienteDotnet.Data;
using faturamentoClienteDotnet.Models;

namespace faturamentoClienteDotnet.Services;

public class UsuarioService
{
    private readonly Database _database;

    public UsuarioService(Database database)
    {
        _database = database;
    }

    public UsuarioModel? GetUsuarioByEmail(string email) 
    {
        using var conn = _database.GetConnection();
        return conn.QueryFirstOrDefault<UsuarioModel>(
            @"SELECT * 
                FROM usuario
              WHERE email = @Email",
            new { Email = email }
        );
    }

    public UsuarioModel? GetUsuarioById(int id) 
    {
        using var conn = _database.GetConnection();
        return conn.QueryFirstOrDefault<UsuarioModel>(
            @"SELECT * 
                FROM usuario
              WHERE id = @Id",
            new { Id = id }
        );
    }


    public int SalvarUsuario(UsuarioModel usuario)
    {
        using var conn = _database.GetConnection();

        return conn.ExecuteScalar<int>(
            @"INSERT INTO usuario
            (
                nome,
                email,
                senha
            )
            VALUES
            (
                @Nome,
                @Email,
                @Senha
            );

            SELECT LAST_INSERT_ID();",
            usuario
        );
    }
}