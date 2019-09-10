using System.Reactive;
using ReactiveUI;

namespace ListViewExample
{
    public class ClientItemViewModel : ViewModelBase
    {
        private readonly ReactiveCommand<Unit, Unit> _createUserCommand;
        private string _name;
        private int _id;

        public ClientItemViewModel(ClientsViewModel view)
        {
            _createUserCommand = ReactiveCommand.CreateFromTask(async () => await view.CreateUser(this));
        }

        public int Id { get => _id; set => this.RaiseAndSetIfChanged(ref _id, value); }
        public string Name { get => _name; set => this.RaiseAndSetIfChanged(ref _name, value); }
        public ReactiveCommand<Unit,Unit> CreateUserCommand => _createUserCommand;
    }
}