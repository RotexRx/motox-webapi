using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Features.Contact
{
    public class ContactDto
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public string Phone { get; set; }

    }
}