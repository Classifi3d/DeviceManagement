namespace Domain.Entities.User;
public class Device
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string OS { get; set; } = string.Empty;
    public string OSVersion { get; set; } = string.Empty;
    public string Processor { get; set; } = string.Empty;
    public int RAM { get; set; } = default;
    public string Description { get; set; } = string.Empty;
    public string? AssignedUserId { get; set; } 
    public User AssignedUser { get; set; } = null!;
}
