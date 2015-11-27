using Spring.Context;
using Spring.Context.Support;
using System.Collections;
using System.Collections.Generic;

namespace SmartCustomerService.Core
{
    public class SpringContainer
    {
        private static IApplicationContext FApplicationContext;

        /// <summary>
        /// 获取应用程序上下文.
        /// </summary>
        /// <returns><see cref="IApplicationContext"/>应用程序上下文.</returns>
        public static IApplicationContext GetContext()
        {
            if (FApplicationContext == null)
            {
                FApplicationContext = ContextRegistry.GetContext();
            }
            return FApplicationContext;
        }

        /// <summary>
        /// 获取应用程序上下文.
        /// </summary>
        /// <param name="name"><see cref="IApplicationContext"/>应用程序上下文名称.</param>
        /// <returns><see cref="IApplicationContext"/>应用程序上下文.</returns>
        public static IApplicationContext GetContext(string name)
        {
            return ContextRegistry.GetContext(name);
        }

        /// <summary>
        /// 判断是否存在对象
        /// </summary>
        /// <param name="id">标识</param>
        /// <returns></returns>
        public static bool ContainsObject(string id)
        {
            return GetContext().ContainsObject(id);
        }

        /// <summary>
        /// 获取对象.
        /// </summary>
        /// <typeparam name="T">对象的类型.</typeparam>
        /// <param name="id">标识.</param>
        /// <returns></returns>
        public static T GetObject<T>(string id) where T : class
        {
            return GetContext().GetObject(id) as T;
        }

        /// <summary>
        /// 获取对象类表.
        /// </summary>
        /// <typeparam name="T">对象的类型.</typeparam>
        /// <returns></returns>
        public static IList<T> GetObjects<T>() where T : class
        {
            IDictionary<string,object> items = GetContext().GetObjectsOfType(typeof(T));
            IList<T> objects = new List<T>();
            foreach (var item in items.Values)
            {
                objects.Add((T)item);
            }
            return objects;
        }
    }
}
