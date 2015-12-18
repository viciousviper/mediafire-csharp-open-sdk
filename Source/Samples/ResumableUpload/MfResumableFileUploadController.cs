using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaFireSDK;
using MediaFireSDK.Model;
using MediaFireSDK.Model.Responses;
using Org.BouncyCastle.Crypto.Digests;

namespace ResumableUpload
{
    public class MfResumableFileUploadController
    {
        private readonly IMediaFireAgent _agent;
        private readonly Stream _fileStream;
        public string FileHash { get; private set; }
        public string FileName { get; private set; }
        public long FileLength { get; private set; }


        public MfResumableFileUploadController(IMediaFireAgent agent, long fileLength, Stream fileStream, string fileName)
        {
            _agent = agent;
            FileLength = fileLength;
            _fileStream = fileStream;
            FileName = fileName;
        }

        public async Task Upload(ResumableUploadDetails resumableUpload)
        {
            UploadResponse uploadInfo = null;
            for (int i = 0; i < resumableUpload.NumberOfUnits; i++)
            {
                Console.WriteLine("##############  Uploading unit {0}  ##############", i);
                uploadInfo = await UploadUnit(i, resumableUpload);


            }


            int chunk = 0;
            foreach (short word in uploadInfo.ResumableUpload.Bitmap.Words)
            {
                for (int j = 0; j < 16 && chunk < uploadInfo.ResumableUpload.NumberOfUnits; j++, chunk++)
                {
                    if ((word & 0x1) != 1)
                        Console.WriteLine("Chunk {0}, failed to upload", chunk);
                }
            }

            if (uploadInfo.ResumableUpload.AllUnitsReady == false)
            {
                Console.WriteLine("Some chunks weren't uploaded.");
                return;
            }

            do
            {
                var uploadDetails = await _agent.Upload.PollUpload(uploadInfo.DoUpload.Key);

                if (uploadDetails.IsComplete)
                {
                    Console.WriteLine("File quick key is {0}", uploadDetails.QuickKey);
                    return;
                }
            } while (true);

        }



        private async Task<UploadResponse> UploadUnit(int unit, ResumableUploadDetails resumableUpload)
        {

            var block = new byte[resumableUpload.UnitSize];
            var unitSize = _fileStream.Read(block, 0, block.Length);
            var unitHash = GetBlockHash(block, unitSize);

            var headers = new Dictionary<string, string>
            {
                {MediaFireApiConstants.ContentTypeHeader, MediaFireApiConstants.ResumableUploadContentTypeValue } ,
                {MediaFireApiConstants.FileNameHeader, FileName } ,
                {MediaFireApiConstants.FileSizeHeader, FileLength.ToString() } ,
                {MediaFireApiConstants.FileHashHeader, FileHash } ,

                {MediaFireApiConstants.UnitHashHeader, unitHash } ,
                {MediaFireApiConstants.UnitIdHeader, unit.ToString() } ,
                {MediaFireApiConstants.UnitSizeHeader, unitSize.ToString() } ,
            };


            var parameters = new Dictionary<string, object>
            {
                //{MediaFireApiParameters.QuickKey, ""}      ,
                //{MediaFireApiParameters.ActionOnDuplicate, MediaFireActionOnDuplicate.Replace}               ,
                //{MediaFireApiParameters.FolderKey, ""}      ,
                ///etc
            };

            var contentToUpload = new MemoryStream(block, 0, unitSize);



            return (await _agent.PostStreamAsync<UploadResponse>(MediaFireApiUploadMethods.Resumable, contentToUpload, parameters, headers));


        }

        public void CalculateFileHash()
        {

            var fileHashAlg = new Sha256Digest();
            var block = new byte[_agent.Configuration.ChunkTransferBufferSize];
            var read = 0;
            do
            {
                read = _fileStream.Read(block, 0, block.Length);
                fileHashAlg.BlockUpdate(block, 0, read);
            } while (read != 0);

            FileHash = GetHashDigest(fileHashAlg);
            _fileStream.Seek(0, SeekOrigin.Begin);
        }

        private static string GetBlockHash(byte[] bytes, int length)
        {
            var algorithm = new Sha256Digest();
            algorithm.BlockUpdate(bytes, 0, length);
            return GetHashDigest(algorithm);
        }

        private static string GetHashDigest(Sha256Digest hash)
        {
            var digest = new byte[hash.GetDigestSize()];
            hash.DoFinal(digest, 0);
            return BitConverter.ToString(digest).Replace("-", string.Empty).ToLower();
        }
    }
}
