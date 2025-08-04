using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.User;

public class UserObjectMessage : OutgoingMessage
{
    public UserObjectMessage(
        int userId,
        string username,
        string figure,
        string sex,
        string motto,
        int tickets,
        string poolFigure,
        int film,
        bool receiveNews
    ) : base(5)
    {
        this.AppendWired(userId);
        this.AppendString(username);
        this.AppendString(figure);
        this.AppendString(sex);
        this.AppendString(motto);
        this.AppendWired(tickets);
        this.AppendString(poolFigure);
        this.AppendWired(film);
        this.AppendBoolean(receiveNews);
    }
}
