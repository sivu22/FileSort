using System;

namespace FileSort
{
    public enum OutputMode
    {
        RenameOriginalFile,
        CopyNextToOriginalFile,
        CopyToSourceFolder
    }

    class Settings : BindableProperty
    {
        public class Defaults
        {
            public const string SourcePath = "";
            public const OutputMode SortMode = OutputMode.CopyToSourceFolder;
            public const string OutputPath = SourcePath + "\\FileSort";
        }

        private string sourcePath = Defaults.SourcePath;
        public string SourcePath { get => sourcePath; set => ChangeProperty(ref sourcePath, value, new string[] { nameof(OutputPath), nameof(CanRun) }); }

        private OutputMode sortMode = Defaults.SortMode;
        public OutputMode SortMode { get => sortMode; set => ChangeProperty(ref sortMode, value); }


        public string OutputPath => $"{SourcePath} + \\FileSort";

        public bool CanRun { get => !String.IsNullOrEmpty(SourcePath); }

        public Settings() { }

        public Settings(string sourcePath, OutputMode sortMode) => (SourcePath, SortMode) = (sourcePath, sortMode);
    }
}
