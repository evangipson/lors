namespace Consogue.Interfaces
{
    /// <summary>
    /// We can put anything on the schedule as long as it has a Time,
    /// which represents how many turns pass until its time comes up again on the schedule.
    /// </summary>
    public interface IScheduleable
    {
        int Time { get; }
    }
}
