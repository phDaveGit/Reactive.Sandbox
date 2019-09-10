using System.Collections.Generic;
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
}