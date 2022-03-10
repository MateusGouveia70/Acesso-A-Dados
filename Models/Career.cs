﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaltaDataAccess.Models
{
    public class Career
    {
        public Career()
        {
            Items = new List<CareerItem>();
        }

        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<CareerItem> Items { get; set; }   
    }
}
 