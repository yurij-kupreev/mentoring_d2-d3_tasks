namespace DocumentCaptureService.Repositories
{
  public interface IFileRepository
  {
    void MoveFile(string sourceDirectory, string fileName);
  }
}