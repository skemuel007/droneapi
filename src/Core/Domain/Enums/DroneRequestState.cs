using System.ComponentModel;

namespace Domain.Enums;

public enum DroneRequestState
{
    [Description("Engaged")]
    ENGAGED = 1,
    [Description("Disengaged")]
    DISENGAGED = 2
}