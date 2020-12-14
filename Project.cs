using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WavePlayer
{
    class Project : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void SetPropertyAndNotifyChange<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            var jsonIgnoreAttribute = GetType().GetProperty(propertyName).GetCustomAttribute<JsonIgnoreAttribute>();
            if (jsonIgnoreAttribute == null)
            {
                HasUnsavedChanges = true;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public int Version { get; } = 1;

        private bool hasUnsavedChanges = false;
        [JsonIgnore]
        public bool HasUnsavedChanges
        {
            get => hasUnsavedChanges;
            set => SetPropertyAndNotifyChange(ref hasUnsavedChanges, value);
        }

        private string name = null;
        public string Name
        {
            get => name;
            set => SetPropertyAndNotifyChange(ref name, value);
        }

        private IReadOnlyList<string> scriptLines = new List<string>();
        [JsonIgnore]
        public IReadOnlyList<string> ScriptLines
        {
            get => scriptLines;
            private set => SetPropertyAndNotifyChange(ref scriptLines, value);
        }

        private string scriptPath = null;
        public string ScriptPath
        {
            get => scriptPath;
            set
            {
                SetPropertyAndNotifyChange(ref scriptPath, value);
                ScriptLines = File.ReadAllLines(scriptPath);
            }
        }


        public BindingList<string> WaveFolderPaths { get; } = new BindingList<string>()
        {
            AllowNew = true,
            AllowRemove = true,
            AllowEdit = false,
        };

        public Project() { }

        public Project(string name)
        {
            Name = name ?? throw new ArgumentNullException("name");
        }


        public const string ConfigExtension = ".wpproj";

        private static void CheckConfigPath(string configPath)
        {
            if (configPath == null)
            {
                throw new ArgumentNullException("projectConfigPath");
            }

            // The extension check is recommended by not required
            Debug.Assert(Path.GetExtension(configPath) == ConfigExtension);
        }

        public void Save(string configPath)
        {
            CheckConfigPath(configPath);

            var serializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            string configJsonString = JsonSerializer.Serialize(this, serializerOptions);
            File.WriteAllText(configPath, configJsonString);

            HasUnsavedChanges = false;
        }

        static public Project Load(string configPath)
        {
            CheckConfigPath(configPath);

            string configJsonString = File.ReadAllText(configPath);
            Project loadedProject = JsonSerializer.Deserialize<Project>(configJsonString);

            // the deserilizer will call setter to restore properties
            loadedProject.hasUnsavedChanges = false;

            return loadedProject;
        }
    }
}
