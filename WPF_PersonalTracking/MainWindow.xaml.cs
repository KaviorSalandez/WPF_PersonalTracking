using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF_PersonalTracking.DB;
using WPF_PersonalTracking.ViewModels;

namespace WPF_PersonalTracking
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            lbWindowName.Content = "Task List";
            DataContext = new TaskViewModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(UserStatic.IsAdmin==false)
            {
                stackDepartment.Visibility = Visibility.Hidden;
                stackPosition.Visibility = Visibility.Hidden;
                stackLogout.SetValue(Grid.RowProperty,5);
                stackExit.SetValue(Grid.RowProperty, 6);
            }
        }

        private void btnDepartment_Click(object sender, RoutedEventArgs e)
        {
            lbWindowName.Content = "Department List";
            DataContext =  new DepartmentViewModel();
        }

        private void btnPosition_Click(object sender, RoutedEventArgs e)
        {
            lbWindowName.Content = "Position List";
            DataContext = new PositionViewModel();
        }

        private void btnEmployee_Click(object sender, RoutedEventArgs e)
        {
           
            if(UserStatic.IsAdmin == false)
            {
                PersonalTrackingContext db = new PersonalTrackingContext();
                //string test = UserStatic.EmployeeId.ToString();
                //MessageBox.Show(test);
                Employee employee = db.Employees.Find(UserStatic.EmployeeId);
                EmployeeModel employeeModel = new EmployeeModel();
             
                employeeModel.Name = employee.Name;
                employeeModel.Userno = employee.UserNo;
                employeeModel.IsAdmin = employee.IsAdmin;
                employeeModel.ImagePath = employee.ImagePath;
                employeeModel.Password = employee.Password;
                employeeModel.DepartmentId = employee.DepartmentId;
                employeeModel.PositionId = employee.PositonId;
                employeeModel.Salary = employee.Salary;
                employeeModel.BirthDay = employee.BirthDay;
                employeeModel.Address = employee.Address;
                employeeModel.Surname = employee.Surname;
                employeeModel.Id = employee.Id;
                EmployeePage page = new EmployeePage();
                page.model = employeeModel;
                page.ShowDialog();
            }
            else
            {
                lbWindowName.Content = "Employee List";
                DataContext = new EmployeeViewModel();
            }
        }

        private void btnTask_Click(object sender, RoutedEventArgs e)
        {
            lbWindowName.Content = "Task List";
            DataContext = new TaskViewModel();
        }

        private void btnSalary_Click(object sender, RoutedEventArgs e)
        {
            lbWindowName.Content = "Salary List";
            DataContext = new SalaryViewModel();
        }

        private void btnPermission_Click(object sender, RoutedEventArgs e)
        {
            lbWindowName.Content = "Permission List";
            DataContext = new PermissionViewModel();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
