using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orbit.WebApi.Base.SqlSerializer;

namespace Orbit.Entities
{
    class CommonContext
    {
        [ThreadStatic]
        private static SqlSerializer applicationDal;

        public static SqlSerializer ApplicationDal
        {
            get
            {
                return applicationDal ?? SqlSerializer.ByName("AuthContext");
            }

            set
            {
                applicationDal = value;
            }
        }
    }
}
