using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_PersonalTracking.DB;

namespace WPF_PersonalTracking.Views
{
    /// <summary>
    /// Interaction logic for DepartmentList.xaml
    /// </summary>
    public partial class DepartmentList : UserControl
    {
        public DepartmentList()
        {
            InitializeComponent();
            // khởi tạo list department to show
            using (PersonalTrackingContext context = new PersonalTrackingContext())
            {
                List<Department> list = context.Departments.OrderBy(x=>x.Id).ToList();
                gridDepartment.ItemsSource = list;
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            DepartmentPage departmentPage = new DepartmentPage();
            departmentPage.ShowDialog();
            using (PersonalTrackingContext context = new PersonalTrackingContext())
            {
                List<Department> list = context.Departments.OrderBy(x => x.Id).ToList();
                gridDepartment.ItemsSource = list;
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //First, we have to take the values from the data grid.
            Department dpm =(Department) gridDepartment.SelectedItem;
            // Get form add department
            DepartmentPage page = new DepartmentPage();
            page.deparment = dpm;
            page.ShowDialog();
            using (PersonalTrackingContext context = new PersonalTrackingContext())
            {
                List<Department> list = context.Departments.OrderBy(x => x.Id).ToList();
                gridDepartment.ItemsSource = list;
            }
        }
        PersonalTrackingContext db = new PersonalTrackingContext();
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Department model  = (Department) gridDepartment.SelectedItem;
            if (model != null && model.Id != 0)
            {
                if (MessageBox.Show("Are you sure to delete?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    List<Position> positions = db.Positions.Where(x=>x.DepartmentId == model.Id).ToList() ;
                    foreach (var item in positions)
                    {
                        db.Positions.Remove(item);
                    }
                    db.SaveChanges();
                    Department department = db.Departments.Find(model.Id);
                    db.Departments.Remove(department);
                    db.SaveChanges();
                    MessageBox.Show("Deapartment was deleted");
                    gridDepartment.ItemsSource = db.Departments.ToList();
                   
                }
            }

        }
    }
}
