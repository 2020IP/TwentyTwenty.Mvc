using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TwentyTwenty.Mvc.DataTables.Core;
using Microsoft.AspNetCore.Mvc;
using TwentyTwenty.Mvc.DataTables;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Handles DataTables.AspNet registration and holds default (global) configuration options.
    /// </summary>
    public static class DataTablesServiceExtensions
    {
        public static IServiceCollection AddDataTables(this IServiceCollection services, 
            Func<ModelBindingContext, IDictionary<string, object>> parseRequestAdditionalParameters = null)
        {
            var modelBinder = new ModelBinder
            {
                ParseAdditionalParameters = parseRequestAdditionalParameters,
            };
            
            services.Configure<MvcOptions>(options =>
            {
                options.ModelBinderProviders.Insert(0, new ModelBinderProvider(modelBinder));
            });

            return services;
        }

        public static IServiceCollection AddDataTables(this IServiceCollection services, Action<DataTablesOptions> setupAction, 
            Func<ModelBindingContext, IDictionary<string, object>> parseRequestAdditionalParameters)
        {
            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            services.AddDataTables(parseRequestAdditionalParameters);
            
            services.Configure(setupAction);

            return services;
        }


        internal class ModelBinderProvider : IModelBinderProvider
        {
            private IModelBinder _modelBinder;
            
            public ModelBinderProvider(IModelBinder modelBinder)
            { 
                _modelBinder = modelBinder; 
            }

            public IModelBinder GetBinder(ModelBinderProviderContext context)
            {
                
                if (context.Metadata.ModelType == typeof(IDataTablesRequest))
                {
                    return _modelBinder;
                }                
                return null;
            }
        }
    }
}