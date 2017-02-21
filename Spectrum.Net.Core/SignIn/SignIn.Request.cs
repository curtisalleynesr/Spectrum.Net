﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectrum.Net.Core
{
    public static partial class SignIn
    {
        public class Request
        {
            [JsonProperty("username")]
            public String Username { get; set; }

            [JsonProperty("password")]
            public String Password { get; set; }

            [JsonProperty("remember")]
            public BooleanEnum Remember { get; set; }
        }
    }
}