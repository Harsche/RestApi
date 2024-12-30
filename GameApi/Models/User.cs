using System;

namespace GameApi.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public DateTime CreateDate { get; set; }
}
