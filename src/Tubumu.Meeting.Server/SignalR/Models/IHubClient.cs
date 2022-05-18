using System.Threading.Tasks;

namespace Tubumu.Meeting.Server
{
    public interface IHubClient
    {
        Task Notify(MeetingNotification notification);
    }
}
