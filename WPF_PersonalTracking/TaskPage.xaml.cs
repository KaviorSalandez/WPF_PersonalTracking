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
using System.Windows.Shapes;
using WPF_PersonalTracking.DB;
using WPF_PersonalTracking.ViewModels;

namespace WPF_PersonalTracking
{
    /// <summary>
    /// Interaction logic for TaskPage.xaml
    /// </summary>
    public partial class TaskPage : Window
    {
        public TaskPage()
        {
            InitializeComponent();
        }
        PersonalTrackingContext db = new PersonalTrackingContext();
        List<Employee> employeeList = new List<Employee>();
        List<Position> positions = new List<Position>();
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public TaskModel model;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            employeeList = db.Employees.OrderBy(x => x.Name).ToList();
            gridEmployee.ItemsSource = employeeList;

            cboDepartment.ItemsSource = db.Departments.ToList();
            cboDepartment.DisplayMemberPath = "DepartmentName";
            cboDepartment.SelectedValuePath = "Id";
            cboDepartment.SelectedIndex = -1;


            positions = db.Positions.ToList();
            cboPosition.ItemsSource = positions;
            cboPosition.DisplayMemberPath = "PositionName";
            cboPosition.SelectedValuePath = "Id";
            cboPosition.SelectedIndex = -1;

            if (model != null && model.Id != 0)
            {
                txtUserNo.Text = model.UserNo.ToString();
                txtName.Text = model.Name.ToString();
                txtSurname.Text = model.Surname.ToString();
                txtTitle.Text = model.TaskTitle.ToString();
                txtContent.Text = model.TaskContent.ToString();


            }
        }
        //defind employeeid
        private int EmployeeId = 0;
        private void gridEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Employee emp = (Employee)gridEmployee.SelectedItem;
            txtUserNo.Text = emp.UserNo.ToString();
            txtSurname.Text = emp.Surname;
            txtName.Text = emp.Name;
            EmployeeId = emp.Id;
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
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            if (txtTitle.Text.Trim() == "" || txtContent.Text.Trim() == "")
            {
                MessageBox.Show("Please fill the necessary areas");
            }
            else
            {
                if (model != null && model.Id != 0)
                {
                    //Update
                    Task update = db.Tasks.Find(model.Id);
                        update.EmployeeId = model.EmployeeId;
                        update.TaskContent = txtContent.Text;
                        update.TaskTitle = txtTitle.Text;
                        db.Update(update);
                        db.SaveChanges();
                        MessageBox.Show("Task was updated");


                        txtContent.Clear();
                        txtName.Clear();
                        txtTitle.Clear();
                        txtSurname.Clear();
                        txtUserNo.Clear();
                }
                else
                {
                    if (EmployeeId == 0)
                    {
                        MessageBox.Show("Please select an employee from table");
                    }
                    else
                    {
                        Task task = new Task();
                        task.EmployeeId = EmployeeId;
                        task.TaskStartDate = DateTime.Now;
                        task.TaskTitle = txtTitle.Text;
                        task.TaskContent = txtContent.Text;
                        task.TaskState = DefinitionStatics.StateTask.OnEmployee;
                        db.Tasks.Add(task);
                        db.SaveChanges();
                        MessageBox.Show("Task was added");
                        EmployeeId = 0;
                        // clear
                        txtContent.Clear();
                        txtName.Clear();
                        txtTitle.Clear();
                        txtSurname.Clear();
                        txtUserNo.Clear();
                    }

                }

            }
        }
    }
}
