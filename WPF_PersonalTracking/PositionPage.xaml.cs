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
    /// Interaction logic for PositionPage.xaml
    /// </summary>
    public partial class PositionPage : Window
    {
        public PositionPage()
        {
            InitializeComponent();
        }
        PersonalTrackingContext db = new PersonalTrackingContext();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var listDep = db.Departments.OrderBy(x=>x.Id).ToList();
            cboDepartment.ItemsSource = listDep;
            cboDepartment.DisplayMemberPath = "DepartmentName";
            cboDepartment.SelectedValuePath = "Id";
            cboDepartment.SelectedIndex = -1;
            if(model != null && model.Id != 0)
            {
                txtPositionName.Text = model.PositionName;
                cboDepartment.SelectedValue = model.DepartmentId;
            }
        }
        void ClearField()
        {
            txtPositionName.Clear();
            cboDepartment.SelectedIndex = -1;
        }
        public PositionModel model;
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cboDepartment.SelectedIndex == -1 || txtPositionName.Text.Trim() == "")
            {
                MessageBox.Show("Please fill all areas");
            }
            else
            {
                if (model != null && model.Id != 0)
                {
                    //update
                    Position update = new Position();
                    update.Id = model.Id;
                    update.DepartmentId = (int)cboDepartment.SelectedValue;
                    update.PositionName = txtPositionName.Text;
                    db.Update(update);
                    db.SaveChanges();
                    MessageBox.Show("Position was updated");
                }
                else
                {
                    //add new
                    Position p = new Position();
                    p.PositionName = txtPositionName.Text;
                    p.DepartmentId = Convert.ToInt32(cboDepartment.SelectedValue);
                    db.Positions.Add(p);
                    db.SaveChanges();
                    ClearField();
                    MessageBox.Show("Position was added");
                }
              
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
