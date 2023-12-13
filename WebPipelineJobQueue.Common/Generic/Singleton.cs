using System;

namespace WebPipelineJobQueue.Common.Generic
{
    public class Singleton<T> where T : class, new()
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null) instance = Activator.CreateInstance<T>();

                return instance;
            }
        }
    }
}
