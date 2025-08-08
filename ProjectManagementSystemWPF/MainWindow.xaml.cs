using Newtonsoft.Json;
using ProjectManagementSystemWPF.Models;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using Task = System.Threading.Tasks.Task;

namespace ProjectManagementSystem
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<Project> projects = new ObservableCollection<Project>();
        private readonly string apiUrl = "http://localhost:5152/api/projects";

        public MainWindow()
        {
            InitializeComponent();
            LoadProjectsFromApi();
        }

        private async void LoadProjectsFromApi()
        {
            projects.Clear();

            var fetchedProjects = await GetProjectsAsync();

            foreach (var project in fetchedProjects)
            {
                project.IsAddButton = false;
                projects.Add(project);
            }

            projects.Add(new Project { IsAddButton = true });

            ProjectItemsControl.ItemsSource = projects;
        }

        public async Task<List<Project>> GetProjectsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync(apiUrl);

                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"API isteği başarısız. Status code: {response.StatusCode}");
                        return new List<Project>();
                    }

                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<Project>>(jsonString);
                }
                catch (HttpRequestException httpEx)
                {
                    MessageBox.Show($"HTTP Hatası: {httpEx.Message}");
                    return new List<Project>();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Beklenmeyen Hata: {ex.Message}");
                    return new List<Project>();
                }
            }
        }

        private async Task AddProject(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
                return;

            using (HttpClient client = new HttpClient())
            {
                var newProject = new Project { Name = name, Description = description };
                var content = new StringContent(JsonConvert.SerializeObject(newProject), System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    LoadProjectsFromApi();
                }
                else
                {
                    MessageBox.Show("Proje eklenemedi. Hata: " + response.StatusCode);
                }
            }
        }

        private async void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddProjectWindow();
            bool? result = addWindow.ShowDialog();

            if (result == true)
            {
                string name = addWindow.ProjectName;
                string desc = addWindow.ProjectDescription;

                await AddProject(name, desc);
            }
        }

        private void ProjectBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Project project)
            {
                if (project.IsAddButton)
                    return;

                ProjectDetailsWindow detailsWindow = new ProjectDetailsWindow(project);
                bool? result = detailsWindow.ShowDialog();

                if (result == true)
                {
                    LoadProjectsFromApi();
                }
            }
        }
    }
}
