using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using DownloadManager.Helpers;
using DownloadManager.Services;

namespace DownloadManager.MainWindowModule
{
  public class MainWindowViewModel : INotifyPropertyChanged
  {
    private readonly DownloadService _downloadService = new DownloadService();
    private CancellationTokenSource _cancellationTokenSource;

    private string _sourceAddress;
    public string SourceAddress
    {
      get => _sourceAddress;
      set {
        _sourceAddress = value;
        OnPropertyChanged();
      }
    }

    private string _content;
    public string Content
    {
      get => _content;
      set {
        _content = value;
        OnPropertyChanged();
      }
    }

    private bool _isDownloadingInProgress;
    public bool IsDownloadingInProgress
    {
      get => _isDownloadingInProgress;
      set {
        _isDownloadingInProgress = value;
        OnPropertyChanged();
      }
    }

    private RelayCommand _cancelCommand;
    public RelayCommand CancelCommand => _cancelCommand ?? (_cancelCommand = new RelayCommand(obj => _cancellationTokenSource?.Cancel()));

    private RelayCommand _downloadCommand;
    public RelayCommand DownloadCommand => _downloadCommand ?? (_downloadCommand = new RelayCommand(this.DownloadCommandAction));

    private RelayCommand _downloadSyncCommand;
    public RelayCommand DownloadSyncCommand => _downloadSyncCommand ?? (_downloadSyncCommand = new RelayCommand(this.DownloadSyncCommandAction));

    public async void DownloadCommandAction(object obj)
    {
      IsDownloadingInProgress = true;
      _cancellationTokenSource = new CancellationTokenSource();
      this.Content = "Loading...";

      this.Content = await _downloadService.DownloadAsync(this.SourceAddress, _cancellationTokenSource.Token);

      IsDownloadingInProgress = false;

      MessageBox.Show("Operation has been completed.", "Done.", MessageBoxButton.OK);
    }

    public void DownloadSyncCommandAction(object obj)
    {
      IsDownloadingInProgress = true;
      this.Content = "Loading...";

      this.Content = _downloadService.Download(this.SourceAddress);

      IsDownloadingInProgress = false;

      MessageBox.Show("Operation has been completed.", "Done.", MessageBoxButton.OK);
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName]string prop = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
  }
}