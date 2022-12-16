using System.ComponentModel;

namespace Domain.Enums;

public enum DroneModel
{
    [Description("Lightweight")]
    LIGHTWEIGHT = 1,
    [Description("Middleweight")]
    MIDDLEWEIGHT = 2,
    [Description("Cruiserweight")]
    CRUISERWEIGHT = 3,
    [Description("Heavyweight")]
    HEAVYWEIGHT = 4
}

public enum DroneRequestState
{
    [Description("Engaged")]
    ENGAGED = 1,
    [Description("Disengaged")]
    DISENGAGED = 2
}