using ProjectManagementSystemWPF.Models;
using System.Windows;
using System.Windows.Controls;

namespace ProjectManagementSystem
{
    public class ProjectTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? ProjectTemplate { get; set; }
        public DataTemplate? AddButtonTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            if (item is Project project)
            {
                return project.IsAddButton ? AddButtonTemplate : ProjectTemplate;
            }

            return base.SelectTemplate(item, container);
        }
    }
}
