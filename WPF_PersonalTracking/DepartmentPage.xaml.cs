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

namespace WPF_PersonalTracking
{
    /// <summary>
    /// Interaction logic for DepartmentPage.xaml
    /// </summary>
    public partial class DepartmentPage : Window
    {
        public DepartmentPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        // tạo ra một thực thể Department để giữ giá trị của Department mình chọn
        public Department deparment;
        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtDepartmentName.Text.Trim() == "")
            {
                MessageBox.Show("Please fill the department name area");
            }
            else {
                using (PersonalTrackingContext context  = new PersonalTrackingContext())
                {
                    if(deparment != null && deparment.Id != 0)
                    {
                        //Update
                        Department update = new Department();
                        update.DepartmentName = txtDepartmentName.Text;
                        update.Id = deparment.Id;
                        context.Update(update);
                        context.SaveChanges();
                        txtDepartmentName.Clear();
                        MessageBox.Show("Update was succesfull");
                    }
                    else
                    {
                        // create
                        Department d = new Department();
                        d.DepartmentName = txtDepartmentName.Text;
                        context.Add(d);
                        context.SaveChanges();
                        txtDepartmentName.Clear();
                        MessageBox.Show("Department was added");

                        //redirect to list view
                    }
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(deparment != null && deparment.Id !=0)
            {
                txtDepartmentName.Text = deparment.DepartmentName;
            }
        }
    }
}
