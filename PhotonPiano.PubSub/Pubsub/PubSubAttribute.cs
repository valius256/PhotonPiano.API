using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PhotonPiano.DataAccess.Extensions;

namespace PhotonPiano.PubSub.Pubsub;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
public class PubSubAttribute : ActionFilterAttribute
{
    private readonly string _rootTopic;
    private ActionExecutingContext _context;
    public string[] paths;

    public PubSubAttribute(string rootTopic, params string[] paths)
    {
        Topic = new List<string>();
        if (paths.Length == 0)
        {
            Topic.Add(rootTopic);
        }
        else
        {
            _rootTopic = rootTopic;
            this.paths = paths;
        }
    }

    public PubSubAttribute(string rootTopic, string path1, string path2)
    {
        Topic = new List<string>();
        _rootTopic = rootTopic;
        paths = new[] { path1, path2 };
    }

    public List<string> Topic { get; set; }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        _context = context;
        await next();
    }

    public override void OnResultExecuted(ResultExecutedContext context)
    {
        if (context.Result is ObjectResult objectResult &&
            objectResult.StatusCode == 200) // Only trigger if request succeeded
        {
            if (paths != null && paths.Length > 0)
            {
                var result = context.Result;
                List<string> m_topics = new();

                foreach (var item in paths)
                {
                    if (item == ".")
                    {
                        m_topics.Add(_rootTopic);
                        continue;
                    }

                    var subTopic = GetTopic(item, result);
                    if (!string.IsNullOrEmpty(subTopic))
                        m_topics.Add(subTopic);
                }

                if (m_topics.Any())
                {
                    var uniqueTopics = m_topics.Distinct().ToList();
                    var pubSubService =
                        context.HttpContext.RequestServices.GetService(typeof(IPubSubService)) as IPubSubService;
                    pubSubService?.SendToAll(m_topics, "changed");
                }
            }
            else if (Topic.Any())
            {
                var pubSubService =
                    context.HttpContext.RequestServices.GetService(typeof(IPubSubService)) as IPubSubService;
                pubSubService?.SendToAll(Topic, "changed");
            }
        }
    }


    private string GetTopic(string fullPath, IActionResult? result)
    {
        var subId = "";
        var pathItems = (fullPath ?? "").Split("/");
        if (pathItems.Length > 0 && !string.IsNullOrEmpty(_rootTopic))
        {
            var objectResult = result as ObjectResult;
            var jResultdata = objectResult?.Value != null ? SerializeToJObject(objectResult.Value) : null;
            foreach (var path in pathItems)
            {
                var _s = GetDataBypath(jResultdata, path);
                if (!string.IsNullOrEmpty(_s))
                    subId += $"/{_s}";
            }
        }

        return $"{_rootTopic}{subId}";
    }

    private string GetDataBypath(JObject? jResultdata, string path)
    {
        var result = "";
        var paths = (path ?? "").Split(".");
        if (paths.Length > 0)
        {
            if (paths[0] == PubSubConst.QUERY && _context.ActionArguments.ContainsKey(paths[1]))
                return _context.ActionArguments[paths[1]]?.ToString() ?? "";

            var jdata = jResultdata;
            if (jdata == null) return result;

            result = GetData(jdata, string.Join(".", paths.Skip(1).Select(f => f.ToCamelCase()))) ?? "";
        }

        return result;
    }

    private string? GetData(JObject? jobj, string path)
    {
        return path == "." ? jobj.ToString() : jobj?.SelectToken(path)?.ToString();
    }

    public static JObject? SerializeToJObject(object obj)
    {
        return (JObject)JsonConvert.DeserializeObject(obj.SerializeObject())!;
    }
}

public static class StringExtensions
{
    public static string ToCamelCase(this string str)
    {
        if (string.IsNullOrEmpty(str) || !char.IsUpper(str[0]))
            return str;

        var chars = str.ToCharArray();
        for (var i = 0; i < chars.Length; i++)
        {
            if (i == 1 && !char.IsUpper(chars[i]))
                break;
            var hasNext = i + 1 < chars.Length;
            if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                break;
            chars[i] = char.ToLowerInvariant(chars[i]);
        }

        return new string(chars);
    }
}