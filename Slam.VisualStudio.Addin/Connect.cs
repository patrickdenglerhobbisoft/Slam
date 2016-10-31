using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell.Interop;
using System.Resources;
using System.Reflection;
using System.Globalization;
using Microsoft.VisualStudio.CommandBars;
namespace Slam.VisualStudio.Addin
{
    /// <summary>The object for implementing an Add-in.</summary>
    /// <seealso class='IDTExtensibility2' />
    public class Connect : IDTExtensibility2
    {
        EnvDTE80.Commands2 commands;
        /// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
        public Connect()
        {
        }

        public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {

            OnConnectionSetup(application, connectMode, addInInst, ref  custom);
                 LoadWindow(application, connectMode, addInInst, ref  custom);
         
        }

        private void LoadWindow(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            _applicationObject = (DTE2)application;
            _addInInstance = (AddIn)addInInst;


            try
            {
                // ctlProgID - the ProgID for your user control. asmPath - the path to your user control DLL. guidStr - a unique GUID for the user control.
                string ctlProgID, asmPath, guidStr;
                // Variables for the new tool window that will hold
                // your user control.
                EnvDTE80.Windows2 toolWins;
                EnvDTE.Window toolWin;
                object objTemp = null;



                ctlProgID = "Slam.Visualizers";
                // Replace the <Path to VS Project> with the path to
                // the folder where you created the WindowsCotrolLibrary.
                // Remove the line returns from the path before 
                // running the add-in.
                asmPath = @"C:\Users\Patrick\Documents\GitHub\bin\Slam.Visualizers.exe";
                guidStr = "{9ED54F84-A89D-4fcd-A854-44251E922109}";

                toolWins = (Windows2)_applicationObject.Windows;
                // Create the new tool window, adding your user control.
                toolWin = toolWins.CreateToolWindow2(_addInInstance,
                  asmPath, ctlProgID, "Slam Visualizers", guidStr,
                  ref objTemp);


                // The tool window must be visible before you do anything 
                // with it, or you will get an error.
                if (toolWin != null)
                {
                    toolWin.Visible = true;
                }

                toolWin.Height = 500;
                toolWin.Width = 400;


                // Get the service provider on the object
                Microsoft.VisualStudio.Shell.ServiceProvider serviceProvider = new Microsoft.VisualStudio.Shell.ServiceProvider(this._applicationObject as Microsoft.VisualStudio.OLE.Interop.IServiceProvider);

                // Get the shell service
                var vsUIShell = (IVsUIShell)serviceProvider.GetService(typeof(SVsUIShell));
                Guid slotGuid = new Guid("63ed935d-eac2-4b8f-9208-f28f48d49f6b");

                // Find the associated window frame on this toolwindow
                IVsWindowFrame wndFrame;
                vsUIShell.FindToolWindow((uint)__VSFINDTOOLWIN.FTW_fFrameOnly, ref slotGuid, out wndFrame);

                // Set the text on the window tab name
                wndFrame.SetProperty((int)__VSFPROPID.VSFPROPID_Caption, "Slam");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void OnConnectionSetup(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
        {
            _applicationObject = (DTE2)application;
            _addInInstance = (AddIn)addInInst;
            if (connectMode == ext_ConnectMode.ext_cm_UISetup)
            {
                object[] contextGUIDS = new object[] { };
                Commands2 commands = (Commands2)_applicationObject.Commands;
                string toolsMenuName;
                try
                {
                    ResourceManager resourceManager = new
                    ResourceManager("Slam.VisualStudio.Addin.CommandBar",
                    Assembly.GetExecutingAssembly());
                    CultureInfo cultureInfo = new
                      System.Globalization.CultureInfo
                     (_applicationObject.LocaleID);
                    string resourceName =
                    String.Concat(cultureInfo.TwoLetterISOLanguageName,
                      "Tools");
                    toolsMenuName = resourceManager.GetString(resourceName);
                }
                catch
                {
                    toolsMenuName = "Tools";
                }

                CommandBar menuBarCommandBar =
                    ((CommandBars)_applicationObject.CommandBars)["MenuBar"];
                CommandBarControl toolsControl =
                  menuBarCommandBar.Controls[toolsMenuName];
                CommandBarPopup toolsPopup = (CommandBarPopup)toolsControl;
                try
                {
                    Command command = commands.AddNamedCommand2(_addInInstance, "Slam.VisualStudio.Addin", "Slam", "Loads Slam Injector Host", true, 59, ref contextGUIDS, 
                      (int)vsCommandStatus.vsCommandStatusSupported+
                      (int)vsCommandStatus.vsCommandStatusEnabled, 
                      (int)vsCommandStyle.vsCommandStylePictAndText,  
                      vsCommandControlType.vsCommandControlTypeButton);

                    if((command != null) && (toolsPopup != null))
                    {
                        command.AddControl(toolsPopup.CommandBar, 1);
                    }
                }
                catch(System.ArgumentException)
                {
                }
            }
        }

        public void QueryStatus(string commandName, vsCommandStatusTextWanted
          neededText, ref vsCommandStatus status, ref object commandText)
        {
            if (neededText ==
              vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
            {
                if (commandName == "Slam.VisualStudio.Addin.Connect.Slam")
                {
                    status = (vsCommandStatus)vsCommandStatus.
                      vsCommandStatusSupported | vsCommandStatus.
                      vsCommandStatusEnabled;
                    return;
                }
            }
        }

        public void Exec(string commandName, vsCommandExecOption executeOption,
          ref object varIn, ref object varOut, ref bool handled)
        {
            handled = false;
            if (executeOption ==
              vsCommandExecOption.vsCommandExecOptionDoDefault)
            {
                if (commandName == "Slam.VisualStudio.Addin.Connect.Slam")
                {
                    handled = true;
                    return;
                }
            }
        }
        /// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
        /// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
        {
        }

        /// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. Receives notification when the collection of Add-ins has changed.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />		
        public void OnAddInsUpdate(ref Array custom)
        {
        }

        /// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface. Receives notification that the host application has completed loading.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnStartupComplete(ref Array custom)
        {
        }

        /// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. Receives notification that the host application is being unloaded.</summary>
        /// <param term='custom'>Array of parameters that are host application specific.</param>
        /// <seealso class='IDTExtensibility2' />
        public void OnBeginShutdown(ref Array custom)
        {
        }

        private DTE2 _applicationObject;
        private AddIn _addInInstance;
    }
}