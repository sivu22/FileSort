using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FileSort
{
    class ViewModel: BindableProperty
    {
        private CancellationTokenSource cts;
        private readonly Progress<int> progressHandler;
        
        public Model Model { get; set; }

        public ICommand SortCommand { get; set; }
        private int sortProgress;
        public int SortProgress { get => sortProgress; set => ChangeProperty(ref sortProgress, value); }
        
        public ViewModel()
        {
            Model = new Model();

            progressHandler = new Progress<int>(value =>
            {
                SortProgress = value;
            });
            SortCommand = new RelayCommand(async param => await Sort(progressHandler as IProgress<int>), param => Model.Settings.CanRun);
        }

        public void UpdateSourcePath(string newPath)
        {
            Model.Settings.SourcePath = newPath;
        }

        private void SortComplete(bool canceled = false)
        {
            if (canceled) Model.Status = "";
            else Model.Status = "Done";
        }

        public async Task Sort(IProgress<int> progress)
        {
            if (!Model.Sorting)
            {
                Model.Sorting = true;
                Model.Status = "Preparing data...";
                SortProgress = 0;

                cts = new CancellationTokenSource();
                var token = cts.Token;
                bool canceled = false;

                try
                {
                    await Task.Run(() => Model.FindAllFiles(token));
                }
                catch (OperationCanceledException)
                {
                    canceled = true;
                }
                catch (Exception e)
                {
                    MessageBox.Show("Failed to search source path: " + e.Message, "FileSort", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (Model.Items.Length > 0)
                {
                    try
                    {
                        Model.CreateSortFolder();
                    }
                    catch (Exception e)
                    {
                        canceled = true;
                        MessageBox.Show($"Could not create FileSort folder: {e.Message}", "FileSort", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    if (!canceled)
                    {
                        var currentItem = 0;
                        var progressPercent = 0;
                        foreach (var item in Model.Items)
                        {
                            try
                            {
                                await Task.Run(() => Model.SortItem(item));

                                Model.Status = $"{++currentItem}/{Model.Items.Length}";
                                progressPercent = 100 * currentItem / Model.Items.Length;

                                progress?.Report(progressPercent);
                                token.ThrowIfCancellationRequested();
                            }
                            catch (OperationCanceledException)
                            {
                                canceled = true;
                                break;
                            }
                            catch (Exception e)
                            {
                                if (MessageBox.Show($"Failed to sort {item.Name}: {e.Message}\n\nContinue?", "FileSort", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.No)
                                {
                                    canceled = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No files found.", "FileSort", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Model.Sorting = false;
                cts = null;
                SortComplete(canceled);
            }
            else
            {
                Model.Sorting = false;
                if (cts != null) cts.Cancel();
            }
        }
    }
}
