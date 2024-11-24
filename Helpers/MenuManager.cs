using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.IO;
using System.Security.Policy;
using System.Linq; // Ensure you have this to access First()

public class MenuManager
{
    public void CreateMenu()
    {
        bool isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        MenuItem[] menu = null!;

        // Add an IPC listener
        Electron.IpcMain.On("invoke-menu-item", async (args) =>
        {
            if (args.ToString() == "zebra")
            {
                // Call the specific menu item function here, e.g.,
                await CallYourMenuItemFunctionAsync();
            }
        });

        MenuItem[] appMenu = new MenuItem[]
        {
            new MenuItem { Role = MenuRole.about },
            new MenuItem { Type = MenuType.separator },
            new MenuItem { Role = MenuRole.services },
            new MenuItem { Type = MenuType.separator },
            new MenuItem { Role = MenuRole.hide },
            new MenuItem { Role = MenuRole.hideothers },
            new MenuItem { Role = MenuRole.unhide },
            new MenuItem { Type = MenuType.separator },
            new MenuItem { Role = MenuRole.quit }
        };

        MenuItem[] fileMenu = new MenuItem[]
        {
            new MenuItem { Role = isMac ? MenuRole.close : MenuRole.quit }
        };

        MenuItem[] viewMenu = new MenuItem[]
        {
            new MenuItem { Role = MenuRole.reload },
            new MenuItem { Role = MenuRole.forcereload },
            new MenuItem { Role = MenuRole.toggledevtools },
            new MenuItem { Type = MenuType.separator },
            new MenuItem { Role = MenuRole.resetzoom },
            new MenuItem { Role = MenuRole.zoomin },
            new MenuItem { Role = MenuRole.zoomout },
            new MenuItem { Type = MenuType.separator },
            new MenuItem { Role = MenuRole.togglefullscreen }
        };

        MenuItem[] aboutMenu = new MenuItem[]
        {
            new MenuItem
            {
                Label = "Home Page",
                Click = async () => await Electron.Shell.OpenExternalAsync("https://ainetprofit.com/")
            },
            new MenuItem
            {
                Label = "Videos",
                Click = async () => await Electron.Shell.OpenExternalAsync("https://www.youtube.com/@StationBreakTV")
            },
            new MenuItem
            {
                Label = "StationBreak™",
                Click = async () => await Electron.Shell.OpenExternalAsync("https://stationbreaktv.com/")
            }  
        };

        MenuItem[] companiesMenu = new MenuItem[]
        {
            new MenuItem
            {
                Label = "Companies",
                Click = () =>
                {
                    var mainWindow = Electron.WindowManager.BrowserWindows.FirstOrDefault();
                    if (mainWindow != null)
                    {
                        mainWindow.LoadURL("http://localhost:5001/Company/Index"); 
                    }
                }
            },
            new MenuItem
            {
                Label = "Open Developer Tools",
                Accelerator = "CmdOrCtrl+I",
                Click = () => Electron.WindowManager.BrowserWindows.First().WebContents.OpenDevTools()
            },
            new MenuItem
            {
                Type = MenuType.separator
            },            
            new MenuItem
            {
                Label = "App Menu Demo",
                Click = async () => {
                    var options = new MessageBoxOptions("This demo is for the Menu section, showing how to create a clickable menu item in the application menu.");
                    options.Type = MessageBoxType.info;
                    options.Title = "Application Menu Demo";
                    await Electron.Dialog.ShowMessageBoxAsync(options);
                }
            }
        };

        if (isMac)
        {
            menu = new MenuItem[]
            {
                new MenuItem { Label = "Electron", Type = MenuType.submenu, Submenu = appMenu },
                new MenuItem { Label = "File", Type = MenuType.submenu, Submenu = fileMenu },
                new MenuItem { Label = "View", Type = MenuType.submenu, Submenu = viewMenu },
                new MenuItem { Label = "Companies", Type = MenuType.submenu, Submenu = companiesMenu },
                new MenuItem { Label = "About", Type = MenuType.submenu, Submenu = aboutMenu },
            };
        }
        else
        {
            menu = new MenuItem[]
            {
                new MenuItem { Label = "File", Type = MenuType.submenu, Submenu = fileMenu },
                new MenuItem { Label = "View", Type = MenuType.submenu, Submenu = viewMenu },
                new MenuItem { Label = "Companies", Type = MenuType.submenu, Submenu = companiesMenu },
                new MenuItem { Label = "About", Type = MenuType.submenu, Submenu = aboutMenu },
            };
        }

        Electron.Menu.SetApplicationMenu(menu);
    }

    public async Task<BrowserWindow> CreateWindow(BrowserWindowOptions options)
    {
        CreateMenu();

        var window = await Electron.WindowManager.CreateWindowAsync(options);

        window.OnClosed += () =>
        {
            Electron.App.Quit();
        };

        return window;
    }

    private async Task CallYourMenuItemFunctionAsync()
    {
        await Electron.Shell.OpenExternalAsync("https://maxlifespan.com/");

    }
}
