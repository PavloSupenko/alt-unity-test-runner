﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace TestsClient.Serialization;

[Serializable]
public class AppiumSessionsListData
{
    [JsonProperty("sessionId")]
    public object SessionId;
    
    [JsonProperty("status")]
    public int Status;
    
    [JsonProperty("value")]
    public List<AppiumSessionData> Value;
}

[Serializable]
public class AppiumSessionData
{
    [JsonProperty("id")]
    public string Id;
    
    [JsonProperty("capabilities")]
    public Dictionary<string, string> Capabilities;
}