using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Alarm_Manager.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;

namespace Alarm_Manager.ViewModel
{
   
    public class ExceptionsViewModel : ViewModelBase ,IDataErrorInfo
    {
        
        private readonly IDataService _dataService;
        private ObservableCollection<Exception> exceptions ;
        private Exception _selectedException;
        private DateTime _singleExceptioDate ;
        private string _singleExceptionName;
        private Schedule _singleExceptionSchedule;
        private string _singleExceptionDescription;
       
        
        public RelayCommand ReadAllCommand { get; set; }
       
        public RelayCommand AddNewExceptionCommand { get; set; }
        public RelayCommand DeleteExceptioCommand { get; set; }
        public RelayCommand ShowAddNewExceptionCommand { get; set; }
        public RelayCommand HideAddNewExceptionCommand { get; set; }

        public string Error
        {
            get { return string.Empty; }
        }

        public string this[string columnName]
        {
            get
            {
                if (columnName == "SingleExceptionName")
                {
                    if (string.IsNullOrEmpty(this.SingleExceptionName))
                    {
                        return "Please Enter A Name";
                    }
                   
                }
                return string.Empty;
            }
        }
        public string SingleExceptionDescription
        {
            get
            {
                return _singleExceptionDescription;
                
            }
            set
            {
                _singleExceptionDescription = value;
                RaisePropertyChanged("SingleExceptionDescription");
            }
        }

        public string SingleExceptionName
        {
            get { return _singleExceptionName; }
            set
            {
                _singleExceptionName = value;
                RaisePropertyChanged("SingleExceptionName");
            }
        }

        public DateTime SingleExceptioDate
        {
            get { return _singleExceptioDate; }
            set
            {
                _singleExceptioDate = value;
                RaisePropertyChanged("SingleExceptioDate");
            }
        }

        public Schedule SingleExceptionSchedule
        {
            get { return _singleExceptionSchedule; }
            set
            {
                _singleExceptionSchedule = value;
                RaisePropertyChanged("SingleExceptionSchedule");
            }
        }
        public ObservableCollection<Schedule> Schedules { get; set; }

     
           
           
        
        public ExceptionsViewModel(IDataService dataService)
        {
            _dataService = dataService;
            exceptions = _dataService.GetExceptions();
           AddNewExceptionCommand=new RelayCommand(AddNewException);
            Schedules = (new ViewModelLocator()).Schedules.Schedules;
            DeleteExceptioCommand=new RelayCommand(DeleteException);
            SingleExceptioDate = DateTime.Now;
            ShowAddNewExceptionCommand=new RelayCommand(ShowNewException);
            HideAddNewExceptionCommand = new RelayCommand(HideNewException);
      
        }

        private void ShowNewException()
        {
            ((new ViewModelLocator()).Main.NewExceptionFlyoutStatus) = true;
        }
        private void HideNewException()
        {
            ((new ViewModelLocator()).Main.NewExceptionFlyoutStatus) = false;
        }
        private async void AddNewException()
        {
          var exception= new Exception
          {  
              Name = SingleExceptionName,
              Description = SingleExceptionDescription,
              Date = SingleExceptioDate,
              Schedule1 = SingleExceptionSchedule
          };


            bool error = false;
            try
            {
                _dataService.AddException(exception);
                exceptions.Add(exception);
                
            }
            catch (System.Exception e)
            {
                error = true;

            }
             if(error)
             {
                 var mainwindow = (Application.Current.MainWindow as MainWindow);



                 await mainwindow.ShowMessageAsync("Sorry!", "A problem has accured trying to add the new exception.");
             }
                       
            SingleExceptionName = "";
            SingleExceptionDescription = "";
            SingleExceptioDate = SingleExceptioDate.AddDays(1);
            SingleExceptionSchedule = null;

            (new ViewModelLocator()).Main.NewExceptionFlyoutStatus = false;
            (new ViewModelLocator()).Main.StartScheduler();


        }

        private void DeleteException()
        {
           _dataService.DeleteException(SelectedException);
        
           (new ViewModelLocator()).Main.StartScheduler();

          
        }
     
        public ObservableCollection<Exception> Exceptions
        {
            get
            {

                return exceptions;
            }
            set
            {
                exceptions = value;
                RaisePropertyChanged("Exceptions");
            }

        }

        public Exception SelectedException
        {
            get { return _selectedException; }
            set{
                _selectedException = value;
                RaisePropertyChanged("SelectedException");
            }
        }
    }
}