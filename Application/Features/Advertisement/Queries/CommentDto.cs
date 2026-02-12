using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;

public class CommentDto
{

    public int Id { get; set; }
    public string Username { get; set; }

    public string Message { get; set; }

    public string? Reply { get; set; }

    public int AdId{ get; set; }


}