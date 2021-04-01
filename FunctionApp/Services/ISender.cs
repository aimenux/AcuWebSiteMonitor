using System.Threading.Tasks;
using FunctionApp.Models;

namespace FunctionApp.Services
{
    public interface ISender
    {
        Task SendAsync(Notification notification);
    }
}
