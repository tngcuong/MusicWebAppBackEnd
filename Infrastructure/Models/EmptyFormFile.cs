namespace MusicWebAppBackend.Infrastructure.Models
{
    public class EmptyFormFile : IFormFile
    {
        public string ContentType => "";

        public string ContentDisposition => throw new NotImplementedException();

        public IHeaderDictionary Headers => throw new NotImplementedException();

        public long Length => 0;

        public string Name => "";

        public string FileName => "";

        public void CopyTo(Stream target)
        {
            throw new NotImplementedException();
        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Stream OpenReadStream()
        {
            throw new NotImplementedException();
        }
    }
}
