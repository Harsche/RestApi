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

    public User ToUser()
    {
        return new User
        {
            Id = Id,
            Username = Username,
            CreateDate = DateTime.Now
        };
    }
}
