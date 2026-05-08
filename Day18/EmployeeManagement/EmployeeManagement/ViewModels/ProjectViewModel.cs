//шаблок из задания
using System.Collections.ObjectModel;
using System.Windows.Input;
using EmployeeManagement.Commands;
using EmployeeManagement.Interfaces;

namespace EmployeeManagement.ViewModels
{
    /// <summary>
    /// Универсальный ViewModel по шаблону из задания.
    /// </summary>
    public class ProjectViewModel<T> : BaseViewModel where T : new()
    {
        private readonly IRepository<T> _repo;
        private T? _selectedItem;

        public ObservableCollection<T> Items { get; } = new();

        public T? SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public ICommand LoadCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }

        public ProjectViewModel(IRepository<T> repo)
        {
            _repo = repo;

            // LoadCommand — загружает данные из репозитория в коллекцию
            LoadCommand = new AsyncRelayCommand(async () =>
            {
                var list = await _repo.GetAllAsync();
                Items.Clear();
                foreach (var item in list)
                    Items.Add(item);
            });

            // AddCommand — добавляет новый элемент через репозиторий
            AddCommand = new AsyncRelayCommand(async () =>
            {
                var newItem = new T();
                await _repo.AddAsync(newItem);
                await _repo.SaveAsync();
                await ((AsyncRelayCommand)LoadCommand).ExecuteAsync(null);
            });

            // DeleteCommand — удаляет выбранный элемент через репозиторий
            DeleteCommand = new AsyncRelayCommand(
                async () =>
                {
                    if (SelectedItem == null) return;
                    await _repo.DeleteAsync(SelectedItem);
                    await _repo.SaveAsync();
                    await ((AsyncRelayCommand)LoadCommand).ExecuteAsync(null);
                },
                () => SelectedItem != null);
        }
    }
}