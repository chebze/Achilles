using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.User;

public class SoundSettingMessage : OutgoingMessage
{
    public SoundSettingMessage(bool soundSetting) : base(309)
    {
        this.AppendBoolean(soundSetting);
        this.AppendWired(0); // TODO: find out what this is
    }
}