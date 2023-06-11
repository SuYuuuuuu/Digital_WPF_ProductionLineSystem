using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WpfProductionLineApp.ViewModels
{
    public partial class ControlViewModel : ObservableObject
    {

        public ControlViewModel()
        {
            ControlPageName = "/Views/RobotControl_Jog.xaml";
            ChangeControlPageCmd = new RelayCommand<string>(ChangePage);
        }

        [ObservableProperty]
        private bool _jogPage_isChecked;
        [ObservableProperty]
        private bool _ptpPage_isChecked;

        private RelayCommand<string> _changeControlPageCmd;
        public RelayCommand<string> ChangeControlPageCmd
        {
            get { return _changeControlPageCmd; }
            set { SetProperty(ref _changeControlPageCmd, value); }
        }

        private string _controlPageName;
        public string ControlPageName
        {
            get { return _controlPageName; }
            set { SetProperty(ref _controlPageName, value); }
        }



        private void ChangePage(string obj)
        {
            if (string.IsNullOrEmpty(obj)) return;
            ControlPageName = obj;
            switch (ControlPageName)
            {
                case "/Views/RobotControl_PTP.xaml":
                    _ptpPage_isChecked = true; break;
                case "/Views/RobotControl_Jog.xaml":
                    _jogPage_isChecked = true; break;
            }
        }


    }
}
