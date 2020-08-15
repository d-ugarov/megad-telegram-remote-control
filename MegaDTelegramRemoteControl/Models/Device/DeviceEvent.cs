﻿using MegaDTelegramRemoteControl.Infrastructure.Models;
using System;
using System.Text;

namespace MegaDTelegramRemoteControl.Models.Device
{
    public class DeviceEvent
    {
        public static DeviceEvent UnknownEvent => new DeviceEvent {Type = DeviceEventType.Unknown};
        
        private DevicePortEvent port;
        
        public DeviceEventType Type { get; set; }
        
        public DateTime Date { get; set; }
        
        public bool IsParsedSuccessfully { get; set; }
        
        public Device Device { get; set; }

        public DevicePortEvent Port
        {
            get => Type == DeviceEventType.PortEvent
                ? port
                : throw new OperationException($"Port is not available for event type {Type}");
            set => port = value;
        }

        public override string ToString()
        {
            var str = new StringBuilder($"Event: {Type}");

            if (Device != null)
                str.Append($", device: {Device.Name}");

            if (port.Port == null)
                str.Append($", {port.Port}");

            str.Append(IsParsedSuccessfully ? ", +" : ", -");

            return str.ToString();
        }
    }
}