namespace Common.Dto;

public class LeaderboardEntryDTO
{
    public Guid Id { get; set; }
    public string Pseudo { get; set; } = null!;
    public int Score { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}