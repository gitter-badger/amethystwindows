﻿using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vanara.PInvoke;

namespace AmethystWindowsSystray
{
    static class Program
    {
        static Mutex mutex = new Mutex(
            true,
            "AmethysyWindowsSysTrayService"
            );

        public class Options
        {
            [Option('s', "standalone", Required = false, HelpText = "Launch without UWP companion.")]
            public bool Standalone { get; set; }
        }

        [STAThread]
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                  .WithParsed<Options>(o =>
                  {
                      if (mutex.WaitOne(TimeSpan.Zero, true))
                      {
                          Application.EnableVisualStyles();
                          Application.SetCompatibleTextRenderingDefault(false);
                          if (o.Standalone)
                          {
                              Application.Run(new SystrayContext(o.Standalone));
                          }
                          else
                          {
                              Application.Run(new SystrayContext());
                          }

                          mutex.ReleaseMutex();
                      }
                      else
                      {
                          uint messageValue = User32.RegisterWindowMessage("AMETHYSTSYSTRAY_RECONNECT");
                          User32.PostMessage(
                              (HWND)(IntPtr)0xffff,
                              messageValue
                              );
                      }
                  });
        }
    }
}
