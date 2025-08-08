using System.Windows;

namespace ProjectManagementSystem
{
    public partial class AddProjectWindow : Window
    {
        public string ProjectName { get; private set; } = "";
        public string ProjectDescription { get; private set; } = "";

        public AddProjectWindow()
        {
            InitializeComponent();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            ProjectName = ProjectNameBox.Text.Trim();
            ProjectDescription = ProjectDescriptionBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(ProjectName) || string.IsNullOrWhiteSpace(ProjectDescription))
            {
                MessageBox.Show("Please fill in both the project name and description.");
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
