using Achilles.Habbo.Messaging.Abstractions.Outgoing;

namespace Achilles.Habbo.Messaging.Outgoing.Tutorial;

public class TutorialConfigurationMessage : OutgoingMessage
{
    public TutorialConfigurationMessage(int tutorialId, string tutorialName, List<TutorialTopic> topics) : base(327)
    {
        this.AppendWired(tutorialId);
        this.AppendString(tutorialName);
        this.AppendWired(topics.Count);

        foreach(var topic in topics)
        {
            this.AppendWired(topic.Id);
            this.AppendString(topic.Name);
            this.AppendWired(topic.Status);
        }
    }
}
