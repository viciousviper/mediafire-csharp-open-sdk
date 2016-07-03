using System;

namespace MediaFireSDK.Services
{
    internal interface ICryptoService
    {
        string GetMd5Hash(string s);

        string GetSha1Hash(string s);
    }
}
