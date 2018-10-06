using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consogue.Interfaces
{

    public interface IEquipment
    {
        int Attack { get; set; }
        int AttackChance { get; set; }
        int Defense { get; set; }
        int DefenseChance { get; set; }
        int Value { get; set; }
        int Speed { get; set; }
        string Name { get; set; }
    }
}