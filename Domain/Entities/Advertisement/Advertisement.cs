using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities.Comments;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Domain.Entities;

public class Advertisement
{
    public int Id { get; set; }

    public string Brand { get; set; } = default!;
    public string Model { get; set; } = default!;
    public int Year { get; set; }
    public int Mileage { get; set; }
    public int EngineVolume { get; set; }

    public DocumentStatus DocumentStatus { get; set; }

    public long Price { get; set; }

    public string PhoneNumber { get; set; } = default!;
    public string Description { get; set; } = default!;

    public AdvertisementStatus Status { get; set; }

    public string UserId { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool Published { get; set; }

    public List<AdvertisementImage> Images { get; set; } = new();

    public byte EngineHealth { get; set; }  
    public byte SuspensionHealth { get; set; } 
    public byte TireHealth { get; set; }
    public List<string> Features { get; set; } = new();
    public ICollection<VehicleHistory>? Histories { get; set; } = new List<VehicleHistory>();

    public ICollection<Comment>? Comments { get; set; } = new List<Comment>();
     

}

