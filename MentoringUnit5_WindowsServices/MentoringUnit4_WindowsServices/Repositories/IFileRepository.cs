namespace MentoringUnit4_WindowsServices.Repositories
{
  public interface IFileRepository
  {
    void MoveFile(string sourceDirectory, string fileName);
  }
}