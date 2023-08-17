using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace WPF_PersonalTracking.Views
{
    /// <summary>
    /// Interaction logic for EmployeeList1.xaml
    /// </summary>
    public partial class EmployeeList1 : UserControl
    {
        public EmployeeList1()
        {
            InitializeComponent();
        }
        PersonalTrackingContext db = new PersonalTrackingContext();
        List<Position> positions = new List<Position>();
        List<EmployeeModel> listE = new List<EmployeeModel>();
        void FillGrid()
        {
            cboDepartment.ItemsSource = db.Departments.ToList();
            cboDepartment.DisplayMemberPath = "DepartmentName";
            cboDepartment.SelectedValuePath = "Id";
            cboDepartment.SelectedIndex = -1;
            positions = db.Positions.ToList();
            cboPosition.ItemsSource = positions;
            cboPosition.DisplayMemberPath = "PositionName";
            cboPosition.SelectedValuePath = "Id";
            cboPosition.SelectedIndex = -1;
            listE = db.Employees.Include(x => x.Positon).Include(x => x.Department).Select(x => new EmployeeModel
            {
                Id = x.Id,
                Name = x.Name,
                Surname = x.Surname,
                Address = x.Address,
                DepartmentName = x.Department.DepartmentName,
                PositionName = x.Positon.PositionName,
                DepartmentId = x.DepartmentId,
                PositionId = x.PositonId,
                Password = x.Password,
                Userno = x.UserNo,
                Salary = x.Salary,
                IsAdmin = x.IsAdmin,
                BirthDay = x.BirthDay,
                ImagePath = x.ImagePath,
            }).ToList();
            gridEmployee.ItemsSource = listE;

        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            EmployeePage formCreateUpdate = new EmployeePage();
            formCreateUpdate.ShowDialog();
            FillGrid();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FillGrid();
        }

        private void cboDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int DepartmentId = Convert.ToInt32(cboDepartment.SelectedValue);
            if (cboDepartment.SelectedIndex != -1)
            {
                cboPosition.ItemsSource = positions.Where(x => x.DepartmentId == DepartmentId).ToList();
                cboPosition.DisplayMemberPath = "PositionName";
                cboPosition.SelectedValuePath = "Id";
                cboPosition.SelectedIndex = -1;
            } 
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            List<EmployeeModel> listSearch = listE;
            if(txtUserNo.Text.Trim() != "")
            {
                listSearch = listE.Where(x => x.Userno == Convert.ToInt32(txtUserNo.Text)).ToList();
            }
            if(txtName.Text.Trim() != "")
            {
                listSearch = listE.Where(x => x.Name.ToLower().Contains(txtName.Text.ToLower())).ToList();
            }
            if (txtSurname.Text.Trim() != "")
            {
                listSearch = listE.Where(x => x.Surname.ToLower().Contains(txtSurname.Text.ToLower())).ToList();
            }
            if(cboDepartment.SelectedIndex != -1)
            {
                listSearch = listE.Where(x => x.DepartmentId == (int)cboDepartment.SelectedValue).ToList();
            }
            if (cboPosition.SelectedIndex != -1)
            {
                listSearch = listE.Where(x => x.PositionId == (int)cboPosition.SelectedValue).ToList();
            }
            gridEmployee.ItemsSource = listSearch;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtName.Clear();
            txtSurname.Clear();
            txtUserNo.Clear();
            cboDepartment.SelectedIndex = -1;
            cboPosition.ItemsSource = positions;
            cboPosition.SelectedIndex = -1;
            gridEmployee.ItemsSource = listE;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            EmployeeModel employeeModel = (EmployeeModel) gridEmployee.SelectedItem;
            EmployeePage page = new EmployeePage();
            page.model = employeeModel;
            page.ShowDialog();
            //after update fill data grid again
            FillGrid();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            EmployeeModel model = (EmployeeModel)gridEmployee.SelectedItem;

            if (model != null && model.Id != 0)
            {
                if (MessageBox.Show("Are you sure to delete?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    //// Sau khi xóa employee đó thì cần xóa cả những task của họ trước
                    //List<Task> tasks = db.Tasks.Where(x => x.EmployeeId == model.Id).ToList();
                    //foreach (Task task in tasks)
                    //{
                    //    db.Tasks.Remove(task);
                    //}
                    //// xóa cả những permission  của employee đó
                    //List<Permission> permissions = db.Permissions.Where(x => x.EmployeeId == model.Id).ToList();
                    //foreach (Permission item in permissions)
                    //{
                    //    db.Permissions.Remove(item);
                    //}
                    //// xóa lương của nhân viên đó
                    //List<Salary> salaries = db.Salaries.Where(x => x.EmployeeId == model.Id).ToList();
                    //foreach (Salary item in salaries)
                    //{
                    //    db.Salaries.Remove(item);
                    //}
                    //db.SaveChanges();

                    //=> ko cần nữa vì sử dụng trigger
                    Employee emp = db.Employees.Find(model.Id);
                    db.Employees.Remove(emp);
                    db.SaveChanges();
                    MessageBox.Show("Employee was deleted");
                    FillGrid();
                }
            }
        }
    }
}
