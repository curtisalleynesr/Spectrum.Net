﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectrum.Net.Core.Message.New
{
    public class Message : History.Message
    {
        [JsonProperty("lobby")]
        public Session.Lobby Lobby { get; internal set; }
    }
}
