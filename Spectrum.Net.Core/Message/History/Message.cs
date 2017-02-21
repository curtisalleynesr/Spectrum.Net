﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spectrum.Net.Core.Message.History
{
    public class Message
    {
        [JsonProperty("content_state")]
        public ContentState ContentState { get; internal set; }

        [JsonProperty("highlight_role_id")]
        public UInt64? HighlightRoleId { get; internal set; }

        [JsonProperty("id")]
        public UInt64 Id { get; internal set; }

        [JsonProperty("lobby_id")]
        public UInt64 LobbyId { get; internal set; }

        // Media

        [JsonProperty("media_id")]
        public String MediaId { get; internal set; }

        [JsonProperty("member")]
        public Member Member { get; internal set; }

        [JsonProperty("member_id")]
        public UInt64 MemberId { get; internal set; }

        private String _plainText;

        [JsonProperty("plaintext")]
        public String PlainText
        {
            get { return this._plainText = this._plainText ?? String.Join(" ", this.ContentState.Blocks.Select(c => c.Text)); }
            internal set { this._plainText = value; }
        }

        [JsonProperty("time_created")]
        public Int64 TimeCreated { get; internal set; } = (DateTime.UtcNow.Ticks - 621355968000000000) / 10000000;

        [JsonProperty("time_modified")]
        public Int64 TimeModified { get; internal set; } = (DateTime.UtcNow.Ticks - 621355968000000000) / 10000000;

        [JsonProperty("reactions")]
        public Reaction[] Reactions { get; internal set; }

        [JsonProperty("erased_by")]
        public Member ErasedBy { get; internal set; }

        [JsonProperty("is_erased")]
        public Boolean IsErased { get; internal set; }
    }
}