using System;
using System.Collections.Generic;
using System.Text;
using MotoX.Domain.Entities;

namespace Domain.Entities.Receipts
{
    public class Receipts
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string? Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User{ get; set; }
    }
}
