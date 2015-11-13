using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaFireSDK.Services
{
   internal interface ICryptoService
   {
       string GetSha1Hash(string s);
   }
}
