namespace Consogue.Interfaces
{
    public interface ITreasure
    {
        string Name { get; set; }
        bool PickUp(IActor actor);
    }
}
