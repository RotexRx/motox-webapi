using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs;

public class EditAdvertisementDto
{
    public int Id { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public long Price { get; set; }
    public int Mileage { get; set; }
    public string Description { get; set; }

    public List<string> Images { get; set; } // لیست URL تصاویر
    public HealthDto Health { get; set; }
    public List<HistoryDto> History { get; set; }
}