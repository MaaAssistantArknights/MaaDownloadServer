using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace MaaDownloadServer.Utils;

[AttributeUsage(AttributeTargets.Method)]
public class HttpRequestPriority : Attribute, IActionConstraint
{
    public readonly int Priority;

    public HttpRequestPriority(int priority = 1)
    {
        Priority = priority;
    }

    public int Order
    {
        get
        {
            return 0;
        }
    }

    public bool Accept(ActionConstraintContext context)
    {
        //check the other candidates
        foreach (var item in context.Candidates.Where(f => !f.Equals(context.CurrentCandidate)))
        {
            var attr = item.Action.ActionConstraints.FirstOrDefault(f => f.GetType() == typeof(HttpRequestPriority));

            if (attr == null)
            {
                return true;
            }
            else
            {
                HttpRequestPriority httpPriority = attr as HttpRequestPriority;
                if (httpPriority.Priority > Priority)
                    return false;
            }
        }
        return true;
    }
}
