using System;

namespace Backend.Models;

public class Transaction {
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? TargetAccountNumber { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Account? Account { get; set; }
}
