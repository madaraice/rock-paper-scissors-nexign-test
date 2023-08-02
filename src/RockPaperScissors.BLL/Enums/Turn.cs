using System.Text.Json.Serialization;

namespace RockPaperScissors.BLL.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Turn
{
    Unknown = 0,
    Rock = 1,
    Paper = 2,
    Scissors = 3
}