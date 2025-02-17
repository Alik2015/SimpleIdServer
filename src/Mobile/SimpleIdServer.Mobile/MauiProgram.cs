﻿using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SimpleIdServer.Mobile.Services;
using SimpleIdServer.Mobile.Stores;
using SimpleIdServer.Mobile.ViewModels;
using ZXing.Net.Maui.Controls;

namespace SimpleIdServer.Mobile;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseBarcodeReader()
            .UseMauiCommunityToolkit()
			.RegisterFirebaseServices()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
		builder.Services.AddTransient<IPromptService, PromptService>();
        builder.Services.AddTransient<IOTPService, OTPService>();
		builder.Services.AddTransient<INavigationService, NavigationService>();
		builder.Services.AddTransient<IUrlService, UrlService>();
		builder.Services.AddSingleton(new OtpListState());
		builder.Services.AddSingleton(new CredentialListState());
        builder.Services.AddTransient<EnrollPage>();
		builder.Services.AddTransient<NotificationPage>();
		builder.Services.AddTransient<SettingsPage>();
		builder.Services.AddTransient<ViewOtpListPage>();
		builder.Services.AddTransient<QRCodeScannerPage>();
		builder.Services.AddTransient<ViewCredentialListPage>();
		builder.Services.AddTransient<QRCodeScannerViewModel>();
        builder.Services.AddTransient<EnrollViewModel>();
		builder.Services.AddTransient<SettingsPageViewModel>();
		builder.Services.AddTransient<ViewOtpListViewModel>();
		builder.Services.AddTransient<ViewCredentialListViewModel>();
		builder.Services.AddTransient<NotificationViewModel>();
        builder.Services.Configure<MobileOptions>(o =>
		{
			o.PushType = "firebase";
			o.IsDev = true;
        });
#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
