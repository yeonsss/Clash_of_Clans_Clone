using System;

public class JobStartEventArgs : EventArgs
{
    public string taskId { get; set; }
    public string monsterName { get; set; }
    public float remainingTime { get; set; }
}
