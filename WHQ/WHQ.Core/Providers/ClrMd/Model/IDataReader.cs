using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WHQ.Providers.ClrMd.Model;

namespace WHQ.Core.Providers.ClrMd.Model
{
    interface IDataReader
    {
        Architecture GetArchitecture();
    }
}
