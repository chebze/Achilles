using Achilles.Database.Abstractions;
using Achilles.Database.Models;
using Achilles.Habbo.Configuration;
using Achilles.Habbo.Data;
using Achilles.TCP.Abstractions;
using Achilles.TCP.Data;

namespace Achilles.Habbo.Messaging.Abstractions;

public class IncomingMessageContext : IDisposable
{
    private IServiceScope ServiceScope { get; }
    public IServiceProvider Services => this.ServiceScope.ServiceProvider;

    public IConnectionManager ConnectionManager { get; }
    public IConnection Connection { get; }
    public HabboConfiguration Configuration { get; }
    public HabboDbContext Database { get; }
    public ILogger<IConnection> Logger { get; }

    public User? User => this.Connection.Metadata.OfType<User>().FirstOrDefault();
    public RankConfiguration? Rank => this.Configuration.Ranks.FirstOrDefault(r => r.Name == this.User?.Rank);
    public Room? Room => this.Connection.Metadata.OfType<Room>().FirstOrDefault();
    public UserRoomState? UserRoomState => this.Connection.Metadata.OfType<UserRoomState>().FirstOrDefault();
    public ConnectionPingState PingState => this.Connection.Metadata.OfType<ConnectionPingState>().FirstOrDefault() ?? throw new Exception("No connection ping state found in connection metadata");

    public IncomingMessageContext(
        IServiceScope serviceScope,
        IConnection connection
    )
    {
        this.ServiceScope = serviceScope;
        this.Connection = connection;

        this.ConnectionManager = this.Services.GetRequiredService<IConnectionManager>();
        this.Configuration = this.Services.GetRequiredService<HabboConfiguration>();
        this.Database = this.Services.GetRequiredService<HabboDbContext>();
        this.Logger = this.Services.GetRequiredService<ILogger<IConnection>>();
    }

    public void Dispose()
    {
        this.ServiceScope.Dispose();
    }
}
