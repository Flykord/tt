using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace testing
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        private Process externalProcess;

        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOMOVE = 0x0002;
        private const int SWP_SHOWWINDOW = 0x0040;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void openProgram(string progName)
        {
            externalProcess = Process.Start(progName);

            // Esperar a que la ventana del proceso externo se cargue completamente
            externalProcess.WaitForInputIdle();

            // Obtener el identificador de la ventana del proceso externo
            IntPtr externalWindowHandle = externalProcess.MainWindowHandle;

            // Superponer la ventana del proceso externo en este formulario
            if (externalWindowHandle != IntPtr.Zero)
            {
                SetParent(externalWindowHandle, this.Handle);
            }

            // Redimensionar y mover la ventana del proceso externo si es necesario
            // Esto depende de tus necesidades específicas

            // Liberar recursos cuando se cierre este formulario
            this.FormClosed += (s, e) =>
            {
                externalProcess.CloseMainWindow(); // Cierra el programa externo
                externalProcess.WaitForExit();
                externalProcess.Dispose();
            };
        }

        private void superposeProgram(string progName)
        {
            // Encuentra la ventana del programa que deseas superponer por su título
            IntPtr targetWindowHandle = FindWindow(null, progName);

            if (targetWindowHandle != IntPtr.Zero)
            {
                // Superpone la ventana del programa externo en este formulario
                SetWindowPos(targetWindowHandle, this.Handle, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW);
            }
            else
            {
                MessageBox.Show("La ventana del programa externo no se encontró.");
            }
        }

        private string getWindowName()
        {
            string processName = "Teams"; // Por ejemplo, para Google Chrome
            string windowTitle;
            // Buscar el proceso por nombre
            Process[] processes = Process.GetProcessesByName(processName);
            Process[] allProcesses  = Process.GetProcesses();

            if (processes.Length > 0)
            {
                // Obtener el título de la ventana principal del primer proceso encontrado
                windowTitle = processes[0].MainWindowTitle;

                if (!string.IsNullOrEmpty(windowTitle))
                {
                    MessageBox.Show("Título de la ventana: " + windowTitle);
                    return windowTitle;
                }
                else
                {
                    MessageBox.Show("La ventana no tiene un título.");
                    return "";
                }
            }
            else
            {
                Console.WriteLine("La aplicación no está en ejecución.");
                return "";
            }
        }
    
        private void superposeTest()
        {
            string processName = "QGATEv1.0";

            // Buscar el proceso por nombre
            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length > 0)
            {
                // Obtener el identificador de la ventana principal del primer proceso encontrado
                IntPtr windowHandle = processes[0].MainWindowHandle;

                if (windowHandle != IntPtr.Zero)
                {
                    Clipboard.SetText(textBox1.Text);
                    // Traer la ventana de Microsoft Teams al frente
                    SetForegroundWindow(windowHandle);
                    MessageBox.Show("Debería");
                    System.Threading.Thread.Sleep(1000);
                    SendKeys.SendWait("^(v)");
                }
                else
                {
                    MessageBox.Show("La ventana principal de Microsoft Teams no se encontró.");
                }
            }
            else
            {
                MessageBox.Show("Microsoft Teams no está en ejecución.");
            }
        }
    

    
    
        private void button1_Click(object sender, EventArgs e)
        {
            string prog = "C:/Program Files/Google/Chrome/Application/chrome.exe";
            string texto = textBox1.Text;
            Clipboard.SetText(texto);
            superposeTest();
            //prog=getWindowName();
            //if(string.IsNullOrEmpty(prog))
            //superposeProgram(prog);
            //else MessageBox.Show("No se encontro la intancia del programa");
        }

    }
}
