﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Anti_Ransomware
{
    class GUI
    {
        [DllImport(@"DLL_Files_Change.dll")]
        static extern string Files_Change(string dir_path);//, ref  variables_t variables);

        private const int PORT = 8000;
        private MainForm mainForm;
        Socket sockListener;
        Socket sock;
        public string[] Trap = { @"C:\A", @"C:\B" };

        Process process = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "OpenFileFinder.exe"
            }
        };

        public GUI(MainForm mainForm)
        {
            // TODO: Complete member initialization
            this.mainForm = mainForm;
            process.Start();
            Thread HandleThread = new Thread(Run);
            HandleThread.Start();
            Thread RDCThread = new Thread(ReadDirectoryChanges);
            RDCThread.Start();
        }
        
        public void ReadDirectoryChanges()
        {
            while (true){
                string[] action = Files_Change(Trap[0]).Split('#');
                if (action[0]==)
                {
                    
                }
            }
        }

        private void Run()
        {
            try
            {
                sockListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sockListener.Bind(new IPEndPoint(IPAddress.Loopback, PORT));
                sockListener.Listen(1);

                sock = sockListener.Accept();
                byte[] buf = new byte[1024];
                while (true)
                {
                    int recv = sock.Receive(buf, 0, buf.Length, SocketFlags.None);
                    string msg = Encoding.ASCII.GetString(buf).Substring(0, recv);
                    mainForm.Invoke((MethodInvoker)delegate
                    {
                        string[] fields = msg.Split('#');
                        ListViewItem itemFound = null;
                        for (int i = 0; i < mainForm.ListView1.Items.Count && itemFound == null ; i++ )
                        {
                            if ( mainForm.ListView1.Items[i].SubItems[0].Text.Equals(fields[0]) )
                                itemFound = mainForm.ListView1.Items[i];
                        }
                        if (itemFound == null)
                            mainForm.ListView1.Items.Add(new ListViewItem(fields));
                        else
                            itemFound.SubItems[3].Text = fields[3];
                    });
                }
            }
            catch (SocketException ex)
            {
                mainForm.Invoke((MethodInvoker)delegate
                {
                   // mainForm.RichTextBoxLog.AppendText(ex.Message + Environment.NewLine);
                });
            }
        }

        public void Send(string command)
        {
            sock.Send(Encoding.ASCII.GetBytes(command));
        }

        public void Close()
        {
            process.Close();
            Environment.Exit(0);
        }
    }
}
