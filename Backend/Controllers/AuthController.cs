using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.Helpers;
using BCrypt.Net;
using Backend.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly BankingContext _context;
    private readonly JwtService _jwt;

    public AuthController(BankingContext context, JwtService jwt)
    {
        _context = context;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {

        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            return BadRequest("Email and password are required");

        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Email already registered");


        string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = passwordHash
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();


        // create a default bank account for the user
        var account = new Account
        {
            UserId = user.Id,
            AccountNumber = GenerateAccountNumber(),
            Balance = 0
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        // Optional: simple account number generator


        return Ok(new
        {
            message = "User registered successfully",
            userId = user.Id,
            accountNumber = account.AccountNumber
        });
    }

    private string GenerateAccountNumber()
    {

        var random = new Random();
        return "PT50-" + random.Next(10000000, 99999999);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] User request)
    {

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.PasswordHash, user.PasswordHash))
            return Unauthorized("Invalid credential");

        var token = _jwt.GenerateToken(user);
        return Ok(new { token });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _context.Users
            .Include(u => u.Accounts)
            .ThenInclude(a => a.Transactions)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
            return NotFound("User not found");

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "User and related accounts deleted successfully" });

    }





}



