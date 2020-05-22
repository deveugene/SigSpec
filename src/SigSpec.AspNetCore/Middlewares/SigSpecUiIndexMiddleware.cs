﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SigSpec.AspNetCore.Middlewares
{
    internal class SwaggerUiIndexMiddleware
    {
        private readonly RequestDelegate _nextDelegate;
        private readonly string _indexPath;
        private readonly SigSpecSettings _settings;
        private readonly string _resourcePath;

        public SwaggerUiIndexMiddleware(RequestDelegate nextDelegate, string indexPath, SigSpecSettings settings, string resourcePath)
        {
            _nextDelegate = nextDelegate;
            _indexPath = indexPath;
            _settings = settings;
            _resourcePath = resourcePath;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.HasValue && string.Equals(context.Request.Path.Value.Trim('/'), _indexPath.Trim('/'), StringComparison.OrdinalIgnoreCase))
            {
                var stream = typeof(SwaggerUiIndexMiddleware).GetTypeInfo().Assembly.GetManifestResourceStream(_resourcePath);
                using (var reader = new StreamReader(stream))
                {
                    context.Response.Headers["Content-Type"] = "text/html; charset=utf-8";
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync(_settings.TransformHtml(await reader.ReadToEndAsync(), context.Request));
                }
            }
            else
            {
                await _nextDelegate(context);
            }
        }
    }
}