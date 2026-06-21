using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NutriTEC.Application.DTOs.Admin;
using NutriTEC.Application.Interfaces.Admin;
using NutriTEC.Infrastructure.Persistence;

namespace NutriTEC.Infrastructure.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly NutriTecDbContext _dbContext;

    public AdminRepository(NutriTecDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AdminLoginRecord?> GetAdminLoginByEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        await using var command = CreateStoredProcedureCommand("dbo.sp_admin_login");
        AddParameter(command, "@email", email);

        await OpenConnectionAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new AdminLoginRecord
        {
            AdminId = reader.GetInt32(reader.GetOrdinal("admin_id")),
            UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
            Birthday = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("birthday"))),
            Email = reader.GetString(reader.GetOrdinal("email")),
            Name = reader.GetString(reader.GetOrdinal("name")),
            LastName = reader.GetString(reader.GetOrdinal("last_name")),
            HashPassword = reader.GetString(reader.GetOrdinal("hash_password"))
        };
    }

    public Task<bool> AdminExistsAsync(int adminId, CancellationToken cancellationToken)
    {
        return AdminExistsCoreAsync(adminId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<AdminProductResponse>> GetProductsAsync(
        string? productStatus,
        CancellationToken cancellationToken)
    {
        await using var command = CreateStoredProcedureCommand("dbo.sp_get_admin_products");
        AddParameter(command, "@product_status", productStatus);

        return await ReadProductsAsync(command, cancellationToken);
    }

    public async Task<AdminProductResponse?> GetProductByBarCodeAsync(
        string barCode,
        CancellationToken cancellationToken)
    {
        await using var command = CreateStoredProcedureCommand("dbo.sp_get_admin_product_by_barcode");
        AddParameter(command, "@bar_code", barCode);

        var products = await ReadProductsAsync(command, cancellationToken);
        return products.FirstOrDefault();
    }

    public async Task<AdminProductResponse?> UpdateProductStatusByAdminAsync(
        string barCode,
        int adminId,
        string newStatus,
        CancellationToken cancellationToken)
    {
        await using var command = CreateStoredProcedureCommand("dbo.sp_update_product_status_by_admin");
        AddParameter(command, "@bar_code", barCode);
        AddParameter(command, "@admin_id", adminId);
        AddParameter(command, "@new_status", newStatus);

        var products = await ReadProductsAsync(command, cancellationToken);
        return products.FirstOrDefault();
    }

    private async Task<IReadOnlyCollection<AdminProductResponse>> ReadProductsAsync(
        DbCommand command,
        CancellationToken cancellationToken)
    {
        await OpenConnectionAsync(cancellationToken);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var products = new List<AdminProductResponse>();
        while (await reader.ReadAsync(cancellationToken))
        {
            products.Add(MapProduct(reader));
        }

        return products;
    }

    private DbCommand CreateStoredProcedureCommand(string storedProcedureName)
    {
        var command = _dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = storedProcedureName;
        command.CommandType = CommandType.StoredProcedure;
        command.Transaction = _dbContext.Database.CurrentTransaction?.GetDbTransaction();
        return command;
    }

    private DbCommand CreateTextCommand(string commandText)
    {
        var command = _dbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = commandText;
        command.CommandType = CommandType.Text;
        command.Transaction = _dbContext.Database.CurrentTransaction?.GetDbTransaction();
        return command;
    }

    private async Task<bool> AdminExistsCoreAsync(int adminId, CancellationToken cancellationToken)
    {
        await using var command = CreateTextCommand(
            "SELECT CASE WHEN EXISTS (SELECT 1 FROM dbo.admin WHERE admin_id = @admin_id) THEN 1 ELSE 0 END");
        AddParameter(command, "@admin_id", adminId);

        await OpenConnectionAsync(cancellationToken);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result) == 1;
    }

    private async Task OpenConnectionAsync(CancellationToken cancellationToken)
    {
        var connection = _dbContext.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }
    }

    private static void AddParameter(DbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }

    private static AdminProductResponse MapProduct(DbDataReader reader)
    {
        return new AdminProductResponse
        {
            BarCode = reader.GetString(reader.GetOrdinal("bar_code")),
            ProductName = reader.GetString(reader.GetOrdinal("product_name")),
            PortionUnit = reader.GetString(reader.GetOrdinal("portion_unit")),
            PortionSize = reader.GetDecimal(reader.GetOrdinal("portion_size")),
            Calories = reader.GetDecimal(reader.GetOrdinal("calories")),
            Fat = reader.GetDecimal(reader.GetOrdinal("fat")),
            Sodium = reader.GetDecimal(reader.GetOrdinal("sodium")),
            Carbohydrates = reader.GetDecimal(reader.GetOrdinal("carbohydrates")),
            Protein = reader.GetDecimal(reader.GetOrdinal("protein")),
            Vitamins = reader.GetDecimal(reader.GetOrdinal("vitamins")),
            Calcium = reader.GetDecimal(reader.GetOrdinal("calcium")),
            Iron = reader.GetDecimal(reader.GetOrdinal("iron")),
            ProductStatus = reader.GetString(reader.GetOrdinal("product_status")),
            CreatedByUserId = reader.GetInt32(reader.GetOrdinal("created_by_user_id")),
            CreatedByName = reader.GetString(reader.GetOrdinal("created_by_name")),
            CreatedByEmail = reader.GetString(reader.GetOrdinal("created_by_email"))
        };
    }
}
