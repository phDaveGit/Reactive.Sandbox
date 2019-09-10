using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace ListViewExample
{
    public interface IApiClientService
    {
        Task<IEnumerable<NamedDto>> GetList();
    }

    public class NamedDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ApiClientService : IApiClientService
    {
        private List<NamedDto> _items;

        public ApiClientService()
        {
            _items = new List<NamedDto> { new NamedDto { Id = 1, Name = "1" } };

            Observable
                .Interval(TimeSpan.FromSeconds(3))
                .Subscribe(x => _items.Add(new NamedDto {Id = (int) x, Name = x.ToString()}));
        }

        public async Task<IEnumerable<NamedDto>> GetList() => await Task.FromResult(_items.AsEnumerable());
    }
}