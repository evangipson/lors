using Consogue.Core;
using Consogue.Systems;

namespace Consogue.Interfaces
{
    public interface IBehavior
    {
        bool Act(Monster monster, CommandSystem commandSystem);
    }
}
