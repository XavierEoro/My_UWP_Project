using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPSlideItem
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ObservableCollection<Item> items = new ObservableCollection<Item>();

            for (var i = 0; i < 1000; i++)
            {
                items.Add(new Item() { Title = "Item " + i });
            }

            _items = items;
            this.DataContext = this;
        }

        public ObservableCollection<Item> _items { get; set; }

        private DelegateCommand<Item> _deleteItem = default(DelegateCommand<Item>);

        public DelegateCommand<Item> DeleteItem => _deleteItem ?? (_deleteItem = new DelegateCommand<Item>(ExecuteDeleteItemCommand, CanExecuteDeleteItemCommand));


        private bool CanExecuteDeleteItemCommand(Item item)
        {
            return true;
        }

        private void ExecuteDeleteItemCommand(Item item)
        {
            Debug.WriteLine("command executed.");
            _items.Remove(item);
        }

        private void listView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Debug.WriteLine("item clicked.");
        }
    }

    public class Item : BindableBase
    {
        private string _title = default(string);

        public string Title
        {
            get { return _title; }
            set { Set(ref _title, value); }
        }

        private bool? _isFavorite = default(bool);

        public bool? IsFavorite
        {
            get { return _isFavorite; }
            set { Set(ref _isFavorite, value); }
        }

        private DelegateCommand _toggleFavorite = default(DelegateCommand);

        public DelegateCommand ToggleFavorite => _toggleFavorite ?? (_toggleFavorite = new DelegateCommand(ExecuteToggleFavoriteCommand, CanExecuteToggleFavoriteCommand));

        private bool CanExecuteToggleFavoriteCommand()
        {
            return true;
        }

        private void ExecuteToggleFavoriteCommand()
        {
            IsFavorite = !IsFavorite;
        }
    }

    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Set<T>(ref T storage, T value, [CallerMemberName]string propertyName = null)
        {
            if (!Equals(storage, value))
            {
                storage = value;
                RaisePropertyChanged(propertyName);
            }
        }
    }

    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> commandExecuteAction;

        private readonly Func<T, bool> commandCanExecute;

        public DelegateCommand(Action<T> executeAction, Func<T, bool> canExecute = null)
        {
            if (executeAction == null)
            {
                throw new ArgumentNullException(nameof(executeAction));
            }

            commandExecuteAction = executeAction;
            commandCanExecute = canExecute ?? (e => true);
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// The parameter used by the command.
        /// </param>
        /// <returns>
        /// Returns a value indicating whether this command can be executed.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            try
            {
                return commandCanExecute(ConvertParameterValue(parameter));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        /// The parameter used by the command.
        /// </param>
        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            try
            {
                commandExecuteAction(ConvertParameterValue(parameter));
            }
            catch
            {
                Debugger.Break();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private static T ConvertParameterValue(object parameter)
        {
            parameter = parameter is T ? parameter : Convert.ChangeType(parameter, typeof(T));
            return (T)parameter;
        }
    }

    public class DelegateCommand : ICommand
    {
        private readonly Action commandExecuteAction;

        private readonly Func<bool> commandCanExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
        /// </summary>
        /// <param name="execute">
        /// The action to execute when called.
        /// </param>
        /// <param name="canExecute">
        /// The function to call to determine if the command can execute the action.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if the execute action is null.
        /// </exception>
        public DelegateCommand(Action execute, Func<bool> canExecute = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            commandExecuteAction = execute;
            commandCanExecute = canExecute ?? (() => true);
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter">
        /// The parameter used by the command.
        /// </param>
        /// <returns>
        /// Returns a value indicating whether this command can be executed.
        /// </returns>
        public bool CanExecute(object parameter = null)
        {
            try
            {
                return commandCanExecute();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Defines the method to be called when the command is invoked.
        /// </summary>
        /// <param name="parameter">
        /// The parameter used by the command.
        /// </param>
        public void Execute(object parameter)
        {
            if (!CanExecute(parameter))
            {
                return;
            }

            try
            {
                commandExecuteAction();
            }
            catch
            {
                Debugger.Break();
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
