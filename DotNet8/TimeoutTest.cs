using System;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Xunit;

namespace DotNet8;

public sealed class TimeoutTest
{
    [Fact(Timeout = 300_000)]
    public async Task SELECT_SLEEP_45()
    {
        var cancellationToken = CancellationToken.None;
        var builder = new MySqlConnectionStringBuilder { Server = "localhost", UserID = "root" };
        await using var connection = new MySqlConnection(builder.ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT SLEEP(45)";
        await Assert.ThrowsAsync<MySqlException>(
            async () => await command.ExecuteNonQueryAsync(cancellationToken));
    }

    [Fact(Timeout = 300_000)]
    public async Task SELECT_SLEEP_45_WITH_CANCELLATION_TOKEN()
    {
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var cancellationToken = cancellation.Token;
        var builder = new MySqlConnectionStringBuilder { Server = "localhost", UserID = "root" };
        await using var connection = new MySqlConnection(builder.ConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT SLEEP(45)";
        await command.ExecuteNonQueryAsync(cancellationToken);
        Assert.True(cancellationToken.IsCancellationRequested);
    }
}