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
using System.Windows.Shapes;
using WPF_PersonalTracking.DB;
using WPF_PersonalTracking.ViewModels;

namespace WPF_PersonalTracking
{
    /// <summary>
    /// Interaction logic for SalaryPage.xaml
    /// </summary>
    public partial class SalaryPage : Window
    {
        public SalaryPage()
        {
            InitializeComponent();
        }

        // trước khi khởi tạo Page phải có windowloaded để load các combo box lên
        PersonalTrackingContext db = new PersonalTrackingContext();
        List<Position> positions = new List<Position>();
        List<Employee> employees = new List<Employee>();
        int EmployeeId = 0;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            employees = db.Employees.ToList();
            gridEmployee.ItemsSource = employees;

            cboDepartment.ItemsSource = db.Departments.ToList();
            cboDepartment.DisplayMemberPath = "DepartmentName";
            cboDepartment.SelectedValuePath = "Id";
            cboDepartment.SelectedIndex = -1;
            positions = db.Positions.ToList();
            cboPosition.ItemsSource = positions;
            cboPosition.DisplayMemberPath = "PositionName";
            cboPosition.SelectedValuePath = "Id";
            cboPosition.SelectedIndex = -1;
            List<Month> months = db.Months.ToList();
            cboMonth.ItemsSource = months;
            cboMonth.DisplayMemberPath = "MonthName";
            cboMonth.SelectedValuePath = "Id";
            cboMonth.SelectedIndex = -1;
            txtYear.Text = DateTime.Now.Year.ToString();
            if (model != null && model.Id != 0)
            {
                txtName.Text = model.Name;
                txtSurname.Text = model.Surname;
                txtUserNo.Text = model.UserNo.ToString();
                txtSalary.Text = model.Amount.ToString();
                txtYear.Text = model.Year.ToString();
                cboMonth.SelectedValue = model.MonthId;
                EmployeeId = model.EmployeeId;
            }


        }

        private void gridEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Employee emp = (Employee)gridEmployee.SelectedItem;
            txtUserNo.Text = emp.UserNo.ToString();
            txtSurname.Text = emp.Surname;
            txtName.Text = emp.Name;
            txtYear.Text = DateTime.Now.Year.ToString();
            txtSalary.Text = emp.Salary.ToString();
            EmployeeId = emp.Id;
        }

        private void cboDepartment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gridEmployee.ItemsSource = employees.Where(x => x.DepartmentId == Convert.ToInt32(cboDepartment.SelectedValue)).ToList();
            int DepartmentId = Convert.ToInt32(cboDepartment.SelectedValue);
            if (cboDepartment.SelectedIndex != -1)
            {
                cboPosition.ItemsSource = positions.Where(x => x.DepartmentId == DepartmentId).ToList();
                cboPosition.DisplayMemberPath = "PositionName";
                cboPosition.SelectedValuePath = "Id";
                cboPosition.SelectedIndex = -1;
            }
        }

        private void cboPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gridEmployee.ItemsSource = employees.Where(x => x.PositonId == Convert.ToInt32(cboPosition.SelectedValue)).ToList();
        }
        public SalaryModel model;
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(txtSalary.Text.Trim() == "" || txtYear.Text.Trim() == "" || cboMonth.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill the nessary areas");
            }
            else
            {
                if(model != null && model.Id != 0)
                {
                    //update
                    Salary update = db.Salaries.Find(model.Id);
                    int oldSalary = update.Amount;
                    update.Amount = Convert.ToInt32(txtSalary.Text);
                    update.EmployeeId = EmployeeId;
                    update.Year = Convert.ToInt32(txtYear.Text);
                    update.MonthId = Convert.ToInt32(cboMonth.SelectedValue);
                    db.Update(update);
                    db.SaveChanges();
                    if(oldSalary < update.Amount)
                    {
                        Employee emp = db.Employees.Find(EmployeeId);
                        emp.Salary = update.Amount;
                        db.SaveChanges();
                    }
                    MessageBox.Show("Salary was updated");


                }
                else
                {
                    //add
                    if(EmployeeId == 0)
                    {
                        MessageBox.Show("Please select an employees from table");
                    }
                    else
                    {
                        Salary salary = new Salary();
                        salary.EmployeeId = EmployeeId;
                        salary.Amount = Convert.ToInt32(txtSalary.Text);
                        salary.MonthId = Convert.ToInt32(cboMonth.SelectedValue);
                        salary.Year = Convert.ToInt32(txtYear.Text);
                        db.Add(salary);
                        db.SaveChanges();
                        MessageBox.Show("Salary was added");
                        EmployeeId = 0;
                        txtName.Clear();
                        txtYear.Text = DateTime.Now.Year.ToString();
                        txtSalary.Clear();
                        txtSurname.Clear();
                        txtUserNo.Clear();
                        cboMonth.SelectedIndex = -1;
                        gridEmployee.ItemsSource = employees;
                        cboDepartment.SelectedIndex = -1;
                        cboPosition.ItemsSource = positions;
                        cboPosition.SelectedIndex = -1;
                    }
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
