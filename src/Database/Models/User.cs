namespace Achilles.Database.Models;

public class User : BaseModel<int>
{
    public string? SSOTicket { get; set; } = null;

    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public required string Birthday { get; set; }

    public required string Rank { get; set; } = string.Empty;

    public string Motto { get; set; } = string.Empty;
    public string ConsoleMotto { get; set; } = string.Empty;

    public required char Gender { get; set; }
    public required string Figure { get; set; }
    public string PoolFigure { get; set; } = string.Empty;

    public int Credits { get; set; } = 0;
    public int Film { get; set; } = 0;
    public int Tickets { get; set; } = 0;

    public DateTime? ClubSubscriptionStart { get; set; } = null;
    public DateTime? ClubSubscriptionEnd { get; set; } = null;

    public string? CurrentBadge { get; set; } = null;
    public bool ShowBadge { get; set; } = true;
    public List<string> Badges { get; set; } = [];

    public bool AllowStalking { get; set; } = true;
    public bool AllowFriendRequests { get; set; } = true;
    public bool SoundEnabled { get; set; } = true;
    public bool TutorialFinished { get; set; } = false;

    public DateTime? LastOnline { get; set; } = null;
    public DateTime? LastLogin { get; set; } = null;

    public bool IsClubMember => this.ClubSubscriptionStart is not null && this.ClubSubscriptionEnd is not null && this.ClubSubscriptionStart <= DateTime.Now && this.ClubSubscriptionEnd >= DateTime.Now;
    
    public void UpdateClubSubscriptionStatus()
    {
        if(this.ClubSubscriptionStart is null || this.ClubSubscriptionEnd is null)
        {
            this.ClubSubscriptionStart = null;
            this.ClubSubscriptionEnd = null;
            this.Badges.Remove("HC1");
            this.Badges.Remove("HC2");
            return;
        }

        if(this.IsClubMember)
        {
            if(!this.Badges.Contains("HC1"))
                this.Badges.Add("HC1");
            if(!this.Badges.Contains("HC2"))
                this.Badges.Add("HC2");
        }

        if(this.ClubSubscriptionEnd < DateTime.Now)
        {
            this.ClubSubscriptionStart = null;
            this.ClubSubscriptionEnd = null;
            this.Badges.Remove("HC1");
            this.Badges.Remove("HC2");
        }
    }
}
