﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarbleTest.Net
{
    public interface ISetupTest
    {
        void ToBe(string marble, 
            object values = null, 
            Exception errorValue = null);
    }
}
