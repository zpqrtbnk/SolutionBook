// Copyright (C) 2020-2021 Pilotine / Stephane Gay / ZpqrtBnk
// 
// Licensed under the MIT License (https://opensource.org/licenses/MIT).
// You may not use this file except in compliance with the License.

using System;
using System.Reflection;

namespace SolutionBook.Services
{
    public class Types
    {
        internal PropertyInfo GetCountProp(Kind kind) =>
            CreateMruListType(kind).GetProperty("Count");

        internal PropertyInfo GetItemsProp(Kind kind) =>
            CreateMruListType(kind).GetProperty("Items");

        internal MethodInfo GetRemoveItemAtMethod(Kind kind) =>
            CreateMruListType(kind).GetMethod("RemoveItemAt");

        internal PropertyInfo GetPathProp() =>
            CreateType("FileSystemMruItem").GetProperty("Path");

        private static Type CreateMruListType(Kind kind)
        {
            return CreateType($"{kind}MruList");
        }

        private static Type CreateType(string name)
        {
            const string @namespace = "Microsoft.VisualStudio.PlatformUI";
            const string assembly = "Microsoft.VisualStudio.Shell.UI.Internal";

            return Type.GetType($"{@namespace}.{name}, {assembly}", throwOnError: true);
        }
    }
}
