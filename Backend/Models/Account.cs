using System;
using System.Collections.Generic;

namespace Backend.Models;

public class Account {
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public decimal Balance { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
    public ICollection<Transaction>? Transactions { get; set; }
}
