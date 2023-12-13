using System;
using WebPipelineJobQueue.Common.Helpers;

namespace WebPipelineJobQueue.Common.Extensions
{
    /// <summary>
    /// Author: Can DOĞU
    /// </summary>
    public static class GenericExtensions
    {
        public static string Serialize(this object item)
        {
            return JsonHelper.Instance.SerializeObject(item);
        }

        public static T Deserialize<T>(this object obj)
        {
            try
            {
                return JsonHelper.Instance.DeserializeObject<T>(obj);
            }
            catch (Exception ex)
            {
                LogHelper.Instance.Write(log: ex.ToString(), methodName: System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, exceptionId: 7008);
            }

            return default;
        }
    }
}
