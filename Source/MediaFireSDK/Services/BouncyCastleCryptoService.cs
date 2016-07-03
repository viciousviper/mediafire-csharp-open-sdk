using System;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;

namespace MediaFireSDK.Services
{
    internal class BouncyCastleCryptoService : ICryptoService
    {
        public string GetMd5Hash(string s)
        {
            return GetHash(s, new MD5Digest());
        }

        public string GetSha1Hash(string s)
        {
            return GetHash(s, new Sha1Digest());
        }

        private static string GetHash(string s, IDigest algorithm)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            algorithm.BlockUpdate(bytes,0,bytes.Length);
            var res = new byte[algorithm.GetDigestSize()];
            algorithm.DoFinal(res, 0);
            return BitConverter.ToString(res).Replace("-", string.Empty);
        }
    }
}
