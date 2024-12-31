using System;
using System.ComponentModel.DataAnnotations;

namespace GameApi.Models.DTO;

public class UserDTO
{
    public int Id { get; set; }
    [Required]
    [MaxLength(20)]
    [MinLength(2)]
    public string Username { get; set; }

    [Range(1, 100)]
    public int Level { get; set; }

    public User ToUser()
    {
        return new User
        {
            Id = Id,
            Username = Username,
            Level = Level
        };
    }
}
