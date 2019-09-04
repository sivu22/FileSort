using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FileSort
{
    class Model: BindableProperty
    {
        public struct Item
        {
            public string Path { get; set; }
            public string Name { get; set; }
            public string Index { get; set; }
        }

        public Item[] Items { get; private set; }
        public ObservableCollection<Item> SortedItems { get; private set; }
        public Settings Settings { get; set; }

        private bool sorting = false;
        public bool Sorting { get => sorting; set => ChangeProperty(ref sorting, value, new string[] { nameof(RunText)  , nameof(NotSorting) }); }
        public bool NotSorting { get => !Sorting; }

        public string RunText { get => Sorting ? "Cancel" : "Run"; }

        private string status = "";
        public string Status { get => status; set => ChangeProperty(ref status, value); }

        public Model()
        {
            Settings = new Settings();
            SortedItems = new ObservableCollection<Item>();
        }

        public void CreateSortFolder()
        {
            if (Settings.SortMode != OutputMode.CopyToSourceFolder) return;

            var sortFolder = Settings.SourcePath + "\\FileSort";
            if (Directory.Exists(sortFolder)) return;

            try
            {
                Directory.CreateDirectory(sortFolder);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void FindAllFiles(CancellationToken? token = null)
        {
            var items = new List<Item>();

            try
            {
                var files = Directory.EnumerateFiles(Settings.SourcePath, "*.*", SearchOption.AllDirectories);
                token?.ThrowIfCancellationRequested();

                Item item = new Item();
                UInt32 index = 0;
                int namePos = 0;
                var formatSpecifier = "D" + files.Count().ToString().Length;
                foreach (string file in files)
                {
                    item.Index = index++.ToString(formatSpecifier);
                    namePos = file.LastIndexOf("\\");
                    item.Path = file.Substring(0, namePos + 1);
                    item.Name = file.Substring(namePos + 1);

                    items.Add(item);
                    token?.ThrowIfCancellationRequested();
                }
            }
            catch (Exception)
            {
                throw;
            }

            Items = items.ToArray();
            App.Current.Dispatcher.Invoke((Action)delegate
            {
                SortedItems.Clear();
            });
        }

        public void SortItem(Item item)
        {
            var currentFile = String.Format("{0}{1}", item.Path, item.Name);
            var newFile = "";
            if (Settings.SortMode != OutputMode.CopyToSourceFolder)
                newFile = String.Format("{0}{1}_{2}", item.Path, item.Index, item.Name);
            else
                newFile = String.Format("{0}\\FileSort\\{1}_{2}", Settings.SourcePath, item.Index, item.Name);

            try
            {
                if (Settings.SortMode == OutputMode.RenameOriginalFile)
                    Directory.Move(currentFile, newFile);
                else
                    File.Copy(currentFile, newFile, true);

                App.Current.Dispatcher.Invoke((Action)delegate
                {
                    SortedItems.Add(item);
                });
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
