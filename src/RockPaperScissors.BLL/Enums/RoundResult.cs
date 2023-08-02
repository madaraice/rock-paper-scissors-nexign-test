using System.Text.Json.Serialization;

namespace RockPaperScissors.BLL.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RoundResult
{
    Unknown = 0,
    Win = 1,
    Lose = 2,
    Draw = 3
}