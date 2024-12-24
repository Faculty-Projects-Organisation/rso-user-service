﻿using Newtonsoft.Json;

namespace RSO.Core.BL.LogicModels.DTO;

/// <summary>
/// Stores the Zip Code and City from Dejci's API.
/// </summary>
public class CityData
{
    /// <summary>
    /// Zip Code.
    /// </summary>
    [JsonProperty("postnaStevilka")]
    public int ZipCode { get; set; }

    /// <summary>
    /// The name of the city that corresponds to the zip code.
    /// </summary>
    [JsonProperty("kraj")]
    public string? City { get; set; }
}
