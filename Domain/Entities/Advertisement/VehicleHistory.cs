using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;


public class VehicleHistory
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }

        public int AdvertisementId { get; set; }
        public Advertisement Advertisement { get; set; }
    }
