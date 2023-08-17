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
    /// Interaction logic for PermissionPage.xaml
    /// </summary>
    public partial class PermissionPage : Window
    {
        public PermissionPage()
        {
            InitializeComponent();
        }
        //tính toán khoảng thời gian cấp phép   
        TimeSpan permissionDay = new TimeSpan();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtUserno.Text = UserStatic.Userno.ToString();
            dpkStart.SelectedDate = DateTime.Today;
            // nếu được chọn thì update và hiện thị thông tin trên form 
            if(model != null && model.Id != 0)
            {
                txtUserno.Text = model.Userno.ToString();
                dpkStart.SelectedDate = model.StartDate;
                dpkEnd.SelectedDate = model.EndDate;    
                txtDayAmout.Text = model.DayAmount.ToString();  
                txtExplaination.Text  = model.Explaination.ToString();  
            }

        }
        public PermissionModel model;
        private void dpkStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if(dpkEnd.SelectedDate != null) {
                permissionDay = (TimeSpan) (dpkEnd.SelectedDate - dpkStart.SelectedDate);
                txtDayAmout.Text = permissionDay.TotalDays.ToString();
            }
        }

        private void dpkEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpkStart.SelectedDate != null)
            {
                permissionDay = (TimeSpan)(dpkEnd.SelectedDate - dpkStart.SelectedDate);
                txtDayAmout.Text = permissionDay.TotalDays.ToString();
            }
        }
        PersonalTrackingContext db = new PersonalTrackingContext();

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if(txtDayAmout.Text.Trim() == "")
            {
                MessageBox.Show("Please select start and end Date");
            }
            else if (Convert.ToInt32(txtDayAmout.Text) <= 0)
            {
                MessageBox.Show("Permission day must be bigger than zero");
            }
            else if(txtExplaination.Text.Trim() == "")
            {
                MessageBox.Show("Please write your permission reason");
            }
            else
            {
                if(model != null && model.Id !=0) {
                    Permission permission = db.Permissions.Find(model.Id);
                    permission.PermissionExplaination = txtExplaination.Text;
                    permission.PermissionStartDate =(DateTime) dpkStart.SelectedDate;
                    permission.PermisstionEndDate = (DateTime) dpkEnd.SelectedDate;
                    permission.PermissionDay = Convert.ToInt32(txtDayAmout.Text);
                    db.Permissions.Update(permission);
                    db.SaveChanges();
                    MessageBox.Show("Permission was updated");
                }
                else
                {
                    Permission p = new Permission();
                    p.EmployeeId = UserStatic.EmployeeId;
                    p.PermissionState = DefinitionStatics.PermissionState.OnEmployee;
                    p.PermissionStartDate = (DateTime)dpkStart.SelectedDate;
                    p.PermisstionEndDate = (DateTime)dpkEnd.SelectedDate;
                    p.PermissionDay = Convert.ToInt32(txtDayAmout.Text);
                    p.PermissionExplaination = txtExplaination.Text;
                    db.Permissions.Add(p);
                    db.SaveChanges();
                    MessageBox.Show("Permission was added");
                    dpkEnd.SelectedDate = DateTime.Today;
                    dpkStart.SelectedDate = DateTime.Today; txtDayAmout.Clear(); txtExplaination.Clear();
                }
               
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
      
    }
}
