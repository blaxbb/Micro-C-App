using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {

        bool isBusy = false;
        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        string title = string.Empty;
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public ICommand ShowActions { get; }
        protected virtual Dictionary<string, ICommand> Actions => new Dictionary<string, ICommand>()
        {
        };

        const string CANCEL_TEXT = "Cancel";
        public BaseViewModel()
        {
            ShowActions = new Command<object>(async (object param) =>
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {

                    var result = await Shell.Current.DisplayActionSheet("Actions", CANCEL_TEXT, null, Actions.Keys.ToArray());
                    if (Actions.ContainsKey(result) && result != CANCEL_TEXT)
                    {
                        Actions[result].Execute(param);
                    }
                });
            });
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
