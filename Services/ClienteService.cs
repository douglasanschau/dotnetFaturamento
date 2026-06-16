using Dapper;
using System.Text;
using faturamentoClienteDotnet.Data;
using faturamentoClienteDotnet.Models;

namespace faturamentoClienteDotnet.Services;

public class ClienteService
{
    private readonly Database _database;

    public ClienteService(Database database)
    {
        _database = database;
    }

    public ClienteModel? GetClienteByEmail(int usuarioId, string email)
    {
        using var conn = _database.GetConnection();
        return conn.QueryFirstOrDefault<ClienteModel>(
            @"SELECT * 
                FROM cliente
              WHERE usuario_id = @UsuarioId AND email = @Email",
            new 
            {
                UsuarioId = usuarioId,
                Email = email
            }
        );
    }

    public ClienteModel? GetClienteById(int usuarioId, int id) 
    {
        using var conn = _database.GetConnection();
        return conn.QueryFirstOrDefault<ClienteModel>(
            @"SELECT * 
                FROM cliente
              WHERE usuario_id = @UsuarioId AND id = @Id",
            new 
            {
                UsuarioId = usuarioId,
                Id = id
            }
        );
    }

    public List<ClienteModel> GetClientes(int usuarioId, string? nome, string? email, string? documento)
    {   
        using var conn = _database.GetConnection();

        var sql = new StringBuilder();

        sql.Append(@"
            SELECT * FROM cliente
            WHERE usuario_id = @UsuarioId");

        var parameters = new DynamicParameters();
        parameters.Add("UsuarioId", usuarioId);

        if(!string.IsNullOrWhiteSpace(nome)){
            sql.Append(" AND nome LIKE CONCAT('%', @Nome, '%') ");
            parameters.Add("Nome", nome);
        }

        if(!string.IsNullOrWhiteSpace(email)){
            sql.Append(" AND email LIKE CONCAT('%', @Email, '%') ");
            parameters.Add("Email", email);
        }

        if(!string.IsNullOrWhiteSpace(documento)){
            sql.Append(" AND documento = @Documento ");
            parameters.Add("Documento", documento);
        }

        return conn.Query<ClienteModel>(
            sql.ToString(),
            parameters
        ).ToList();
    }

    public int SalvarCliente(ClienteModel cliente)
    {
        using var conn = _database.GetConnection();

        return conn.ExecuteScalar<int>(
            @"INSERT INTO cliente
            (
                usuario_id,
                nome,
                email,
                documento
            )
            VALUES
            (
                @UsuarioId,
                @Nome,
                @Email,
                @Documento
            );

            SELECT LAST_INSERT_ID();",
            cliente
        );
    }

    public int AtualizarCliente(ClienteModel cliente)
    {
        using var conn = _database.GetConnection();

        return conn.Execute(
            @"UPDATE cliente
                SET
                    nome  = @Nome,
                    email = @Email,
                    documento = @Documento
                WHERE id = @Id",
                cliente
        );
    }

    public int ExcluirCliente(int clienteId)
    {
        using var conn = _database.GetConnection();

        return conn.Execute(
            @"DELETE FROM cliente
            WHERE id = @Id",
            new { Id = clienteId }
        );
    }
}