﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zorbo.Interface
{
    public enum AdminLevel : byte
    {
        User,
        Moderator = 1,
        Admin = 2,
        Host = 3,
    }
}
