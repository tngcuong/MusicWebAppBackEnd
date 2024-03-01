using Azure.Storage.Blobs;
using Azure.Storage;
using System.Reflection.Metadata;
using System.ComponentModel;
using Microsoft.IdentityModel.Tokens;
using MusicWebAppBackend.Infrastructure.Helpers;
using MusicWebAppBackend.Infrastructure.ViewModels;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.Models.Const;

namespace MusicWebAppBackend.Services
{
    public interface IFileService
    {
        Task<string> SetAvatarDefault();
        Task<bool> SetAvatar(IFormFile file,string id);
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
            return blobUri+ "/user-icon-free-8.jpg";
        }

        public async Task<bool> SetAvatar(IFormFile file, string id)
        {
            if (!Validator.IsImage(file))
            {
                return false;
            }

            var blobClient = connect(id);
            bool exists = await blobClient.ExistsAsync();

            if(!exists)
            { 
                await blobClient.CreateAsync();
                await blobClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
            }

            BlobClient client = blobClient.GetBlobClient(file.FileName);
            await using (Stream? data = file.OpenReadStream())
            {
                await client.UploadAsync(data);
            }

            return true;
        }
    }
}
