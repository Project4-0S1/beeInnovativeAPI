﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace beeInnovative.DAL.Models
{
    public class Color
    {
        public int Id {  get; set; }
        public string ColorName {  get; set; }

        public ICollection<Hornet>? Hornet { get; set; }
    }
}
