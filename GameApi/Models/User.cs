using System;
using System.ComponentModel.DataAnnotations;
using GameApi.Models.DTO;

namespace GameApi.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public DateTime CreateDate { get; set; }

    public UserDTO ToDTO()
    {
        return new UserDTO
        {
            Id = Id,
            Username = Username
        };
    }
}
