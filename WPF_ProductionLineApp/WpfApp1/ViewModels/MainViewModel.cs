using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace WpfProductionLineApp.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public MainViewModel()
        {
            PageName = "/Views/RobotConnect.xaml";
            ChangePageCmd = new RelayCommand<string>(ChangePage);
        }

        private string _pagename;

        public string PageName
        {
            get { return _pagename; }
            set { SetProperty(ref _pagename, value); }
        }

        private RelayCommand<string> _changePageCmd;

        public RelayCommand<string> ChangePageCmd
        {
            get { return _changePageCmd; }
            set { SetProperty(ref _changePageCmd, value); }
        }


        private void ChangePage(string obj)
        {
            if (string.IsNullOrEmpty(obj)) return;
            PageName = obj;
        }

    }
}
