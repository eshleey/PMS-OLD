using Newtonsoft.Json;
using ProjectManagementSystemWPF;
using ProjectManagementSystemWPF.Models;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Task = System.Threading.Tasks.Task;
using TaskModel = ProjectManagementSystemWPF.Models.Task;

namespace ProjectManagementSystem
{
    public partial class ProjectDetailsWindow : Window
    {
        private Project project;
        private ObservableCollection<TaskModel> tasks = new ObservableCollection<TaskModel>();
        private readonly string apiUrl = "http://localhost:5152/api/projects";

        public ProjectDetailsWindow(Project project)
        {
            InitializeComponent();
            this.project = project;
            DataContext = project;
            LoadTaskFromApi();
        }

        private async void LoadTaskFromApi()
        {
            tasks.Clear();

            var fetchedTasks = await GetTasksAsync();

            foreach (var task in fetchedTasks )
            {
                tasks.Add(task);
            }

            TasksListBox.ItemsSource = tasks;
        }

        public async Task<List<TaskModel>> GetTasksAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetAsync($"{apiUrl}/{project.Id}/tasks");

                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("An error occurred while loading tasks. Error: " + response.StatusCode);
                        return new List<TaskModel>();
                    }

                    var jsonString = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<TaskModel>>(jsonString);
                }
                catch (HttpRequestException httpEx)
                {
                    MessageBox.Show($"HTTP Hatası: {httpEx.Message}");
                    return new List<TaskModel>();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Beklenmeyen Hata: {ex.Message}");
                    return new List<TaskModel>();
                }
            }
        }

        private async Task DeleteProjectAsync(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.DeleteAsync($"{apiUrl}/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Project could not be deleted. Error: " + response.StatusCode);
                }
            }
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete this project?",
                                         "Confirm",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                await DeleteProjectAsync(project.Id);

                MessageBox.Show("Project deleted.", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);

                this.DialogResult = true;
                this.Close();
            }
        }

        private async Task UpdateProjectAsync(Project updatedProject)
        {
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(JsonConvert.SerializeObject(updatedProject), Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"{apiUrl}/{updatedProject.Id}", content);

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Project could not be updated. Error: " + response.StatusCode);
                }
            }
        }

        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            await UpdateProjectAsync(project);

            MessageBox.Show("Project updated.", "Successful", MessageBoxButton.OK, MessageBoxImage.Information);

            this.DialogResult = true;
            this.Close();
        }

        private async Task AddTaskAsync(string name, string description, int projectId)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
                return;

            using (HttpClient client = new HttpClient())
            {
                var newTask = new TaskModel { Name = name, Description = description, ProjectId = projectId };
                var content = new StringContent(JsonConvert.SerializeObject(newTask), Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"http://localhost:5152/api/tasks/{projectId}/tasks", content);

                if (response.IsSuccessStatusCode)
                {
                    LoadTaskFromApi();
                }
                else
                {
                    MessageBox.Show("Görev eklenemedi. Hata: " + response.StatusCode);
                }
            }
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddTaskWindow();
            bool? result = addWindow.ShowDialog();

            if (result == true)
            {
                string name = addWindow.TaskName;
                string description = addWindow.TaskDescription;
                int projectId = project.Id;

                await AddTaskAsync(name, description, projectId);
            }
        }

        private void TasksListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TasksListBox.SelectedItem is TaskModel selectedTask)
            {
                var detailsWindow = new TaskDetailsWindow(selectedTask);
                bool? result = detailsWindow.ShowDialog();

                if (result == true)
                {
                    LoadTaskFromApi();
                }

                TasksListBox.SelectedItem = null;
            }
        }

    }
}
