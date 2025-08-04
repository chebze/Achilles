using Achilles.Habbo.Messaging.Abstractions;
using Achilles.Habbo.Messaging.Abstractions.Outgoing;
using Achilles.TCP.Abstractions;

namespace Achilles.Habbo.Messaging.Outgoing.User;

public class AccountPreferencesMessage : OutgoingMessage
{
    public AccountPreferencesMessage(IncomingMessageContext ctx) : base(308)
    {
        this.AppendBoolean(ctx?.User?.SoundEnabled ?? true);
        this.AppendBoolean(ctx?.User?.TutorialFinished ?? true);
    }

    public AccountPreferencesMessage(bool soundSetting, bool tutorialFinished) : base(308)
    {
        this.AppendBoolean(soundSetting);
        this.AppendBoolean(tutorialFinished);
    }
}