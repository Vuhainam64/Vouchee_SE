﻿using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Business.Models;
using Vouchee.Data.Helpers;

namespace Vouchee.Business.Services.Extensions.RedisCache
{
    public static class DeviceTokenCache
    {
        public static async Task UpdateDeviceToken(this IDistributedCache cache, string userId, string token)
        {
            try
            {
                string key = "Device-" + userId;
                DeviceToken result = await DistributedCacheExtensions.GetRecordAsync<DeviceToken>(cache, key);
                if (result == null)
                {
                    result = new DeviceToken
                    {
                        Tokens = new()
                        {
                            token
                        }
                    };
                }
                else
                {
                    string check = result.Tokens.Find(x => x.Equals(token));
                    check ??= "";
                    if (check.Equals(""))
                        result.Tokens.Add(token);
                }
                await DistributedCacheExtensions.SetRecordAsync(cache, key, result);
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public static async Task<DeviceToken> GetAvailableDevice(this IDistributedCache cache, string userId)
        {
            try
            {
                string key = "Device-" + userId;
                DeviceToken result = await DistributedCacheExtensions.GetRecordAsync<DeviceToken>(cache, key);
                if (result == null)
                {
                    return result = new()
                    {
                        Tokens = new()
                    };
                }
                var message = new MulticastMessage()
                {
                    Tokens = result.Tokens,
                };

                var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message, true);

                var failedTokens = new List<string>();
                if (response.FailureCount > 0)
                {
                    for (var i = 0; i < response.Responses.Count; i++)
                    {
                        if (!response.Responses[i].IsSuccess)
                        {
                            failedTokens.Add(result.Tokens[i]);
                        }
                    }
                }
                result.Tokens = result.Tokens.Except(failedTokens).ToList();

                await DistributedCacheExtensions.SetRecordAsync(cache, key, result);
                return result;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public static async Task<List<string>> ValidateDeviceToken(List<string> newTokens)
        {
            try
            {
                var message = new MulticastMessage()
                {
                    Tokens = newTokens
                };

                var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message, true);

                var failedTokens = new List<string>();
                if (response.FailureCount > 0)
                {
                    for (var i = 0; i < response.Responses.Count; i++)
                    {
                        if (!response.Responses[i].IsSuccess)
                            failedTokens.Add(newTokens[i]);

                    }
                }

                return newTokens.Except(failedTokens).ToList();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public static async Task<List<string>> SubcribeUserToTopic(this IDistributedCache cache, string topic, string userId)
        {
            try
            {
                List<string> userIdList = await DistributedCacheExtensions.GetRecordAsync<List<string>>(cache, topic);
                if (userIdList == null)
                {
                    userIdList = new()
                    {
                        userId
                    };
                }
                else
                {
                    string check = userIdList.Find(x => x.Equals(userId));
                    check ??= "";
                    if (check.Equals(""))
                        userIdList.Add(userId);

                }

                await DistributedCacheExtensions.SetRecordAsync(cache, topic, userIdList);

                return userIdList;

            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public static async Task<List<string>> GetSubcribedUserFromTopic(this IDistributedCache cache, string topic)
        {
            try
            {
                List<string> userIdList = await DistributedCacheExtensions.GetRecordAsync<List<string>>(cache, topic);
                if (userIdList != null)
                    return userIdList;
                else
                    return userIdList = new();
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }

        public static async Task<List<string>> UnsubcribeUserToTopic(this IDistributedCache cache, string topic, string userId)
        {
            try
            {
                List<string> userIdList = await DistributedCacheExtensions.GetRecordAsync<List<string>>(cache, topic);
                if (userIdList != null)
                {
                    if (userIdList.Remove(userId))
                        return userIdList;
                }

                await DistributedCacheExtensions.SetRecordAsync(cache, topic, userIdList);
                return userIdList;
            }
            catch (Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }
        }
    }
}