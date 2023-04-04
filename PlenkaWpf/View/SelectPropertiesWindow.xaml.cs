using PlenkaAPI.Models;

using PlenkaWpf.VM;


namespace PlenkaWpf.View
{
    /// <summary>
    ///     Interaction logic for SelectPropertiesWindow.xaml
    /// </summary>
    public partial class SelectPropertiesWindow
    {
        public SelectPropertiesWindow(ObjectType ot)
        {
            InitializeComponent();
            var vm = new SelectPropertiesWindowVM(ot);
            DataContext = vm;
            vm.ClosingRequest += (sender, e) => Close();
        }
    }
}
