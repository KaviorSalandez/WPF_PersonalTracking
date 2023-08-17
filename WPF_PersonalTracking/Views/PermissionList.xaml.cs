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
    /// Interaction logic for PermissionList.xaml
    /// </summary>
    public partial class PermissionList : UserControl
    {
        public PermissionList()
        {
            InitializeComponent();
        }
        PersonalTrackingContext db = new PersonalTrackingContext();
        List<PermissionModel> listmodel  = new List<PermissionModel>(); 
        List<Position> positions = new List<Position>();   
        PermissionModel model = new PermissionModel();
        void FillData()
        {
            listmodel = db.Permissions.Include(x => x.Employee).Include(x => x.PermissionStateNavigation).Select(x=>new PermissionModel()
            {
                EmployeeId = x.EmployeeId, PermissionState = x.PermissionState,Surname = x.Employee.Surname,Name = x.Employee.Name, Userno = x.Employee.UserNo,
                StartDate = x.PermissionStartDate,EndDate = x.PermisstionEndDate,StateName = x.PermissionStateNavigation.StateName,DepartmentId = x.Employee.DepartmentId,
                PositionId= x.Employee.PositonId,
                Explaination = x.PermissionExplaination,
                DayAmount = x.PermissionDay,
                Id = x.Id
            }).OrderByDescending(x=>x.StartDate).ToList();

            if(UserStatic.IsAdmin == false)
            {
                listmodel = listmodel.Where(x => x.EmployeeId == UserStatic.EmployeeId).ToList();
                txtUserno.IsEnabled = false;
                txtSurname.IsEnabled = false;
                txtName.IsEnabled = false;
                cboDepartment.IsEnabled = false;
                cboPosition.IsEnabled = false;
                btnDelete.Visibility = Visibility.Hidden;
                btnApprove.Visibility = Visibility.Hidden;
                btnDisapprove.Visibility = Visibility.Hidden;
                btnAdd.SetValue(Grid.ColumnProperty,1);
            }
      
            gridPermission.ItemsSource = listmodel;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            PermissionPage page = new PermissionPage();
            page.ShowDialog();
            FillData();
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FillData();
            cboDepartment.ItemsSource = db.Departments.ToList();
            cboDepartment.DisplayMemberPath = "DepartmentName";
            cboDepartment.SelectedValuePath = "Id";
            cboDepartment.SelectedIndex = -1;
            positions = db.Positions.ToList();
            cboPosition.ItemsSource = positions;
            cboPosition.DisplayMemberPath = "PositionName";
            cboPosition.SelectedValuePath = "Id";
            cboPosition.SelectedIndex = -1;

            List<Permissionstate> permissionstates = db.Permissionstates.ToList();
            cboState.ItemsSource   = permissionstates;
            cboState.DisplayMemberPath = "StateName";
            cboState.SelectedValuePath = "Id";
            cboState.SelectedIndex =-1;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            List<PermissionModel> search = listmodel;
            if (txtUserno.Text.Trim() != "")
            {
                search = search.Where(x => x.Userno == Convert.ToInt32(txtUserno.Text)).ToList();
            }
            if (txtName.Text.Trim() != "")
            {
                search = search.Where(x => x.Name.ToLower().Contains(txtName.Text.ToLower())).ToList();
            }
            if (txtSurname.Text.Trim() != "")
            {
                search = search.Where(x => x.Surname.ToLower().Contains(txtSurname.Text.ToLower())).ToList();
            }
            if (cboDepartment.SelectedIndex != -1)
            {
                search = search.Where(x => x.DepartmentId == (int)cboDepartment.SelectedValue).ToList();
            }
            if (cboPosition.SelectedIndex != -1)
            {
                search = search.Where(x => x.PositionId == (int)cboPosition.SelectedValue).ToList();
            }
            if(radioStart.IsChecked == true)
            {
                search = search.Where(x => x.StartDate > dpkStart.SelectedDate && x.StartDate < dpkEnd.SelectedDate).ToList();
            }
            if (radioEnd.IsChecked == true)
            {
                search = search.Where(x => x.EndDate > dpkStart.SelectedDate && x.EndDate < dpkEnd.SelectedDate).ToList();
            }
            if(cboState.SelectedIndex != -1)
            {
                search = search.Where(x => x.PermissionState == (int)cboState.SelectedValue).ToList();
            }
            if(txtDayAmount.Text.Trim() != "")
            {
                search = search.Where(x=>x.DayAmount== Convert.ToInt32(txtDayAmount.Text)).ToList();
            }
            gridPermission.ItemsSource= search; 
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtDayAmount.Clear();
            txtSurname.Clear();
            txtUserno.Clear();
            txtName.Clear();
            radioEnd.IsChecked = false;
            radioStart.IsChecked = false;
            cboDepartment.SelectedIndex = -1;
            cboPosition.ItemsSource = positions;
            cboPosition.SelectedIndex = -1;
            cboState.SelectedIndex = -1;
            dpkEnd.SelectedDate = DateTime.Today;
            dpkStart.SelectedDate = DateTime.Today;
            gridPermission.ItemsSource = listmodel;
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

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (model != null && model.Id != 0)
            {
                PermissionPage page = new PermissionPage();
                page.model = model;
                page.ShowDialog();
                FillData();
            }
            else
            {
                MessageBox.Show("Please select permission from Table");
            }
        }

        private void gridPermission_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            model = (PermissionModel) gridPermission.SelectedItem;
        }

        private void btnDisapprove_Click(object sender, RoutedEventArgs e)
        {
            if (model != null && model.Id != 0 && model.PermissionState == DefinitionStatics.PermissionState.OnEmployee)
            {
                Permission permission = db.Permissions.Find(model.Id);
                permission.PermissionState = DefinitionStatics.PermissionState.Disapproved;
                db.SaveChanges();
                MessageBox.Show("Permission was disapproved");
                FillData();
            }
        }

        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            if(model != null&& model.Id != 0 && model.PermissionState == DefinitionStatics.PermissionState.OnEmployee)
            {
                Permission permission = db.Permissions.Find(model.Id);
                permission.PermissionState = DefinitionStatics.PermissionState.Approved;
                db.SaveChanges();
                MessageBox.Show("Permission was approved");
                FillData();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (model!= null && model.Id != 0)
            {
                if (MessageBox.Show("Are you sure to delete?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning)==MessageBoxResult.Yes)
                {
                    Permission permission = db.Permissions.Find(model.Id);
                    db.Permissions.Remove(permission);
                    db.SaveChanges();
                    MessageBox.Show("Permission was deleted");
                    FillData();
                }
            }
        }
    }
}
