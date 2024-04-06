namespace MusicWebAppBackend.Infrastructure.ViewModels.PlayList
{
    public class UpdateSongToPlayListDto
    {
        public string IdPlayList { get; set; }
        public IList<string> IdSong { get; set; }
    }
}
