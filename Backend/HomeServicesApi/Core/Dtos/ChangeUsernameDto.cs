﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class ChangeUsernameDto
    {
        public string NewUsername { get; set; }
        public string CurrentPassword { get; set; }

    }
}
