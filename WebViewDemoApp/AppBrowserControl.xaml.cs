﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WebViewDemoApp
{
    /// <summary>
    /// Interaction logic for AppBrowserControl.xaml
    /// </summary>
    public partial class AppBrowserControl : UserControl
    {
        public AppBrowserControl()
        {
            InitializeComponent();
            this.Unloaded += AppBrowserControl_Unloaded;
            this.Browser.CoreWebView2InitializationCompleted += Browser_CoreWebView2InitializationCompleted;
        }

        private void Browser_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            this.Browser.CoreWebView2.AddHostObjectToScript("cv_host", new AppBrowserControlScriptHelper(this));
        }

        public void AppBrowserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (Browser != null)
            {
                Browser.Dispose();
                Browser = null;
                Debug.WriteLine("made browser disposed");
            }
        }

        public async Task<bool> HasUnsavedChanges()
        {
            bool bret = false;
            if (Browser != null)
            {
                string ret = await Browser.CoreWebView2.ExecuteScriptAsync("haveUnsavedChangesIn()");
                bret = Boolean.Parse(ret);
            }
            return bret;
        }

        internal void PopOut()
        {
            //if (HasUnsavedChanges)
            //{
            //    var result =
            //        MessageBox.Show(
            //            "You have unsaved changes.  Are you sure you want to pop out and lose these changes",
            //            "Unsaved Changes", MessageBoxButton.YesNo);
            //    if (result == MessageBoxResult.Yes)
            //        RaiseAppPopOutEvent();
            //}
            //else
            //{
            Debug.WriteLine("Popout2");
            RaiseAppPopOutEvent();
            //}

        }

        internal void LogTrace(string type, string title, string message)
        {
            RaiseAppTraceEvent(type, title, message);
        }

        //Pop Out
        public static readonly RoutedEvent AppPopOutEvent =
            EventManager.RegisterRoutedEvent("AppPopOut", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AppBrowserControl));

        public event RoutedEventHandler AppPopOut
        {
            add { AddHandler(AppPopOutEvent, value); }
            remove { RemoveHandler(AppPopOutEvent, value); }
        }

        void RaiseAppPopOutEvent()
        {
            var newEventArgs = new RoutedEventArgs(AppPopOutEvent);
            Debug.WriteLine("Popout3");
            RaiseEvent((newEventArgs));
        }

        //Trace Log
        public static readonly RoutedEvent AppTraceEvent =
            EventManager.RegisterRoutedEvent("AppTrace", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AppBrowserControl));

        public event RoutedEventHandler AppTrace
        {
            add { AddHandler(AppTraceEvent, value); }
            remove { RemoveHandler(AppTraceEvent, value); }
        }

        void RaiseAppTraceEvent(string type, string title, string message)
        {
            var newEventArgs = new TraceLogEventArgs(AppTraceEvent, type, title, message);
            Debug.WriteLine("LogTrace2 " + title);
            RaiseEvent((newEventArgs));
        }
    }
}
