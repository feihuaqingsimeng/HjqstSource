
using System;
using System.Collections.Generic;
public static class TypeExpand
{
   
    public static T FindAttribute<T>(this Type type) where T : Attribute
    {
        foreach (object att in type.GetCustomAttributes(true))
        {           
            if (att.GetType() == typeof(T))
                return att as T;
        }
        return null;
    }

    /*
    * 扩展方法下无法正常工作 
   public static Type[] FindSubType(this Type obj)
   {
       Type tp = obj.GetType();
       List<Type> list = new List<Type>();
       System.Reflection.Assembly assmbly = System.Reflection.Assembly.GetAssembly(tp);
       foreach (Type item in assmbly.GetTypes())
       {
           if (item.IsSubclassOf(tp))
               list.Add(item);
       }
       return list.ToArray();
   }
   public static Type[] GetInterface<T>(this Type obj)
   {


       Type tp = obj.GetType();
       List<Type> list = new List<Type>();
       System.Reflection.Assembly assmbly = System.Reflection.Assembly.GetAssembly(typeof(T));
       foreach (Type item in assmbly.GetTypes())
       {
           Type add = item.GetInterface(typeof(T).FullName, true);
           if (add != null)
           {
               xDebug.LogError(add.FullName);
               list.Add(add);
           }
       }
       //xDebug.LogError("{0}",list.Count);
       return list.ToArray();
   }
   
   */
}