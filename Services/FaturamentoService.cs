using Dapper;
using System.Text;
using faturamentoClienteDotnet.Data;
using faturamentoClienteDotnet.Models;
using faturamentoClienteDotnet.Enums;

namespace faturamentoClienteDotnet.Services;

public class FaturamentoService
{
    private readonly Database _database;

    public FaturamentoService(Database database)
    {
        _database = database;
    }

    public FaturamentoModel? GetFaturamentoById(int usuarioId, int id)
    {
        using var conn = _database.GetConnection();

        return conn.QueryFirstOrDefault<FaturamentoModel>(
            @"SELECT * 
              FROM faturamento
              WHERE usuario_id = @UsuarioId AND id = @Id",
            new 
            {
                UsuarioId = usuarioId,
                Id = id
            }
        );
    }

    public List<FaturamentoModel> GetFaturamentoByClienteId(int clienteId)
    {
        using var conn = _database.GetConnection();

        return conn.Query<FaturamentoModel>(
            @"SELECT * 
              FROM faturamento
              WHERE cliente_id = @ClienteId",
            new 
            {
                ClienteId = clienteId
            }
        ).ToList();
    }

    public int SalvarFaturamento(FaturamentoModel faturamento)
    {
        using var conn = _database.GetConnection();

        return conn.ExecuteScalar<int>(
            @"INSERT INTO faturamento
            (
                usuario_id,
                cliente_id, 
                titulo, 
                descricao,
                valor, 
                parcelas,
                status
            )
            VALUES
            (
                @UsuarioId,
                @ClienteId,
                @Titulo,
                @Descricao,
                @Valor,
                @Parcelas,
                @Status
            );

            SELECT LAST_INSERT_ID();",
            faturamento
        );
    }

    public void SalvarParcelamento(List<ParcelamentoModel> parcelamentos)
    {
        using var conn = _database.GetConnection();

        conn.Open();

        using var transaction = conn.BeginTransaction();

        try {
            conn.Execute(
                @"INSERT INTO parcelamento
                (
                    faturamento_id, 
                    valor, 
                    status
                )
                VALUES
                (
                    @FaturamentoId, 
                    @Valor, 
                    @Status
                )",
                parcelamentos,
                transaction
            );

            transaction.Commit();
        } catch {
            transaction.Rollback();
            throw;
        }
    }

    public int AtualizarFaturamento(int id, int usuarioId, FaturamentoModel parcelamento)
    {
        using var conn = _database.GetConnection();

        var sql = new StringBuilder();
        var updates = new List<string>();
        var parameters = new DynamicParameters();

        parameters.Add("Id", id);
        parameters.Add("UsuarioId", usuarioId);

        if(parcelamento.Status != default){
            updates.Add("status = @Status");
            parameters.Add("Status", parcelamento.Status);
        }

        if(parcelamento.Valor > 0){
            updates.Add("valor = @Valor");
            parameters.Add("Valor", parcelamento.Valor);
        }

        sql.Append($@"
            UPDATE faturamento
            SET {string.Join(", ", updates)}
            WHERE id = @Id and usuario_id = @UsuarioId
        ");

        return conn.Execute(
            sql.ToString(),
            parameters
        );
    }
    

    public ParcelamentoModel? GetParcelamentoById(int usuarioId, int id) 
    {
        using var conn = _database.GetConnection();
        return conn.QueryFirstOrDefault<ParcelamentoModel>(
            @"SELECT parcelamento.id, 
                     parcelamento.status, 
                     parcelamento.valor, 
                     parcelamento.faturamento_id as faturamentoId
                FROM parcelamento
                INNER JOIN faturamento
                   ON faturamento.id = parcelamento.faturamento_id
              WHERE faturamento.usuario_id = @UsuarioId AND parcelamento.id = @Id",
            new 
            {
                UsuarioId = usuarioId,
                Id = id
            }
        );
    }

    public List<ParcelamentoModel> GetParcelamentosByFaturamentoId(int usuarioId, int id)
    {
        using var conn = _database.GetConnection();

        return conn.Query<ParcelamentoModel>(
            @"SELECT parcelamento.*
                FROM parcelamento
                INNER JOIN faturamento
                   ON faturamento.id = parcelamento.faturamento_id
            WHERE faturamento.id = @FaturamentoId and faturamento.usuario_id = @UsuarioId",
            new { 
                UsuarioId = usuarioId,
                FaturamentoId = id
            }
        ).ToList();
    }

    public int AtualizarStatusParcelamento(int id, StatusFaturamento status)
    {
        using var conn = _database.GetConnection();

        return conn.Execute(
            @"UPDATE parcelamento
            SET
                status = @Status
            WHERE id = @Id",
            new {
                Id = id,
                Status = status
            }
        );
    }

    public int AtualizarParcelamentos(List <int> ids, ParcelamentoModel parcelamento)
    {
        using var conn = _database.GetConnection();

        var sql = new StringBuilder();
        var updates = new List<string>();
        var parameters = new DynamicParameters();

        parameters.Add("Ids", ids);

        if(parcelamento.Status != default){
            updates.Add("status = @Status");
            parameters.Add("Status", parcelamento.Status);
        }

        if(!string.IsNullOrWhiteSpace(parcelamento.ComprovantePagamento)){
            updates.Add("comprovante_pagamento = @ComprovantePagamento");
            parameters.Add("ComprovantePagamento", parcelamento.ComprovantePagamento);
        }
    
        sql.Append($@"
            UPDATE parcelamento
            SET {string.Join(", ", updates)}
            WHERE id IN @Ids 
        ");

        conn.Open();

        using var transaction = conn.BeginTransaction();

        try {
            var rows = conn.Execute(
                sql.ToString(),
                parameters,
                transaction
            );

            transaction.Commit();

            return rows;
        } catch {
            transaction.Rollback();
            throw;
        }
    }
}