namespace Consogue.Interfaces
{
    public interface IActor
    {
        /// <summary>
        /// Name of the actor.
        /// </summary>
        string Name { get; set; }
        int Awareness { get; set; }
        /// <summary>
        /// Number of dice to roll when performing an attack.
        /// Also represents the maximum amount of damage that can be inflicted in a single attack.
        /// </summary>
        int Attack { get; set; }
        /// <summary>
        /// Percentage chance that each die rolled is a success.
        /// A success for a die means that 1 point of damage was inflicted.
        /// </summary>
        int AttackChance { get; set; }
        /// <summary>
        /// Number of dice to roll when defending against an attack.
        /// Also represents the maximum amount of damage that can blocked or dodged from an incoming attack.
        /// </summary>
        int Defense { get; set; }
        /// <summary>
        /// Percentage chance that each die rolled is a success.
        /// A success for a die means that 1 point of damage was blocked.
        /// </summary>
        int DefenseChance { get; set; }
        /// <summary>
        /// How much money the actor has.
        /// </summary>
        int Gold { get; set; }
        /// <summary>
        /// Current health total of the actor.
        /// If this value is 0 or less then the actor is killed.
        /// </summary>
        int Health { get; set; }
        /// <summary>
        /// The amount of health the actor has when fully healed.
        /// </summary>
        int MaxHealth { get; set; }
        /// <summary>
        /// How fast the actor is.
        /// This determines the rate at which they perform actions.
        /// A lower number is faster.
        /// e.g.; An actor with a speed of 10 will perform twice as many
        /// actions in the same time as an actor with a speed of 20.
        /// </summary>
        int Speed { get; set; }
    }
}
