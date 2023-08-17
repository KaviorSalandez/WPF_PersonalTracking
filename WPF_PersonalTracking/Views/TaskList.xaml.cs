using Microsoft.EntityFrameworkCore;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xaml;
using WPF_PersonalTracking.DB;
using WPF_PersonalTracking.ViewModels;

namespace WPF_PersonalTracking.Views
{
    /// <summary>
    /// Interaction logic for TaskList.xaml
    /// </summary>
    public partial class TaskList : UserControl
    {
        public TaskList()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            TaskPage page = new TaskPage();
            page.ShowDialog();
            FillDataGird();
        }
        PersonalTrackingContext db = new PersonalTrackingContext();
        List<TaskModel> list = new List<TaskModel>();
        List<TaskModel> listSearch = new List<TaskModel>();
        List<Position> positions = new List<Position>();
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FillDataGird();
            if(UserStatic.IsAdmin == false)
            {
                btnAdd.Visibility = Visibility.Hidden;
                btnUpdate.Visibility = Visibility.Hidden;
                btnDelete.Visibility = Visibility.Hidden;
                btnApprove.SetValue(Grid.ColumnProperty,1);
            }
        }
        void FillDataGird()
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

            List<Taskstate> taskstates = db.Taskstates.ToList();
            cboState.ItemsSource = taskstates;
            cboState.DisplayMemberPath = "StateName";
            cboState.SelectedValuePath = "Id";
            cboState.SelectedIndex =-1;
            // vì nó sẽ trả ra 1 list task nên ta cần convert nó sang taskmodel

            list = db.Tasks.Include(x => x.TaskStateNavigation).Include(x => x.Employee).ThenInclude(x => x.Department).ThenInclude(x => x.Positions).Select(x => new TaskModel()
            {
                Id = x.Id,
                EmployeeId = x.EmployeeId,
                Name = x.Employee.Name,
                Surname = x.Employee.Surname,
                TaskContent = x.TaskContent,
                TaskTitle = x.TaskTitle,
                TaskStartDate = (DateTime)x.TaskStartDate,
                TaskDeliveryDate = x.TaskDeliveryDate,
                TaskState = (int)x.TaskState,
                DepartmentId = x.Employee.DepartmentId,
                PositionId = x.Employee.PositonId,
                StateName = x.TaskStateNavigation.StateName,
                UserNo =x.Employee.UserNo,

            }).ToList();
            if(UserStatic.IsAdmin == false)
            {
                list = list.Where(x => x.EmployeeId == UserStatic.EmployeeId).ToList();
                txtUserno.IsEnabled = false;
                txtName.IsEnabled = false;
                txtSurName.IsEnabled = false;
                cboDepartment.IsEnabled = false;
                cboPosition.IsEnabled = false;
                btnApprove.Content = "Delivery";
            }
            gridTask.ItemsSource = list;
            listSearch = list;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            List<TaskModel> search = listSearch;
            if(txtUserno.Text.Trim() != "")
            {
                search = search.Where(x=>x.UserNo==Convert.ToInt32(txtUserno.Text)).ToList();
            }
            if (txtName.Text.Trim() != "")
            {
                search = search.Where(x => x.Name.ToLower().Contains(txtName.Text.Trim().ToLower())).ToList();
            }
            if (txtSurName.Text.Trim() != "")
            {
                search = search.Where(x => x.Surname.ToLower().Contains(txtSurName.Text.Trim().ToLower())).ToList();
            }
            if(cboDepartment.SelectedIndex != -1)
            {
                search = search.Where(x => x.DepartmentId == Convert.ToInt32(cboDepartment.SelectedValue)).ToList();
            }
            if (cboPosition.SelectedIndex != -1)
            {
                search = search.Where(x => x.PositionId == Convert.ToInt32(cboPosition.SelectedValue)).ToList();
            }
            if(cboState.SelectedIndex != -1) {
                search = search.Where(x => x.TaskState == Convert.ToInt32(cboState.SelectedValue)).ToList();
            }
            if(radioStartDate.IsChecked == true)
            {
                search = search.Where(x => x.TaskStartDate > dpkStart.SelectedDate && x.TaskStartDate < dpkEnd.SelectedDate).ToList();
            }
            if(radioDeliveryDate.IsChecked == true)
            {
                search = search.Where(x => x.TaskDeliveryDate > dpkStart.SelectedDate && x.TaskDeliveryDate < dpkEnd.SelectedDate).ToList();

            }
            gridTask.ItemsSource = search;
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

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            txtName.Clear();
            txtSurName.Clear(); 
            txtUserno.Clear();
            dpkStart.SelectedDate = DateTime.Now;
            dpkEnd.SelectedDate = DateTime.Now;
            cboDepartment.SelectedIndex = -1;
            cboState.SelectedIndex = -1;
            cboPosition.ItemsSource = positions;
            cboPosition.SelectedIndex = -1;
            radioStartDate.IsChecked = false;
            radioDeliveryDate.IsChecked = false;
            gridTask.ItemsSource = list;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            TaskPage page = new TaskPage();
            page.model = model;
            page.ShowDialog();
            FillDataGird();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure to delete", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (model.Id != 0)
                {
                    TaskModel selectedModel = (TaskModel)gridTask.SelectedItem;
                    Task deleteTask = db.Tasks.Find(selectedModel.Id);
                    db.Tasks.Remove(deleteTask);
                    db.SaveChanges();
                    MessageBox.Show("Task was deleted");
                    FillDataGird();
                }

            }
        }
        TaskModel model = new TaskModel();  
        private void gridTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            model= (TaskModel)gridTask.SelectedItem;

        }

        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            // một người dùng tiêu chuẩn thì bàn giao lại nhiệm vụ khi người đó đã hoàn thành task
            if(model != null && model.Id != 0) {
                //kiểm tra xem người dùng đó có phải admin ko , nếu phải thì check xem nhiệm vụ đó được bàn giao chưa để duyệt approve
                if(UserStatic.IsAdmin==true && model.TaskState == DefinitionStatics.StateTask.OnEmployee)
                {
                    // nếu nhiệm vụ này chưa được nộp sau khi hoàn thành thì thông báo
                    MessageBox.Show("Before approve a task, Task must be Delivered");
                }
                else if(UserStatic.IsAdmin == true && model.TaskState == DefinitionStatics.StateTask.Approved)
                {
                    MessageBox.Show("This task is already approved");
                }
                // nếu ko phải là admin 
                else if (UserStatic.IsAdmin == false && model.TaskState == DefinitionStatics.StateTask.Delivered)
                {
                    MessageBox.Show("This task is already delivered");
                }
                else if (UserStatic.IsAdmin == false && model.TaskState == DefinitionStatics.StateTask.Approved)
                {
                    MessageBox.Show("This task is already approved");
                }
                else
                {
                    Task task = db.Tasks.Find(model.Id);
                    if(UserStatic.IsAdmin== true)
                    {
                        task.TaskState = DefinitionStatics.StateTask.Approved;
                    }
                    else
                    {
                        task.TaskState = DefinitionStatics.StateTask.Delivered;
                    }
                    db.SaveChanges();
                    MessageBox.Show("Task was uppdated");
                    FillDataGird();
                }
            }

                  
        }
    }
}
