using SecurityExplorer.SampleWebApi.Entities;

namespace SecurityExplorer.SampleWebApi.Services
{
    public class EmployeeTrackerService
    {

        Stack<string> _activityStack = new Stack<string>();

        Queue<User> _registrationQueue = new Queue<User>();


        void EnqueueRegistration(User user)
        {
            _registrationQueue.Enqueue(user);
        }

        User? ProcessNextRegistration()
        {
            if (_registrationQueue.Count > 0)
            {
                //return _registrationQueue.Peek();

                return _registrationQueue.Dequeue();
            }
            return null;
        }

        void LogActivity(string activity)
        {
            _activityStack.Push(activity);
        }

        List<string> GetRecentActivities(int count)
        {
            List<string> recentActivities = new List<string>();
            foreach (var activity in _activityStack.Take(count))
            {
                recentActivities.Add(activity);
            }
            return recentActivities;
        }




    }
}
