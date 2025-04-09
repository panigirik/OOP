using ConsoleWord;
using ConsoleWord.Application.Commands;
using ConsoleWord.Application.DocumentUseCases;
using ConsoleWord.Application.Dropbox;
using ConsoleWord.Application.Services;
using ConsoleWord.Infrastracture;
using ConsoleWord.Infrastracture.CloudStorage.Interfaces;
using ConsoleWord.Infrastracture.CloudStorage.Services;
using ConsoleWord.Infrastracture.LocalStorage.Interfaces;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
// Эта директива приводит ICloudStorageProvider к типу из Dropbox


class Program
{
    public const string dropboxToken = "sl.u.AFrkgt4JQaigkApyMC_yfRRHoJ_nA_i_aTZ0jOd8DAx-FRa58u3wHYwgsrbCz9bvrF8pIa7_IWPGcBujVSrnpogQNSq3Ulq-AdMg8Y-tkPBa3H7vJhAm77hhJi_fQb-m0-RlNBmZoX8vK32pRBtrdawbYVnRvwl-CcciF2Ra58SapA0DnvEJJSamHfJqWn3nir1guGgQYLuJ9PxCudIlx2yJf-oOlrUXdjFoVAHLf6PPD3B6ADJnyCSfTBa_eZMvfQ8A2Uuw8HPEkdfJywP2pC9t8UKfPnIwyQsLICfthWudFguUhmfNUC_UQOIAvh39H_n7uVKZy9DZI5vkSV8YNCdpYqHG6p-iy1zaRf_R8nUzSC4YU4j71oGiHoHXVGy6_RfxRRrfHQEviq2CECsNjehbngCnc-KYnIXsSX6S34JqrN0w-86l8eP6D5reHg1Fr8CfEHlyzWf9U3rVQyLDP-H4sk8MdXZ5xXU-iMK-v05nfotcXCTZAFx4-bJ6CGPntfhZUNAFOhQ3RttYy5b42y-F-VZjRdTlKP6elgcPTvjpdXsfZ3kisGOKa6daDPoBCACYIvqeRxFmsXrpJKmanydGBmi5Oqkh9tJ_CEJOeotj_w82cohPB_PtPwJo_FnlT39o747MWRtSz1RVIi_Jf8P-joV2L6Xy6A56Q3-XMgzDEFWWfztiZcr1fI0FaPRdCBSt5dhoUV6UlbAyiDcpxzc5SeygjvWB2eySFaaKfdDkVa1HkUdZEQ8olRohX0A0N6Vps5Z8DxL7TWthMWpDiciYUJ1LL4_BVfl4gx5G7vXJj1ENhIx-xIM-IdE4F5PXnyEkbOgf8ZD2hgCB9Ucato8X0L4jhfi4pSGdRKr1n_BPANMD6P4-kmMrr5nFwHwhxlV35V27XV5V-QAX8yPou1JLDWDJQEHAwh7IWvOob8JBFtKIhfU5oDFEtj8AyR7WOY_UhgViiDzfseJfLjp7eGLYFdcSvi2_ap1Skw95atMLFKh0uA-taBGAgPvk7ZAAXjSFd-C9Ak95LLqRoFf_kqEXZYXbk6In3Oc65_Ev0pK7HBTpMpdEpvxptaV4FKati9vZ7UHh1iAoDNIHiMGXkS2x5m3nFaqVrHGmpCGh92z1K2z3yAJBOHncD4Wfx9Y2n4ki1Q8nAkqlYxkHHYobSObQ2AkmrHaxWKHkZSBsz-17eyqDiaWbX7oEJy315cHvLjYmR5fG0ykLqqMjvsO6u6uRk9jqg87vxKY4GBgXSwuSZmJApgw8fx69-wqLm0yjQW4Mbgs29lPIKIwxcx05gRCvqCGK7NEsq7SDVElUNJ_-H71njfyzG7hztWdxbIu_fg2FEIjf5sEaUsP7Bgggymjya0ilRgP-mSXvYx2wiPs3Et_PeXFfi-f1hZm82Lqd8b56fR4Wewnu3VWex9mLrz8m8fS_svBqa5aq4jrNE2abgQ\n"; // укорочено

    static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                string notificationFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "notifications.txt");

                // Регистрация зависимости для пути к файлу уведомлений
                services.AddSingleton(notificationFile);
                services.AddSingleton<NotificationService>();

                // Регистрация хранилища: регистрируем IStorageProvider и ICloudStorageProvider
                services.AddSingleton<IStorageProvider, LocalFileStorageProvider>();
                // Вместо CloudFileStorage, регистрируем DropboxStorageProvider, который реализует ICloudStorageProvider из Dropbox
                services.AddSingleton<ICloudDropBoxStorageProvider>(provider => new DropboxStorageProvider(dropboxToken));

                // Регистрация других сервисов приложения
                services.AddSingleton<StorageService>();
                services.AddScoped<IDocumentCommand, DeleteDocumentCommand>();
                services.AddSingleton<UndoRedoService>();
                services.AddSingleton<AuthenticationService>();
                services.AddSingleton<FormattingService>();
                services.AddSingleton<CloudFileStorage>();
                services.AddSingleton<DocumentEditor>();
                services.AddSingleton<DocumentFactory>();
                services.AddSingleton<DocumentLoader>();
                services.AddSingleton<DocumentStorageService>();
                services.AddSingleton<InputHelper>();
                services.AddSingleton<DocumentService>();
                services.AddSingleton<Menu>();  // Menu будет использовать ICloudStorageProvider

                // Регистрация Hangfire
                services.AddHangfire(config => config.UseMemoryStorage());
                services.AddHangfireServer();
            })
            .Build();

        // Получение и отображение меню
        var menu = host.Services.GetRequiredService<Menu>();
        menu.Show();

        // Работа с уведомлениями
        var notifier = host.Services.GetRequiredService<NotificationService>();
        notifier.SendNotification("admin", "Test message");

        Console.WriteLine("Notification sent. Waiting for background job to complete...");
        System.Threading.Thread.Sleep(5000);

        var userNotifications = notifier.GetUserNotifications("admin");
        Console.WriteLine("\nNotifications for user 'admin':");
        foreach (var notification in userNotifications)
        {
            Console.WriteLine(notification);
        }

        Console.WriteLine("Check notifications.txt now.");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
