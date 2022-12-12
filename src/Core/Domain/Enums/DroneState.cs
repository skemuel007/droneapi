using System.ComponentModel;

namespace Domain.Enums;

public enum DroneState
{
    [Description("Idle")]
    IDLE = 1,
    [Description("Loading")]
    LOADING = 2,
    [Description("Loaded")]
    LOADED = 3,
    [Description("Delivering")]
    DELIVERING = 4,
    [Description("Delivered")]
    DELIVERED = 5,
    [Description("Returning")]
    RETURNING = 6
}