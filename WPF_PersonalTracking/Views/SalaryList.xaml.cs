using Microsoft.EntityFrameworkCore;
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

namespace WPF_PersonalTracking.Views
{
    /// <summary>
    /// Interaction logic for SalaryList.xaml
    /// </summary>
    public partial class SalaryList : UserControl
    {
        public SalaryList()
        {
            InitializeComponent();
        }
        PersonalTrackingContext db = new PersonalTrackingContext();
        List<Position> positions = new List<Position>();
        List<SalaryModel> list = new List<SalaryModel>();
        SalaryModel model = new SalaryModel();  // lưu dữ liệu để gửi vào biến model trong form update
        void DataGridFill()
        {
            list = db.Salaries.Include(x => x.Employee).Include(x => x.Month).Select(x => new SalaryModel()
            {
                UserNo = x.Employee.UserNo,
                Name = x.Employee.Name,
                Amount = x.Amount,
                EmployeeId = x.EmployeeId,
                Id = x.Id,
                MonthId = x.MonthId,
                MonthName = x.Month.MonthName,
                Surname = x.Employee.Surname,
                Year = x.Year,
                DepartmentId = x.Employee.DepartmentId,
                PositionId = x.Employee.PositonId,

            }).OrderByDescending(x => x.Year).OrderByDescending(x => x.MonthId).ToList();

           
            if(UserStatic.IsAdmin == false)
            {
                list = list.Where(x => x.EmployeeId == UserStatic.EmployeeId).ToList();
                // với người dùng cũng kko cần các tiêu chí tìm như của admin
                txtUserno.IsEnabled = false;
                txtSurname.IsEnabled = false;
                txtName.IsEnabled = false;
                cboDepartment.IsEnabled = false;
                cboPosition.IsEnabled = false;
                // đối với người dùng thì họ ko thể add,update,delete lương của họ được mà chỉ xem được thôi
                btnAdd.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Hidden;
                btnDelete.Visibility = Visibility.Hidden;
            }


            gridSalary.ItemsSource = list;
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            SalaryPage page = new SalaryPage();
            page.ShowDialog();
            DataGridFill();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DataGridFill();
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

        private void cboPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //ko cần trong search
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            List<SalaryModel> search = list;
            if (txtUserno.Text.Trim() != "")
            {
                search = search.Where(x => x.UserNo == Convert.ToInt32(txtUserno.Text)).ToList();
            }
            if (txtName.Text.Trim() != "")
            {
                search = search.Where(x => x.Name.ToLower().Contains(txtName.Text.Trim().ToLower())).ToList();
            }
            if (txtSurname.Text.Trim() != "")
            {
                search = search.Where(x => x.Surname.ToLower().Contains(txtSurname.Text.Trim().ToLower())).ToList();
            }
            if (cboDepartment.SelectedIndex != -1)
            {
                search = search.Where(x => x.DepartmentId == Convert.ToInt32(cboDepartment.SelectedValue)).ToList();
            }
            if (cboPosition.SelectedIndex != -1)
            {
                search = search.Where(x => x.PositionId == Convert.ToInt32(cboPosition.SelectedValue)).ToList();
            }
            if (txtYear.Text.Trim() != "")
            {
                search = search.Where(x => x.Year == Convert.ToInt32(txtYear.Text)).ToList();
            }
            if (cboMonth.SelectedIndex != -1)
            {
                search = search.Where(x => x.MonthId == Convert.ToInt32(cboMonth.SelectedValue)).ToList();
            }
            if (txtSalary.Text.Trim() != "")
            {
                if(radioMore.IsChecked== true)
                {
                    search = search.Where(x => x.Amount > Convert.ToInt32(txtSalary.Text)).ToList();
                }else if(radioEqual.IsChecked== true) {
                    search = search.Where(x => x.Amount == Convert.ToInt32(txtSalary.Text)).ToList();
                }
                else
                {
                    search = search.Where(x => x.Amount < Convert.ToInt32(txtSalary.Text)).ToList();
                }
            }
            gridSalary.ItemsSource = search;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtName.Clear();
            txtYear.Text = DateTime.Now.Year.ToString();
            txtSalary.Clear();
            txtSurname.Clear();
            txtUserno.Clear();
            cboMonth.SelectedIndex = -1;
            gridSalary.ItemsSource = list;
            cboDepartment.SelectedIndex = -1;
            cboPosition.ItemsSource = positions;
            cboPosition.SelectedIndex = -1;
            radioEqual.IsChecked = false;
            radioLess.IsChecked = false;
            radioMore.IsChecked = false;
        }

        private void gridSalary_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            model = (SalaryModel)gridSalary.SelectedItem;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            SalaryModel model = (SalaryModel)gridSalary.SelectedItem;
            SalaryPage page = new SalaryPage();
            page.model = model;
            page.ShowDialog();
            DataGridFill();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Are you sure to delete", "Question", MessageBoxButton.YesNo,MessageBoxImage.Warning)==MessageBoxResult.Yes)
            {
                if(model.Id != 0)
                {
                    SalaryModel model1 = (SalaryModel)gridSalary.SelectedItem;
                    Salary s = db.Salaries.Find(model1.Id);
                    db.Salaries.Remove(s);
                    db.SaveChanges();
                    MessageBox.Show("Salary was deleted");
                    DataGridFill();
                }

            }
        }
    }
}
