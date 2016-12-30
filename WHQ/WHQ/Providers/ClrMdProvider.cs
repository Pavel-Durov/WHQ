#define CLRMD

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace DllRefProvider.Providers
{
    class ClrMdProvider
    {
        static ClrMdProvider()
        {
         
        }

        private OfficialClrMd _officialClrMd;
        private ForkedClrMd _forkedClrMd;

        public void LoadAssemblies()
        {
            _officialClrMd = new OfficialClrMd();
            _forkedClrMd = new ForkedClrMd();
        }
    }

    abstract class Base
    {
        public abstract string PATH { get; }

        Assembly _assembly;

        public void Load()
        {
            _assembly = Assembly.LoadFrom(PATH);
        }
    }

    class OfficialClrMd : Base
    {
        public override string PATH => @"..\..\lib\net40\Microsoft.Diagnostics.Runtime.dll";
        
    }

    class ForkedClrMd : Base
    {
        public override string PATH => @"..\..\lib\Microsoft.Diagnostics.Runtime\Microsoft.Diagnostics.Runtime.dll";
        
        
    }
}
