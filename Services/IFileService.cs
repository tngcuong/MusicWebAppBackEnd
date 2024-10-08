﻿using Azure.Storage;
using Azure.Storage.Blobs;
using MusicWebAppBackend.Infrastructure.Helpers;
using MusicWebAppBackend.Infrastructure.Models;

namespace MusicWebAppBackend.Services
{
    public interface IFileService
    {
        Task<string> SetAvatarDefault();
        Task<IFormFile> SetImage(IFormFile file, string id);
        Task<IFormFile> UploadMp3(IFormFile file, string userId);
    }

    public class FileService : IFileService
    {
        private static string _storageAccount = "musicwebapp";
        private static string _key = "cLH5vvofUvPIhFP6itdU6s4nzSaiFei7AvU1yVd89b2XzpqJ89ixXYIse441T+xWpaPZkQOsV/yv+AStPVD5nw==";
        private static BlobContainerClient _fileContainer;
        public FileService()
        {
        }

        public static BlobContainerClient connect(string blobname)
        {
            var credential = new StorageSharedKeyCredential(_storageAccount, _key);
            var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
            var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
            _fileContainer = blobServiceClient.GetBlobContainerClient(blobname);
            return _fileContainer;
        }

        public async Task<string> SetAvatarDefault()
        {
            var blobClient = connect("default");
            var blobUri = blobClient.Uri.ToString();
            return blobUri + "/user-icon-free-8.jpg";
        }

        public async Task<IFormFile> SetImage(IFormFile file, string id)
        {
            if (!Validator.IsImage(file))
            {
                return new EmptyFormFile();
            }
            try
            {
                var blobClient = connect(id);
                bool exists = await blobClient.ExistsAsync();

                if (!exists)
                {
                    await blobClient.CreateAsync();
                    await blobClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
                }

                string fileName = FunctionHelper.GenerateUniqueFileName(file);
                BlobClient client = blobClient.GetBlobClient(fileName);
                await using (Stream? data = file.OpenReadStream())
                {
                    await client.UploadAsync(data);
                }

                var renamedFile = new FormFile(file.OpenReadStream(), 0, file.Length, null, fileName);
                return renamedFile;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<IFormFile> UploadMp3(IFormFile file, string userId)
        {
            if (!Validator.IsMP3File(file))
            {
                return new EmptyFormFile();
            }

            if (file.Length > 10 * 1024 * 1024)
            {
                return new EmptyFormFile();
            }

            try
            {
                var blobClient = connect(userId);
                bool exists = await blobClient.ExistsAsync();

                if (!exists)
                {
                    await blobClient.CreateAsync();
                    await blobClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
                }
                string fileName = FunctionHelper.GenerateUniqueFileName(file);
                BlobClient client = blobClient.GetBlobClient(fileName);
                await using (Stream? data = file.OpenReadStream())
                {
                    await client.UploadAsync(data);
                }

                var renamedFile = new FormFile(file.OpenReadStream(), 0, file.Length, null, fileName);
                return renamedFile;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
