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
    /// Interaction logic for PositionList.xaml
    /// </summary>
    public partial class PositionList : UserControl
    {
        public PositionList()
        {
            InitializeComponent();
        }
        PersonalTrackingContext db = new PersonalTrackingContext();
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FillGrid();

        }
        void FillGrid()
        {
            var list = db.Positions.Include(x => x.Department).Select(a => new PositionModel
            {
                Id = a.Id,
                PositionName = a.PositionName,
                DepartmentId = a.DepartmentId,
                DepartmentName = a.Department.DepartmentName
            }).OrderBy(x => x.PositionName).ToList();
            gridPosition.ItemsSource = list;

        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            PositionPage page = new PositionPage();
            page.ShowDialog();
            FillGrid();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            PositionModel model = (PositionModel)gridPosition.SelectedItem;
            if(model != null && model.Id != 0)
            {
                PositionPage page = new PositionPage();
                page.model = model;
                page.ShowDialog();
                FillGrid();
            }
        }
        // if want delete position, we have to delete employees who have this position
        // => create new trigger for position
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            PositionModel model = (PositionModel)gridPosition.SelectedItem;
            if (model != null && model.Id != 0)
            {
                if (MessageBox.Show("Are you sure to delete?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    Position position = db.Positions.Find(model.Id);
                    db.Positions.Remove(position);
                    db.SaveChanges();
                    MessageBox.Show("Position was deleted");
                    FillGrid();
                }
            }
        }
    }
}
