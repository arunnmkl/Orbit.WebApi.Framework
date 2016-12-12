using System;
using Orbit.Entities.MyModule.Models;
using Orbit.Entities.MyModule.Repository;

namespace Orbit.Entities.MyModule
{
    internal interface IMyModuleManager
    {
        bool Post(Sample model);

        Sample Get(int moduleId);

        bool Put(Sample model);

        bool Delete(int moduleId);
    }

    public class MyModuleManager : IMyModuleManager
    {
        private readonly IMyModuleRepository myModuleRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyModuleManager" /> class.
        /// </summary>
        public MyModuleManager()
        {
            myModuleRepository = new MyModuleSqlRepository();
        }

        public bool Delete(int moduleId)
        {
            throw new NotImplementedException();
        }

        public Sample Get(int moduleId)
        {
            throw new NotImplementedException();
        }

        public bool Post(Sample model)
        {
            throw new NotImplementedException();
        }

        public bool Put(Sample model)
        {
            throw new NotImplementedException();
        }
    }
}
