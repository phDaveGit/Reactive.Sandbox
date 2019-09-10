using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

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
        public ApiClientService()
        {
            Items = new List<NamedDto>();

            Observable
                .Interval(TimeSpan.FromSeconds(3))
                .Subscribe(x => Items.Add(new NamedDto {Id = (int) x, Name = x.ToString()}));
        }

        public List<NamedDto> Items { get; set; }

        public Task<IEnumerable<NamedDto>> GetList() => Task.FromResult(Items.AsEnumerable());
    }
}