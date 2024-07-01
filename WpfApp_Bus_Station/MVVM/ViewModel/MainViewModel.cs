using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp_Bus_Station.Model;

namespace WpfApp_Bus_Station.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand TicketsViewCommand { get; set; }
        public RelayCommand PassengersViewCommand { get; set; }
        public RelayCommand FlightsViewCommand { get; set; }
        public RelayCommand AboutViewCommand { get; set; }

        public HomeViewModel HomeVM { get; set; }
        public TicketsViewModel TicketsVM { get; set; }
        public PassengersViewModel PassengersVM { get; set; }
        public FlightsViewModel FlightsVM { get; set; }
        public AboutViewModel AboutVM { get; set; }


        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set { 
                _currentView = value;
                OnPropertyChanged();
            }
        }
           

        public MainViewModel()
        {
            HomeVM = new HomeViewModel();
            PassengersVM = new PassengersViewModel();
            TicketsVM = new TicketsViewModel();
            FlightsVM = new FlightsViewModel();
            AboutVM = new AboutViewModel();

            CurrentView = HomeVM;

            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeVM;
            });

            TicketsViewCommand = new RelayCommand(o =>
            {
                CurrentView = TicketsVM;
            });

            PassengersViewCommand = new RelayCommand(o =>
            {
                CurrentView = PassengersVM;
            });

            FlightsViewCommand = new RelayCommand(o =>
            {
                CurrentView = FlightsVM;
            });

            AboutViewCommand = new RelayCommand(o => 
            {
                CurrentView = AboutVM;
            });
        }
    }
}
