using System.Windows;

namespace ProjectManagementSystemWPF
{
    public partial class AddTaskWindow : Window
    {
        public string TaskName { get; private set; } = "";
        public string TaskDescription { get; private set; } = "";

        public AddTaskWindow()
        {
            InitializeComponent();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            TaskName = TaskNameBox.Text.Trim();
            TaskDescription = TaskDescriptionBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(TaskName) || string.IsNullOrWhiteSpace(TaskDescription))
            {
                MessageBox.Show("Please fill in both the task name and description.");
                return;
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
