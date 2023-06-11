using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfProductionLineApp.Models
{
    public class TextModel : ObservableObject
    {
        /* ----------------修饰坐标的文字属性------------*/
        private string? _cordText_1;
        public string? CordText_1
        {
            get { return _cordText_1; }
            set { SetProperty(ref _cordText_1, value); }
        }

        private string? _cordText_2;
        public string? CordText_2
        {
            get { return _cordText_2; }
            set { SetProperty(ref _cordText_2, value); }
        }

        private string? _cordText_3;
        public string? CordText_3
        {
            get { return _cordText_3; }
            set { SetProperty(ref _cordText_3, value); }
        }

        private string? _cordText_4;
        public string? CordText_4
        {
            get { return _cordText_4; }
            set { SetProperty(ref _cordText_4, value); }
        }
    }
}
