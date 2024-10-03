using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp1
{
    internal class IpMonitor
    {
        private static bool IsIpAvailable(string ipAddress)
        {
            Ping ping = new Ping();
            try
            {
                PingReply reply = ping.Send(ipAddress, 1000);
                return reply.Status == IPStatus.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void StartMonitoring()
        {
            while (true)
            {
                bool isAvailable = IsIpAvailable("192.168.88.2");

                if (!isAvailable)
                {
                    ShowScreensaverOnAllMonitors(); // Показать блокирующие формы, если сервер недоступен
                }
                else
                {
                    CloseAllScreensaverForms(); // Закрыть формы, если сервер снова доступен
                    InputBlocker.UnblockInput(); // Разблокировать клавиатуру и мышь
                    if (IsCtrl9Pressed())
                    {
                        InputBlocker.UnblockInput(); // Разблокировка через Ctrl + 9
                    }

                }

                Thread.Sleep(5000); // Пауза в 5 секунд перед следующей проверкой
            }
        }


        private static void ShowScreensaverOnAllMonitors()
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                FullScreenForm form = new FullScreenForm(screen.Bounds);
                form.Show();
            }
            System.Windows.Forms.Application.Run(); // Запуск всех окон
        }
        public static void CloseAllScreensaverForms()
        {
            foreach (Form form in System.Windows.Forms.Application.OpenForms)
            {
                if (form is FullScreenForm)
                {
                    form.Invoke(new Action(() => form.Close())); // Безопасное закрытие форм из другого потока
                }
            }
        }

    }
}