using System.Reflection;
using Castle.DynamicProxy;

namespace Shared;

internal class ProxyInterceptor<T> : IInterceptor
   {
      private readonly Dictionary<string, object> _propValues = new();
      private readonly PropertyInfo[] _interfaceProperties = typeof(T).GetProperties();
      
      public void Intercept(IInvocation invocation)
      {
         if (IsProperty(invocation))
         {
            HandleProperty(invocation);
         }
      }
      
      private bool IsProperty(IInvocation invocation)
      {
         var method = invocation.Method;
         return _interfaceProperties.Any(prop => prop.GetAccessors().Contains(method));
      }

      private void HandleProperty(IInvocation invocation)
      {
         var method = invocation.Method;
         var property = _interfaceProperties.First(prop => prop.GetAccessors().Contains(method));
        
         if (invocation.Method == property.SetMethod)
         {
            HandleSetProperty(invocation, property);
         }
         else
         {
            HandleGetProperty(invocation, property);
         }
      }

      private void HandleSetProperty(IInvocation invocation, PropertyInfo property)
      {
         var value = invocation.Arguments.First();
         _propValues[property.Name] = value;
      }

      private void HandleGetProperty(IInvocation invocation, PropertyInfo property)
      {
         if (!_propValues.ContainsKey(property.Name))
         {
            invocation.ReturnValue = GetDefaultPropertyValue(property);
         }
         else
         {
            invocation.ReturnValue = _propValues[property.Name];
         }
      }

      private object? GetDefaultPropertyValue(PropertyInfo propertyInfo)
      {
         return GetType()
                .GetMethod(nameof(GetDefaultGeneric), BindingFlags.Instance | BindingFlags.NonPublic)!
                .MakeGenericMethod(propertyInfo.PropertyType)
                .Invoke(this, null);
      }

      private TValue? GetDefaultGeneric<TValue>()
      {
         return default;
      }
   }