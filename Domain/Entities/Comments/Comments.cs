using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Comments
{
    public class Comment
    {

        public int Id { get; set; }
        public string Username { get; set; }

        public string Message { get; set; }

        public string? Reply { get; set; }

        public int AdvertisementId { get; set; }
        public Advertisement Advertisement { get; set; }

    }
}
