using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace WPF_PersonalTracking
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Window
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        PersonalTrackingContext db = new PersonalTrackingContext();
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            if(txtUserno.Text.Trim() ==""||txtPassword.Text.Trim() =="")
            {
                MessageBox.Show("Please fill the userno and password areas");
            }
            else
            {
                Employee employee = db.Employees.FirstOrDefault(x => x.UserNo == Convert.ToInt32(txtUserno.Text) && x.Password.Equals(txtPassword.Text));
                if(employee != null && employee.Id != 0)
                {
                    this.Visibility = Visibility.Collapsed;
                    MainWindow main = new MainWindow();
                    // khi login tthif lưu thông tin người dùng vào 1 lớp tĩnh kiểu như sesssion trong web
                    UserStatic.EmployeeId = employee.Id;
                    UserStatic.IsAdmin = employee.IsAdmin;
                    UserStatic.Name = employee.Name;
                    UserStatic.Surname = employee.Surname;
                    UserStatic.Userno = employee.UserNo; 
                    main.ShowDialog();
                    // khi bấm vào logout thì main biến mất và hiện lại form login
                    txtUserno.Clear();
                    txtPassword.Clear();
                    this.Visibility = Visibility.Visible;
                }
                else
                {
                    MessageBox.Show("Please make sure that your password and userno is true");
                }
            }
        }

        private void txtUserno_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(txtUserno.Text);
        }
    }
}
