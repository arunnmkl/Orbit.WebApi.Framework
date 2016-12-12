using Orbit.Entities.MyModule.Models;

namespace Orbit.Entities.MyModule.Repository
{
    internal interface IMyModuleRepository
    {
        bool Create(Sample model);

        bool Update(Sample model);

        Sample Read(int mduleId);

        bool Delete(int mduleId);
    }
}
