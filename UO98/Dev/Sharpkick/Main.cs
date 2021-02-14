// UO:98 / Sidekick / Sharpkick : Protocol and Administration Extensions for Ultima Online(c) Demo.
// Copyright (C) 2011  JoinUO | derrick@joinuo.com Licensed under the Open Software License version 3.0
// This program is free software: you can redistribute it and/or modify it under the terms of OSL-3.0 
// This license applies to all files in this project.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

//You should have received a copy of the The Open Software License 3.0
//along with this program (COPYING.txt).  
// If not, see <http://www.opensource.org/licenses/OSL-3.0>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Sharpkick.Network;

namespace Sharpkick
{
    public class Main
    {
        /// <summary>
        /// Indicates that we have run our initialization routines
        /// </summary>
        public static bool Initialized {get;private set;}

        /// <summary>
        /// Called once, on first pulse from server. Set everything up here.
        /// </summary>
        public static void Initialize()
        {
            if(!Initialized)
            {
                // Call all class configuration routines.
                WorldSave.Configure();
                Accounting.Configure();
                PacketVersions.Configure();

                Initialized=true;

                Sharpkick.WorldBuilding.Builder.Configure();

                Console.WriteLine("Sharpkick Initialized.");
            }
        }
    }
}
