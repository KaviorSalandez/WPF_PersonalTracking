using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.DirectoryServices.ActiveDirectory;
using System.IO;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using WPF_PersonalTracking.DB;
using WPF_PersonalTracking.ViewModels;
using static System.Net.Mime.MediaTypeNames;

namespace WPF_PersonalTracking
{
    /// <summary>
    /// Interaction logic for EmployeePage.xaml
    /// </summary>
    public partial class EmployeePage : Window
    {
        public EmployeePage()
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
        PersonalTrackingContext db = new PersonalTrackingContext();
        // when change department combobox then comboboxPosition change
        List<Position> positions = new List<Position>();
        // for update
        public EmployeeModel model;
        private void Window_Loaded(object sender, RoutedEventArgs e)
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

            if (model != null && model.Id != 0)
            {
                cboDepartment.SelectedValue = model.DepartmentId;
                cboPosition.SelectedValue = model.PositionId;
                txtUserNo.Text = model.Userno.ToString();
                txtPassword.Text = model.Password;
                txtSurName.Text = model.Surname;
                txtSalary.Text = model.Salary.ToString();
                txtAddress.AppendText(model.Address);
                dpkBirthDay.SelectedDate = model.BirthDay;
                ckbIsAdmin.IsChecked = model.IsAdmin;
                txtName.Text = model.Name;
                ///ảnh 
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(@"images//" + model.ImagePath,UriKind.RelativeOrAbsolute);
                image.EndInit();
                EmployeeImage.Source = image;
            }
            if(UserStatic.IsAdmin == false)
            {
                ckbIsAdmin.IsEnabled = false;
                cboDepartment.IsEnabled = false;
                cboPosition.IsEnabled = false;
                txtSalary.IsEnabled = false;
                txtUserNo.IsEnabled = false;
            }
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        OpenFileDialog dialog = new OpenFileDialog();
        private void btnChoose_Click(object sender, RoutedEventArgs e)
        {
            if (dialog.ShowDialog() == true)
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(dialog.FileName);
                image.EndInit();
                EmployeeImage.Source = image;
                txtImage.Text = dialog.FileName;
            }
        }

        private void txtUserNo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]").IsMatch(e.Text);
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtUserNo.Text.Trim() == "" || txtPassword.Text.Trim() == "" || txtName.Text.Trim() == "" || txtSurName.Text.Trim() == "" || txtSalary.Text.Trim() == "" || cboDepartment.SelectedIndex == -1 || cboPosition.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill the nessary area");
            }
            else
            {

                if (model != null && model.Id != 0)
                {
                    Employee employee = db.Employees.Find(model.Id);
                    var UniqueListToCheckUserNo = db.Employees.Where(x => x.UserNo == Convert.ToInt32(txtUserNo.Text) && x.Id != employee.Id).ToList();
                    if (UniqueListToCheckUserNo.Count > 0)
                    {
                        MessageBox.Show("This userno is already used by another employee");
                    }
                    else
                    {

                        if (txtImage.Text != "")
                        {
                            // xóa file ảnh cũ trong bin debug images rồi cập nhật ảnh mới
                            if (File.Exists(@"images//" + employee.ImagePath))
                            {
                                File.Delete(@"images//" + employee.ImagePath);
                                string filename = "";
                                string UniqueName = Guid.NewGuid().ToString();
                                filename = UniqueName + System.IO.Path.GetFileName(txtImage.Text);
                                File.Copy(txtImage.Text, @"images//" + filename);
                                employee.ImagePath = filename;
                            }
                        }
                        employee.UserNo = Convert.ToInt32(txtUserNo.Text);
                        employee.Password = txtPassword.Text;
                        employee.Name = txtName.Text;
                        employee.Surname = txtSurName.Text;
                        employee.Salary = Convert.ToInt32(txtSalary.Text);
                        employee.DepartmentId = (int)cboDepartment.SelectedValue;
                        employee.PositonId = (int)cboPosition.SelectedValue;
                        TextRange txtRange = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd);
                        employee.Address = txtRange.Text;
                        employee.BirthDay = dpkBirthDay.SelectedDate;
                        employee.IsAdmin = ckbIsAdmin.IsChecked;
                        db.Update(employee);
                        db.SaveChanges();
                        MessageBox.Show("Employee was updated");
                    }
                }

                else
                {
                    //Button Check
                    // With each employee has a unique userno
                    var UniqueList = db.Employees.Where(x => x.UserNo == Convert.ToInt32(txtUserNo.Text)).ToList();
                    if (UniqueList.Count > 0)
                    {
                        MessageBox.Show("The unique user no is already used by another employee");
                    }
                    else
                    {
                        Employee emp = new Employee();

                        emp.UserNo = Convert.ToInt32(txtUserNo.Text);
                        emp.Password = txtPassword.Text;
                        emp.Name = txtName.Text;
                        emp.Surname = txtSurName.Text;
                        emp.Salary = Convert.ToInt32(txtSalary.Text);
                        emp.DepartmentId = (int)cboDepartment.SelectedValue;
                        emp.PositonId = (int)cboPosition.SelectedValue;
                        TextRange txtRange = new TextRange(txtAddress.Document.ContentStart, txtAddress.Document.ContentEnd);
                        emp.Address = txtRange.Text;
                        emp.BirthDay = dpkBirthDay.SelectedDate;
                        emp.IsAdmin = ckbIsAdmin.IsChecked;
                        //hãy nhớ rằng chúng ta phải có hình ảnh trong một thư mục, chúng ta sẽ chỉ giữ tên của chúng trong bảng của mình   
                        string fileName = "";
                        string Unique = Guid.NewGuid().ToString();
                        fileName += Unique + dialog.SafeFileName;
                        //Trong ví dụ này, khi người dùng chọn một tệp,
                        //SafeFileName sẽ chứa tên của tệp đó (ví dụ: "document.txt") mà không bao gồm đường dẫn (ví dụ: "C:\Documents\document.txt")
                        emp.ImagePath = fileName;
                        db.Employees.Add(emp);
                        db.SaveChanges();

                        //First, we'll need a folder named images and in order to be able to easily access
                        //And in order for it to just be the general path for all computers, we'll just add the folder to the
                        //application start folder open object in Folder Explorer Bin Debug.

                        File.Copy(txtImage.Text, @"images//" + fileName);
                        MessageBox.Show("Employee was added");


                        txtName.Clear();
                        txtUserNo.Clear();
                        txtSurName.Clear();
                        txtSalary.Clear();
                        txtPassword.Clear();
                        txtImage.Clear();
                        cboDepartment.SelectedIndex = -1;
                        dpkBirthDay.SelectedDate = DateTime.Today;
                        cboPosition.ItemsSource = positions; cboPosition.SelectedIndex = -1;
                        txtAddress.Document.Blocks.Clear();
                        ckbIsAdmin.IsChecked = false;
                        EmployeeImage.Source = new BitmapImage();
                        txtImage.Clear();


                    }
                }


            }
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Button Check
            // With each employee has a unique userno
            var UniqueList = db.Employees.Where(x => x.UserNo == Convert.ToInt32(txtUserNo.Text)).ToList();
            if (UniqueList.Count > 0)
            {
                MessageBox.Show("The unique user no is already used by another employee");
            }
            else
            {
                MessageBox.Show("This userno is available");
            }
        }
    }
}
