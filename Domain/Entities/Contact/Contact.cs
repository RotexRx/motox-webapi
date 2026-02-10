using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Contact
{
    public class Contact
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public string Phone { get; set; }

    }
}
