﻿// Copyright (C) 2020-2021 Pilotine / Stephane Gay / ZpqrtBnk
// 
// Licensed under the MIT License (https://opensource.org/licenses/MIT).
// You may not use this file except in compliance with the License.

using System;
using System.Threading.Tasks;
using System.Windows;

namespace SolutionBook
{
    public static class TryCatch
    {
        public static void ReportException(Exception e)
        {
            var text = $"The SolutionBook extension has thrown an Exception!\n{e.GetType().Name}: {e.Message}\n{e.StackTrace}";
            MessageBox.Show(text, "SolutionBook", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static async Task<bool> RetryAsync(Func<Task> action, int delayMilliseconds, int totalMilliseconds)
        {
            var attempts = 0;
            var count = totalMilliseconds / delayMilliseconds;

            while (attempts == 0 && attempts++ < count)
            {
                try
                {
                    await action().ConfigureAwait(false);
                    return true;
                }
                catch (Exception e)
                {
                    if (attempts < count) await Task.Delay(delayMilliseconds); else ReportException(e);
                }
            }

            return false;
        }

        public static async Task<(bool success, T result)> RetryAsync<T>(Func<Task<T>> action, int delayMilliseconds, int totalMilliseconds)
        {
            var attempts = 0;
            var count = totalMilliseconds / delayMilliseconds;
            while (attempts == 0 && attempts++ < count)
            {
                try
                {
                    var result = await action().ConfigureAwait(false);
                    return (true, result);
                }
                catch (Exception e)
                {
                    if (attempts < count) await Task.Delay(delayMilliseconds); else ReportException(e);
                }
            }

            return (false, default);
        }
    }
}
