using System;
using Orbit.Entities.MyModule.Models;
using Orbit.WebApi.Base.SqlSerializer;

namespace Orbit.Entities.MyModule.Repository
{
    internal class MyModuleSqlRepository : IMyModuleRepository
    {
        /// <summary>
        /// The SQL serializer
        /// </summary>
        private readonly SqlSerializer sqlSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MyModuleSqlRepository"/> class.
        /// </summary>
        public MyModuleSqlRepository()
        {
            sqlSerializer = CommonContext.ApplicationDal;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyModuleSqlRepository"/> class.
        /// </summary>
        /// <param name="sqlSerializer">The SQL serializer.</param>
        public MyModuleSqlRepository(SqlSerializer sqlSerializer)
        {
            this.sqlSerializer = sqlSerializer;
        }

        public bool Create(Sample model)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int mduleId)
        {
            throw new NotImplementedException();
        }

        public Sample Read(int mduleId)
        {
            throw new NotImplementedException();
        }

        public bool Update(Sample model)
        {
            throw new NotImplementedException();
        }
    }
}
