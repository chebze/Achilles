using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Outgoing.User;

public class SendUserInfoMessage : OutgoingMessage
{
    public SendUserInfoMessage(IncomingMessageContext ctx) : base(5)
    {
        var user = ctx.User;
        if(user is null)
            return;

        this.Build(
            user.Id,
            user.Username,
            user.Figure,
            user.Gender.ToString(),
            user.Motto,
            user.Tickets,
            user.PoolFigure,
            user.Film,
            false
        );
    }

    public SendUserInfoMessage(int userId, string username, string figure, string gender, string motto, int tickets, string poolFigure, int film, bool isReceiveNews) : base(5)
    {
        this.Build(userId, username, figure, gender, motto, tickets, poolFigure, film, isReceiveNews);
    }

    private void Build(
        int userId,
        string username,
        string figure,
        string gender,
        string motto,
        int tickets,
        string poolFigure,
        int film,
        bool isReceiveNews
    )
    {
        this.AppendString(userId);
        this.AppendString(username);
        this.AppendString(figure);
        this.AppendString(gender);
        this.AppendString(motto);
        this.AppendWired(tickets);
        this.AppendString(poolFigure);
        this.AppendWired(film);
        this.AppendBoolean(isReceiveNews);

    }
}

