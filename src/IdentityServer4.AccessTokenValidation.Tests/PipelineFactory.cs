﻿// Copyright (c) Dominick Baier & Brock Allen. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Tests.Util
{
    class PipelineFactory
    {
        public static TestServer CreateServer(Action<IdentityServerAuthenticationOptions> options)
        {
            return new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services
                        .AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                        .AddIdentityServerAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme, options);
                })
                .Configure(app =>
                {
                    app.UseAuthentication();

                    app.Run(async (context) =>
                    {
                        var user = context.User;
                        var isAuthenticated = user?.Identity?.IsAuthenticated == true;
                        context.Response.StatusCode = isAuthenticated ? 200 : 401;
                    });
                }));
        }

        public static HttpClient CreateClient(Action<IdentityServerAuthenticationOptions> options)
        {
            return CreateServer(options).CreateClient();
        }

        public static HttpMessageHandler CreateHandler(Action<IdentityServerAuthenticationOptions> options)
        {
            return CreateServer(options).CreateHandler();
        }
    }
}