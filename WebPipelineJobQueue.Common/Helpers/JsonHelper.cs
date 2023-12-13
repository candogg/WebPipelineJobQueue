using Newtonsoft.Json;
using System;
using WebPipelineJobQueue.Common.Extensions;
using WebPipelineJobQueue.Common.Generic;

namespace WebPipelineJobQueue.Common.Helpers
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public class JsonHelper : Singleton<JsonHelper>
    {
        public T DeserializeObject<T>(object obj)
        {
            if (obj == null) return Activator.CreateInstance<T>();

            try
            {
                return JsonConvert.DeserializeObject<T>(Convert.ToString(obj), new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Write(log: ex.ToString(), data: obj.Serialize(), methodName: System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, exceptionId: 7061);
            }

            return Activator.CreateInstance<T>();
        }

        public string SerializeObject(object obj, bool isCamelCase = false)
        {
            try
            {
                if (isCamelCase)
                {
                    return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                    });
                }
                else
                {
                    return JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                }
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, exceptionId: 7062);
            }

            return string.Empty;
        }
    }
}
