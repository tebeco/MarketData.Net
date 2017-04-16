// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Sockets.Routing
{
    internal static class RouteBuilderExtensions
    {
        public static IRouteBuilder AddPrefixRoute(
            this IRouteBuilder routeBuilder,
            string prefix,
            IRouteHandler handler)
        {
            routeBuilder.Routes.Add(new PrefixRoute(handler, prefix));
            return routeBuilder;
        }
    }
}
