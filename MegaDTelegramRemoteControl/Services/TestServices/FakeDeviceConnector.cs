﻿using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models.Device;
using MegaDTelegramRemoteControl.Services.Interfaces;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services.TestServices
{
    public class FakeDeviceConnector : IDeviceConnector
    {
        private readonly IDeviceDataParser deviceDataParser;

        public FakeDeviceConnector(IDeviceDataParser deviceDataParser)
        {
            this.deviceDataParser = deviceDataParser;
        }

        public Task<OperationResult<DevicePortStatus>> GetPortStatusAsync(DevicePort port)
        {
            return Task.FromResult(InvokeOperations.InvokeOperation(() =>
            {
                var bytes = new byte[8];

                using var generator = RandomNumberGenerator.Create();
                generator.GetBytes(bytes);

                var rand = BitConverter.ToInt64(bytes, 0);
                var data = rand % 2 == 0 ? SWStatus.On : SWStatus.Off;

                return deviceDataParser.ParseStatus(port, data.ToString());
            }));
        }
    }
}